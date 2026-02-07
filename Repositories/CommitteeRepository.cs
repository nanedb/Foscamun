using Microsoft.Data.Sqlite;
using Foscamun2026.Models;

namespace Foscamun2026.Repositories
{
    public class CommitteeRepository
    {
        private readonly string _connectionString;

        public CommitteeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Committee> GetAll()
        {
            var list = new List<Committee>();

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Name, TopicA, TopicB, President, VicePresident, Moderator FROM Committees";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Committee
                {
                    CommID = reader.GetInt32(0),
                    Name = reader["Name"] as string ?? "",
                    TopicA = reader["TopicA"] as string ?? "",
                    TopicB = reader["TopicB"] as string ?? "",
                    President = reader["President"] as string ?? "",
                    VicePresident = reader["VicePresident"] as string ?? "",
                    Moderator = reader["Moderator"] as string ?? ""
                });
            }

            return list;
        }

        public void Save(Committee c)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            using var cmd = conn.CreateCommand();

            if (c.CommID == 0)
            {
                cmd.CommandText = @"
                    INSERT INTO Committees (Name, TopicA, TopicB, President, VicePresident, Moderator)
                    VALUES (@Name, @TopicA, @TopicB, @President, @VicePresident, @Moderator)";
            }
            else
            {
                cmd.CommandText = @"
                    UPDATE Committees
                    SET Name=@Name, TopicA=@TopicA, TopicB=@TopicB,
                        President=@President, VicePresident=@VicePresident, Moderator=@Moderator
                    WHERE Id=@Id";
                cmd.Parameters.AddWithValue("@Id", c.CommID);
            }

            cmd.Parameters.AddWithValue("@Name", c.Name);
            cmd.Parameters.AddWithValue("@TopicA", c.TopicA);
            cmd.Parameters.AddWithValue("@TopicB", c.TopicB);
            cmd.Parameters.AddWithValue("@President", c.President);
            cmd.Parameters.AddWithValue("@VicePresident", c.VicePresident);
            cmd.Parameters.AddWithValue("@Moderator", c.Moderator);

            cmd.ExecuteNonQuery();
        }
    }
}