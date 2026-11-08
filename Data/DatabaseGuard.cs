using Foscamun.Views;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Windows;

namespace Foscamun.Data
{
    /// <summary>
    /// Keeps the user's database across installations: it is used as it is unless it cannot work
    /// with this version of the app (written by a newer version, or not a Foscamun database).
    /// The schema version lives in SQLite's <c>user_version</c>.
    /// </summary>
    public static class DatabaseGuard
    {
        /// <summary>
        /// Schema version written by this app. Raise it when the schema changes, and teach
        /// <see cref="SqliteDataAccess"/> to bring older databases up to date.
        /// 1 = committees, countries, ICJ with any number of advocates, ICJ member warnings.
        /// </summary>
        public const int CurrentVersion = 1;

        // Tables that every Foscamun database has had since the first release
        private static readonly string[] RequiredTables = { "Committees", "Countries", "CountryLists", "ICJ" };

        public static string DbPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Foscamun", "Foscamun.db");

        private enum State { Missing, Usable, TooNew, Invalid }

        /// <summary>
        /// Checks the existing database before the app opens it. If it cannot be used, warns and
        /// offers to set it aside (renamed, never deleted) and start with a new one.
        /// False if the user chose to quit.
        /// </summary>
        public static bool EnsureCompatible()
        {
            var (state, version) = Inspect();
            if (state is State.Missing or State.Usable)
                return true;

            string message = state == State.TooNew
                ? AppDialog.Text("DbTooNewMsg", version, CurrentVersion)
                : AppDialog.Text("DbInvalidMsg");

            if (AppDialog.Show(message + "\n\n" + AppDialog.Text("DbResetQuestion", DbPath),
                               AppDialog.Text("DbIncompatibleTitle"),
                               MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return false;

            // The old file stays next to the new one, so nothing is lost
            SqliteConnection.ClearAllPools();
            string backup = Path.ChangeExtension(DbPath, null) + $"-backup-{DateTime.Now:yyyyMMdd-HHmmss}.db";
            File.Move(DbPath, backup);
            AppDialog.Show(AppDialog.Text("DbBackupDoneMsg", backup), AppDialog.Text("DbIncompatibleTitle"));
            return true;
        }

        /// <summary>
        /// Marks the database with this app's schema version, once <see cref="SqliteDataAccess"/>
        /// has brought it up to date. A newer version is never lowered.
        /// </summary>
        public static void StampVersion()
        {
            using var connection = new SqliteConnection($"Data Source={DbPath}");
            connection.Open();
            if (ReadVersion(connection) < CurrentVersion)
            {
                using var cmd = new SqliteCommand($"PRAGMA user_version = {CurrentVersion};", connection);
                cmd.ExecuteNonQuery();
            }
        }

        private static (State, int) Inspect()
        {
            if (!File.Exists(DbPath))
                return (State.Missing, 0);

            try
            {
                using var connection = new SqliteConnection($"Data Source={DbPath};Mode=ReadOnly;Pooling=False");
                connection.Open();

                int version = ReadVersion(connection);
                if (version > CurrentVersion)
                    return (State.TooNew, version);

                var tables = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                using (var cmd = new SqliteCommand("SELECT name FROM sqlite_master WHERE type = 'table';", connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        tables.Add(reader.GetString(0));
                }

                return RequiredTables.All(tables.Contains) ? (State.Usable, version) : (State.Invalid, version);
            }
            catch (SqliteException)
            {
                // Not a database at all, or damaged
                return (State.Invalid, 0);
            }
        }

        private static int ReadVersion(SqliteConnection connection)
        {
            using var cmd = new SqliteCommand("PRAGMA user_version;", connection);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }
    }
}
