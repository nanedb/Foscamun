using Microsoft.Data.Sqlite;
using System.Text.Json;
using Foscamun.Models;

namespace Foscamun.Repositories
{
    /// <summary>
    /// Repository for managing ICJ (International Court of Justice) data in the database.
    /// </summary>
    public class ICJRepository
    {
        private readonly string _connectionString;

        public ICJRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Loads the ICJ configuration from the database.
        /// Returns a new empty ICJ instance if no data exists.
        /// </summary>
        public ICJ Load()
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM ICJ LIMIT 1";

            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
                return new ICJ();

            // Safe string reading helper
            string GetString(string column) =>
                reader[column] as string ?? "";

            // Safe jurors list reading from JSON
            List<string> GetJurors()
            {
                var json = reader["JurorsJson"] as string ?? "[]";
                return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
            }

            return new ICJ
            {
                Judge = GetString("Judge"),
                ViceJudge1 = GetString("ViceJudge1"),
                ViceJudge2 = GetString("ViceJudge2"),
                Topic = GetString("Topic"),

                Plaintiff1 = GetString("Plaintiff1"),
                Plaintiff2 = GetString("Plaintiff2"),
                Plaintiff3 = GetString("Plaintiff3"),

                PCountry = new Country { IsoCode = GetString("PCountryIso") },

                Defense1 = GetString("Defense1"),
                Defense2 = GetString("Defense2"),
                Defense3 = GetString("Defense3"),

                DCountry = new Country { IsoCode = GetString("DCountryIso") },

                Jurors = GetJurors()
            };
        }

        /// <summary>
        /// Saves the ICJ configuration to the database.
        /// Deletes any existing ICJ data and inserts the new configuration.
        /// </summary>
        public void Save(ICJ icj)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                DELETE FROM ICJ;

                INSERT INTO ICJ (
                    Judge, ViceJudge1, ViceJudge2, Topic,
                    Plaintiff1, Plaintiff2, Plaintiff3, PCountryIso,
                    Defense1, Defense2, Defense3, DCountryIso,
                    JurorsJson
                ) VALUES (
                    @Judge, @ViceJudge1, @ViceJudge2, @Topic,
                    @Plaintiff1, @Plaintiff2, @Plaintiff3, @PCountryIso,
                    @Defense1, @Defense2, @Defense3, @DCountryIso,
                    @JurorsJson
                );
            ";

            cmd.Parameters.AddWithValue("@Judge", icj.Judge);
            cmd.Parameters.AddWithValue("@ViceJudge1", icj.ViceJudge1);
            cmd.Parameters.AddWithValue("@ViceJudge2", icj.ViceJudge2);
            cmd.Parameters.AddWithValue("@Topic", icj.Topic);

            cmd.Parameters.AddWithValue("@Plaintiff1", icj.Plaintiff1);
            cmd.Parameters.AddWithValue("@Plaintiff2", icj.Plaintiff2);
            cmd.Parameters.AddWithValue("@Plaintiff3", icj.Plaintiff3);
            cmd.Parameters.AddWithValue("@PCountryIso", icj.PCountry?.IsoCode);

            cmd.Parameters.AddWithValue("@Defense1", icj.Defense1);
            cmd.Parameters.AddWithValue("@Defense2", icj.Defense2);
            cmd.Parameters.AddWithValue("@Defense3", icj.Defense3);
            cmd.Parameters.AddWithValue("@DCountryIso", icj.DCountry?.IsoCode);

            cmd.Parameters.AddWithValue("@JurorsJson", JsonSerializer.Serialize(icj.Jurors));

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Checks if ICJ data exists in the database.
        /// </summary>
        public bool HasData()
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM ICJ";

            var count = (long)(cmd.ExecuteScalar() ?? 0);
            return count > 0;
        }
    }
}
