using Foscamun2026.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace Foscamun2026.Data
{
    public class SqliteDataAccess
    {
        private readonly string _connectionString;

        public SqliteDataAccess()
        {
            // Percorso sicuro e universale
            string folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Foscamun2026"
            );

            Directory.CreateDirectory(folder);

            string dbPath = Path.Combine(folder, "Foscamun.db");

            _connectionString = $"Data Source={dbPath}";
        }

        public async Task<List<Country>> LoadAllCountriesAsync()
        {
            var list = new List<Country>();

            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            string sql = "SELECT IsoCode, Name FROM Countries ORDER BY Name";

            using var cmd = new SqliteCommand(sql, connection);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new Country
                {
                    IsoCode = reader.GetString(0),
                    EnglishName = reader.GetString(1)
                });
            }

            return list;
        }

        // GET
        public async Task<List<Committee>> GetCommitteesAsync()
        {
            var list = new List<Committee>();

            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            string sql = "SELECT * FROM Committees ORDER BY Name";

            using var cmd = new SqliteCommand(sql, connection);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new Committee
                {
                    CommID = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    TopicA = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    TopicB = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    President = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    VicePresident = reader.IsDBNull(5) ? "" : reader.GetString(5),
                    Moderator = reader.IsDBNull(6) ? "" : reader.GetString(6)
                });
            }

            return list;
        }

        // ADD
        public async Task<Committee> AddCommitteeAsync(Committee committee)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"
                        INSERT INTO Committees (Name, TopicA, TopicB, President, VicePresident, Moderator)
                        VALUES (@Name, @TopicA, @TopicB, @President, @VicePresident, @Moderator);
                        SELECT last_insert_rowid();
                    ";

            using var cmd = new SqliteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Name", committee.Name);
            cmd.Parameters.AddWithValue("@TopicA", committee.TopicA);
            cmd.Parameters.AddWithValue("@TopicB", committee.TopicB);
            cmd.Parameters.AddWithValue("@President", committee.President);
            cmd.Parameters.AddWithValue("@VicePresident", committee.VicePresident);
            cmd.Parameters.AddWithValue("@Moderator", committee.Moderator);

            var result = await cmd.ExecuteScalarAsync();

            if (result is long newId)
            {
                committee.CommID = (int)newId;
            }
            else
            {
                throw new Exception("Errore: impossibile ottenere l'ID del nuovo comitato.");
            }
            committee.CommID = (int)newId;

            return committee;
        }

        public async Task InsertSelectedCountryAsync(int committeeId, string isoCode)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"INSERT INTO CommitteeCountries (CommitteeId, CountryIso)
                            VALUES (@CommitteeId, @IsoCode);";

            using var cmd = new SqliteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@CommitteeId", committeeId);
            cmd.Parameters.AddWithValue("@IsoCode", isoCode);

            await cmd.ExecuteNonQueryAsync();
        }

        // UPDATE
        public async Task UpdateCommitteeAsync(Committee committee)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"
                UPDATE Committees
                SET Name = @Name,
                    TopicA = @TopicA,
                    TopicB = @TopicB,
                    President = @President,
                    VicePresident = @VicePresident,
                    Moderator = @Moderator
                WHERE CommID = @CommID;
            ";

            using var cmd = new SqliteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@CommID", committee.CommID);
            cmd.Parameters.AddWithValue("@Name", committee.Name);
            cmd.Parameters.AddWithValue("@TopicA", committee.TopicA);
            cmd.Parameters.AddWithValue("@TopicB", committee.TopicB);
            cmd.Parameters.AddWithValue("@President", committee.President);
            cmd.Parameters.AddWithValue("@VicePresident", committee.VicePresident);
            cmd.Parameters.AddWithValue("@Moderator", committee.Moderator);

            await cmd.ExecuteNonQueryAsync();
        }

        // REMOVE
        public async Task RemoveCommitteeAsync(Committee committee)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            string sql = "DELETE FROM Committees WHERE CommID = @CommID";

            using var cmd = new SqliteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@CommID", committee.CommID);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}