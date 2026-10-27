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

            // Advocates list reading from JSON. Data saved by older versions only has
            // the fixed columns (three names sharing one country): convert those.
            List<ICJAdvocate> GetAdvocates(string jsonColumn, string namePrefix, string countryColumn)
            {
                if (reader[jsonColumn] is string json && !string.IsNullOrWhiteSpace(json))
                    return JsonSerializer.Deserialize<List<ICJAdvocate>>(json) ?? new List<ICJAdvocate>();

                string isoCode = GetString(countryColumn);
                return Enumerable.Range(1, 3)
                    .Select(i => GetString($"{namePrefix}{i}"))
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .Select(name => new ICJAdvocate { Name = name, IsoCode = isoCode })
                    .ToList();
            }

            return new ICJ
            {
                Judge = GetString("Judge"),
                ViceJudge1 = GetString("ViceJudge1"),
                ViceJudge2 = GetString("ViceJudge2"),
                Topic = GetString("Topic"),

                Plaintiffs = GetAdvocates("PlaintiffsJson", "Plaintiff", "PCountryIso"),
                Defense = GetAdvocates("DefenseJson", "Defense", "DCountryIso"),

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

            // The old fixed advocate columns (Plaintiff1-3, PCountryIso, Defense1-3,
            // DCountryIso) are NOT NULL: they are left empty, advocates live in the JSON columns.
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                DELETE FROM ICJ;

                INSERT INTO ICJ (
                    Judge, ViceJudge1, ViceJudge2, Topic,
                    Plaintiff1, Plaintiff2, Plaintiff3, PCountryIso,
                    Defense1, Defense2, Defense3, DCountryIso,
                    PlaintiffsJson, DefenseJson, JurorsJson
                ) VALUES (
                    @Judge, @ViceJudge1, @ViceJudge2, @Topic,
                    '', '', '', '',
                    '', '', '', '',
                    @PlaintiffsJson, @DefenseJson, @JurorsJson
                );
            ";

            cmd.Parameters.AddWithValue("@Judge", icj.Judge);
            cmd.Parameters.AddWithValue("@ViceJudge1", icj.ViceJudge1);
            cmd.Parameters.AddWithValue("@ViceJudge2", icj.ViceJudge2);
            cmd.Parameters.AddWithValue("@Topic", icj.Topic);

            cmd.Parameters.AddWithValue("@PlaintiffsJson", JsonSerializer.Serialize(icj.Plaintiffs));
            cmd.Parameters.AddWithValue("@DefenseJson", JsonSerializer.Serialize(icj.Defense));
            cmd.Parameters.AddWithValue("@JurorsJson", JsonSerializer.Serialize(icj.Jurors));

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Adds the PlaintiffsJson and DefenseJson columns to databases created by
        /// older versions, which only had three fixed advocates per side.
        /// </summary>
        public void EnsureAdvocatesColumnsExist()
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var columns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using (var info = conn.CreateCommand())
            {
                info.CommandText = "PRAGMA table_info(ICJ)";
                using var reader = info.ExecuteReader();
                while (reader.Read())
                    columns.Add(reader.GetString(1));
            }

            foreach (var column in new[] { "PlaintiffsJson", "DefenseJson" })
            {
                if (columns.Contains(column))
                    continue;

                using var alter = conn.CreateCommand();
                alter.CommandText = $"ALTER TABLE ICJ ADD COLUMN {column} TEXT";
                alter.ExecuteNonQuery();
            }
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
