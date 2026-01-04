using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Foscamun2026.Models;

namespace Foscamun2026.Data
{
    public class SqliteDataAccess
    {
        private readonly string _connectionString =
            $"Data Source={Properties.Settings.Default.DbPath};Cache=Shared";


        // ---------------------------------------------------------
        // GET ALL COMMITTEES
        // ---------------------------------------------------------
        public async Task<List<Committee>> GetCommitteesAsync()
        {
            var committees = new List<Committee>();

            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            string sql = "SELECT CommID, Name, TopicA, TopicB, President, VicePresident, Moderator FROM Committees";

            using var command = new SqliteCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                committees.Add(new Committee
                {
                    CommID = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    TopicA = reader.GetString(2),
                    TopicB = reader.GetString(3),
                    President = reader.GetString(4),
                    VicePresident = reader.GetString(5),
                    Moderator = reader.GetString(6)
                });
            }

            return committees;
        }

        // ---------------------------------------------------------
        // ADD COMMITTEE
        // ---------------------------------------------------------
        public async Task AddCommitteeAsync(Committee committee)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"
                INSERT INTO Committees (Name, TopicA, TopicB, President, VicePresident, Moderator)
                VALUES (@Name, @TopicA, @TopicB, @President, @VicePresident, @Moderator)";

            using var command = new SqliteCommand(sql, connection);

            command.Parameters.AddWithValue("@Name", committee.Name);
            command.Parameters.AddWithValue("@TopicA", committee.TopicA);
            command.Parameters.AddWithValue("@TopicB", committee.TopicB);
            command.Parameters.AddWithValue("@President", committee.President);
            command.Parameters.AddWithValue("@VicePresident", committee.VicePresident);
            command.Parameters.AddWithValue("@Moderator", committee.Moderator);

            await command.ExecuteNonQueryAsync();
        }

        // ---------------------------------------------------------
        // UPDATE COMMITTEE
        // ---------------------------------------------------------
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
                WHERE CommID = @CommID";

            using var command = new SqliteCommand(sql, connection);

            command.Parameters.AddWithValue("@Name", committee.Name);
            command.Parameters.AddWithValue("@TopicA", committee.TopicA);
            command.Parameters.AddWithValue("@TopicB", committee.TopicB);
            command.Parameters.AddWithValue("@President", committee.President);
            command.Parameters.AddWithValue("@VicePresident", committee.VicePresident);
            command.Parameters.AddWithValue("@Moderator", committee.Moderator);
            command.Parameters.AddWithValue("@CommID", committee.CommID);

            await command.ExecuteNonQueryAsync();
        }

        // ---------------------------------------------------------
        // REMOVE COMMITTEE
        // ---------------------------------------------------------
        public async Task RemoveCommitteeAsync(Committee committee)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            string sql = "DELETE FROM Committees WHERE CommID = @CommID";

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@CommID", committee.CommID);

            await command.ExecuteNonQueryAsync();
        }

        // ---------------------------------------------------------
        // GET COMMITTEE BY ID
        // ---------------------------------------------------------
        public async Task<Committee?> GetCommitteeByIdAsync(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            string sql = @"
                SELECT CommID, Name, TopicA, TopicB, President, VicePresident, Moderator
                FROM Committees
                WHERE CommID = @CommID";

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@CommID", id);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Committee
                {
                    CommID = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    TopicA = reader.GetString(2),
                    TopicB = reader.GetString(3),
                    President = reader.GetString(4),
                    VicePresident = reader.GetString(5),
                    Moderator = reader.GetString(6)
                };
            }

            return null;
        }
    }
}