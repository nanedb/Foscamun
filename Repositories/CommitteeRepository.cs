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
            cmd.CommandText = "SELECT Id, Name, Topic, President, VicePresident, Moderator FROM Committees";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Committee
                {
                    CommID = reader.GetInt32(0),
                    Name = reader["Name"] as string ?? "",
                    Topic = reader["Topic"] as string ?? "",
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
                    INSERT INTO Committees (Name, Topic, President, VicePresident, Moderator)
                    VALUES (@Name, @Topic, @President, @VicePresident, @Moderator)";
            }
            else
            {
                cmd.CommandText = @"
                    UPDATE Committees
                    SET Name=@Name, Topic=@Topic,
                        President=@President, VicePresident=@VicePresident, Moderator=@Moderator
                    WHERE Id=@Id";
                cmd.Parameters.AddWithValue("@Id", c.CommID);
            }

            cmd.Parameters.AddWithValue("@Name", c.Name);
            cmd.Parameters.AddWithValue("@Topic", c.Topic);
            cmd.Parameters.AddWithValue("@President", c.President);
            cmd.Parameters.AddWithValue("@VicePresident", c.VicePresident);
            cmd.Parameters.AddWithValue("@Moderator", c.Moderator);

            cmd.ExecuteNonQuery();
        }
    }
}