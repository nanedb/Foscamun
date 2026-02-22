using Foscamun2026.Models;
using Foscamun2026.Repositories;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Foscamun2026.Data
{
    public class SqliteDataAccess
    {
        private readonly string _connectionString;

        public CommitteeRepository CommitteeRepository { get; init; }
        public CountryRepository CountryRepository { get; init; }
        public ICJRepository ICJRepository { get; init; }

        /// <summary>
        /// Initializes the SQLite database connection.
        /// Ensures the application data folder and DB file exist.
        /// </summary>
        public SqliteDataAccess()
        {
            string folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Foscamun2026"
            );

            Directory.CreateDirectory(folder);

            string dbPath = Path.Combine(folder, "Foscamun.db");
            _connectionString = $"Data Source={dbPath}";
            CommitteeRepository = new CommitteeRepository(_connectionString);
            CountryRepository = new CountryRepository(_connectionString);
            ICJRepository = new ICJRepository(_connectionString);
        }

        // ============================================================
        //  COUNTRIES (ALL)
        // ============================================================

        /// <summary>
        /// Loads all countries from the Countries table.
        /// Returns English, French and Spanish names for each ISO code.
        /// </summary>
        public async Task<List<Country>> LoadAllCountriesAsync()
        {
            var list = new List<Country>();

            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            string sql = "SELECT IsoCode, EnglishName, FrenchName, SpanishName FROM Countries";

            using var cmd = new SqliteCommand(sql, connection);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new Country
                {
                    IsoCode = reader.GetString(0),
                    EnglishName = reader.GetString(1),
                    FrenchName = reader.GetString(2),
                    SpanishName = reader.GetString(3)
                });
            }

            return list;
        }

        // ============================================================
        //  COUNTRIES FOR NORMAL COMMITTEES
        // ============================================================

        /// <summary>
        /// Loads all countries assigned to a specific committee (by CommID).
        /// </summary>
        public async Task<List<Country>> LoadCountriesForCommitteeAsync(int commID)
        {
            var list = new List<Country>();

            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"
                SELECT c.IsoCode, c.EnglishName, c.FrenchName, c.SpanishName, cl.Warnings
                FROM CountryLists cl
                JOIN Countries c ON cl.IsoCode = c.IsoCode
                WHERE cl.CommID = @CommID
                ORDER BY c.EnglishName;
            ";

            using var cmd = new SqliteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@CommID", commID);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new Country
                {
                    IsoCode = reader.GetString(0),
                    EnglishName = reader.GetString(1),
                    FrenchName = reader.GetString(2),
                    SpanishName = reader.GetString(3),
                    Warnings = reader.IsDBNull(4) ? 0 : reader.GetInt32(4)
                });
            }

            return list;
        }

        /// <summary>
        /// Deletes all countries assigned to a specific committee.
        /// </summary>
        public async Task DeleteCountriesForCommitteeAsync(int commID)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            string sql = "DELETE FROM CountryLists WHERE CommID = @CommID";

            using var cmd = new SqliteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@CommID", commID);

            await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Inserts a country into CountryLists for a specific committee.
        /// </summary>
        public async Task InsertSelectedCountryAsync(int committeeId, string isoCode)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"
                INSERT INTO CountryLists (CommID, IsoCode, Warnings)
                VALUES (@CommID, @IsoCode, 0);
            ";

            using var cmd = new SqliteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@CommID", committeeId);
            cmd.Parameters.AddWithValue("@IsoCode", isoCode);

            await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Updates the Warnings count for a specific country in a specific committee.
        /// </summary>
        public async Task UpdateCountryWarningsAsync(int committeeId, string isoCode, int warnings)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"
                UPDATE CountryLists
                SET Warnings = @Warnings
                WHERE CommID = @CommID AND IsoCode = @IsoCode;
            ";

            using var cmd = new SqliteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@CommID", committeeId);
            cmd.Parameters.AddWithValue("@IsoCode", isoCode);
            cmd.Parameters.AddWithValue("@Warnings", warnings);

            await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Loads all countries with warnings > 0 for a specific committee.
        /// </summary>
        public async Task<List<Country>> LoadCountriesWithWarningsAsync(int commID)
        {
            var list = new List<Country>();

            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"
                SELECT c.IsoCode, c.EnglishName, c.FrenchName, c.SpanishName, cl.Warnings
                FROM CountryLists cl
                JOIN Countries c ON cl.IsoCode = c.IsoCode
                WHERE cl.CommID = @CommID AND cl.Warnings > 0
                ORDER BY c.EnglishName;
            ";

            using var cmd = new SqliteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@CommID", commID);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new Country
                {
                    IsoCode = reader.GetString(0),
                    EnglishName = reader.GetString(1),
                    FrenchName = reader.GetString(2),
                    SpanishName = reader.GetString(3),
                    Warnings = reader.GetInt32(4)
                });
            }

            return list;
        }

        // ============================================================
        //  COMMITTEES (NORMAL)
        // ============================================================

        /// <summary>
        /// Loads all committees from the Committees table.
        /// Also adds ICJ with CommID = -1 if the ICJ table contains data.
        /// </summary>
        public async Task<List<Committee>> GetCommitteesAsync()
        {
            var list = new List<Committee>();

            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            // Load regular committees (excluding ICJ if it exists in Committees table)
            string sql = "SELECT * FROM Committees WHERE Name != 'ICJ' ORDER BY Name";

            using var cmd = new SqliteCommand(sql, connection);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new Committee
                {
                    CommID = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Topic = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    President = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    VicePresident = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    Moderator = reader.IsDBNull(5) ? "" : reader.GetString(5)
                });
            }

            // Add ICJ with CommID = -1 if ICJ table has data
            if (ICJRepository.HasData())
            {
                list.Add(new Committee
                {
                    CommID = -1,
                    Name = "ICJ",
                    Topic = "",
                    President = "",
                    VicePresident = "",
                    Moderator = ""
                });
            }

            return list;
        }

        /// <summary>
        /// Loads a committee by name from the Committees table.
        /// </summary>
        public async Task<Committee?> GetCommitteeByNameAsync(string name)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            string sql = "SELECT * FROM Committees WHERE Name = @Name";

            using var cmd = new SqliteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Name", name);

            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Committee
                {
                    CommID = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Topic = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    President = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    VicePresident = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    Moderator = reader.IsDBNull(5) ? "" : reader.GetString(5)
                };
            }

            return null;
        }

        /// <summary>
        /// Inserts a new committee and returns it with the generated CommID.
        /// </summary>
        public async Task<Committee> AddCommitteeAsync(Committee committee)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"
                INSERT INTO Committees (Name, Topic, President, VicePresident, Moderator)
                VALUES (@Name, @Topic, @President, @VicePresident, @Moderator);
                SELECT last_insert_rowid();
            ";

            using var cmd = new SqliteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Name", committee.Name);
            cmd.Parameters.AddWithValue("@Topic", committee.Topic);
            cmd.Parameters.AddWithValue("@President", committee.President);
            cmd.Parameters.AddWithValue("@VicePresident", committee.VicePresident);
            cmd.Parameters.AddWithValue("@Moderator", committee.Moderator);

            var result = await cmd.ExecuteScalarAsync();

            if (result is long newId)
                committee.CommID = (int)newId;
            else
                throw new Exception("Unable to retrieve new committee ID.");

            return committee;
        }

        /// <summary>
        /// Updates an existing committee.
        /// </summary>
        public async Task UpdateCommitteeAsync(Committee committee)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"
                UPDATE Committees
                SET Name = @Name,
                    Topic = @Topic,
                    President = @President,
                    VicePresident = @VicePresident,
                    Moderator = @Moderator
                WHERE CommID = @CommID;
            ";

            using var cmd = new SqliteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@CommID", committee.CommID);
            cmd.Parameters.AddWithValue("@Name", committee.Name);
            cmd.Parameters.AddWithValue("@Topic", committee.Topic);
            cmd.Parameters.AddWithValue("@President", committee.President);
            cmd.Parameters.AddWithValue("@VicePresident", committee.VicePresident);
            cmd.Parameters.AddWithValue("@Moderator", committee.Moderator);

            await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Removes a committee and all its assigned countries.
        /// If the committee is ICJ (CommID = -1), deletes all data from the ICJ table instead.
        /// </summary>
        public async Task RemoveCommitteeAsync(Committee committee)
        {
            // Special case: ICJ (CommID = -1)
            if (committee.CommID == -1)
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync();

                string sql = "DELETE FROM ICJ";
                using var cmd = new SqliteCommand(sql, connection);
                await cmd.ExecuteNonQueryAsync();
                return;
            }

            // Regular committee
            using var conn = new SqliteConnection(_connectionString);
            await conn.OpenAsync();

            string sqlCountries = "DELETE FROM CountryLists WHERE CommID = @CommID";
            using (var cmd = new SqliteCommand(sqlCountries, conn))
            {
                cmd.Parameters.AddWithValue("@CommID", committee.CommID);
                await cmd.ExecuteNonQueryAsync();
            }

            string sqlCommittee = "DELETE FROM Committees WHERE CommID = @CommID";
            using (var cmd = new SqliteCommand(sqlCommittee, conn))
            {
                cmd.Parameters.AddWithValue("@CommID", committee.CommID);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        // ============================================================
        //  ICJ — MAIN INFO (Topic, President, Moderator)
        // ============================================================

        /// <summary>
        /// Loads the single ICJ record (Topic, President, Moderator).
        /// </summary>
        public async Task<Committee?> LoadICJAsync()
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"
                    SELECT CommID, Name, Topic, President, VicePresident, Moderator
                    FROM Committees
                    WHERE Name = 'ICJ';
                ";

            using var cmd = new SqliteCommand(sql, connection);
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Committee
                {
                    CommID = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Topic = reader.GetString(2),
                    President = reader.GetString(3),
                    VicePresident = reader.GetString(4),
                    Moderator = reader.GetString(5)
                };
            }

            return null;
        }

        /// <summary>
        /// Saves ICJ main info (Topic, President, Moderator).
        /// Replaces the single row in the ICJ table.
        /// </summary>
        public async Task SaveICJAsync(Committee icj)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"
                    UPDATE Committees
                    SET Topic = @Topic,
                        President = @President,
                        VicePresident = @VicePresident,
                        Moderator = @Moderator
                    WHERE Name = 'ICJ';
                ";

            using var cmd = new SqliteCommand(sql, connection);

            cmd.Parameters.AddWithValue("@Topic", icj.Topic);
            cmd.Parameters.AddWithValue("@President", icj.President);
            cmd.Parameters.AddWithValue("@VicePresident", icj.VicePresident);
            cmd.Parameters.AddWithValue("@Moderator", icj.Moderator);

            await cmd.ExecuteNonQueryAsync();
        }
        // ============================================================
        //  ICJ — MEMBERS
        // ============================================================

        /// <summary>
        /// Loads all ICJ members (Jurors and Lawyers).
        /// </summary>
        public async Task<List<ICJMember>> LoadICJMembersAsync()
        {
            var list = new List<ICJMember>();

            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            string sql = "SELECT MemberID, Name, IsoCode, Kind, Warnings FROM ICJMembers ORDER BY Name";

            using var cmd = new SqliteCommand(sql, connection);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new ICJMember
                {
                    MemberID = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    IsoCode = reader.GetString(2),
                    Kind = reader.GetString(3),
                    Warnings = reader.GetInt32(4)
                });
            }

            return list;
        }

        /// <summary>
        /// Inserts a new ICJ member (Juror or Lawyer).
        /// </summary>
        public async Task InsertICJMemberAsync(string name, string kind, string isoCode = "IC")
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"
                INSERT INTO ICJMembers (Name, IsoCode, Kind, Warnings)
                VALUES (@Name, @IsoCode, @Kind, 0);
            ";

            using var cmd = new SqliteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Name", name);
            cmd.Parameters.AddWithValue("@IsoCode", isoCode);
            cmd.Parameters.AddWithValue("@Kind", kind);

            await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Deletes an ICJ member by MemberID.
        /// </summary>
        public async Task DeleteICJMemberAsync(int memberId)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            string sql = "DELETE FROM ICJMembers WHERE MemberID = @ID";

            using var cmd = new SqliteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@ID", memberId);

            await cmd.ExecuteNonQueryAsync();
        }

        // ============================================================
        //  ICJ — COUNTRIES (CommID = -1)
        // ============================================================

        /// <summary>
        /// Loads all countries assigned to ICJ (stored with CommID = -1).
        /// </summary>
        public async Task<List<Country>> LoadICJCountriesAsync()
        {
            var list = new List<Country>();

            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"
                    SELECT c.IsoCode, c.EnglishName, c.FrenchName, c.SpanishName, cl.Warnings
                    FROM CountryLists cl
                    JOIN Countries c ON cl.IsoCode = c.IsoCode
                    WHERE cl.CommID = (SELECT CommID FROM Committees WHERE Name = 'ICJ')
                    ORDER BY c.EnglishName;
                ";

            using var cmd = new SqliteCommand(sql, connection);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new Country
                {
                    IsoCode = reader.GetString(0),
                    EnglishName = reader.GetString(1),
                    FrenchName = reader.GetString(2),
                    SpanishName = reader.GetString(3),
                    Warnings = reader.IsDBNull(4) ? 0 : reader.GetInt32(4)
                });
            }

            return list;
        }
        /// <summary>
        /// Saves the list of countries assigned to ICJ.
        /// Replaces all entries with CommID = -1.
        /// </summary>
        public async Task SaveICJCountriesAsync(IEnumerable<Country> countries)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            // ⭐ Cancella tutte le righe associate all'ICJ
            string sqlDelete = @"
                    DELETE FROM CountryLists
                    WHERE CommID = (SELECT CommID FROM Committees WHERE Name = 'ICJ');
                ";

            using (var cmd = new SqliteCommand(sqlDelete, connection))
                await cmd.ExecuteNonQueryAsync();

            // ⭐ Inserisci le nuove righe
            foreach (var c in countries)
            {
                string sqlInsert = @"
                        INSERT INTO CountryLists (CommID, IsoCode, Warnings)
                        VALUES ((SELECT CommID FROM Committees WHERE Name = 'ICJ'), @IsoCode, 0);
                    ";

                using var cmd = new SqliteCommand(sqlInsert, connection);
                cmd.Parameters.AddWithValue("@IsoCode", c.IsoCode);
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}