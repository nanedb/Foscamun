using Microsoft.Data.Sqlite;
using Foscamun.Models;

namespace Foscamun.Repositories
{
    /// <summary>
    /// Repository for managing country data in the database.
    /// </summary>
    public class CountryRepository
    {
        private readonly string _connectionString;

        public CountryRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Retrieves all countries from the database, ordered by English name.
        /// </summary>
        public List<Country> GetAll()
        {
            var list = new List<Country>();

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT IsoCode, EnglishName, FrenchName, SpanishName
                FROM Countries
                ORDER BY EnglishName";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Country
                {
                    IsoCode = reader["IsoCode"] as string ?? "",
                    EnglishName = reader["EnglishName"] as string ?? "",
                    FrenchName = reader["FrenchName"] as string ?? "",
                    SpanishName = reader["SpanishName"] as string ?? ""
                });
            }

            return list;
        }
    }
}
