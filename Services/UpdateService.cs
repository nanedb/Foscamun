using Foscamun.Properties;
using Foscamun.Views;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace Foscamun.Services
{
    /// <summary>
    /// Looks for newer releases on GitHub at startup and, if the user agrees, downloads the ZIP
    /// for this architecture, checks it against SHA256SUMS.txt, and replaces the app's files once
    /// the app has closed. The database and the logos in %LocalAppData% are not touched.
    /// </summary>
    public static class UpdateService
    {
        private const string ReleasesApi = "https://api.github.com/repos/nanedb/Foscamun/releases?per_page=30";

        private record Release(Version Version, string Tag, string PageUrl, string ZipName, string ZipUrl, string? SumsUrl);

        private static readonly HttpClient Http = CreateClient();

        private static HttpClient CreateClient()
        {
            var client = new HttpClient();
            // GitHub's API wants a User-Agent: the app's name is enough
            client.DefaultRequestHeaders.UserAgent.ParseAdd($"Foscamun/{CurrentVersion}");
            return client;
        }

        public static Version CurrentVersion
        {
            get
            {
                var v = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(0, 0, 0);
                return new Version(v.Major, v.Minor, Math.Max(v.Build, 0));
            }
        }

        /// <summary>
        /// Checks for a newer version. Without a connection, or if GitHub does not answer,
        /// nothing is shown: the check is retried at the next start.
        /// </summary>
        public static async Task CheckAtStartupAsync()
        {
            Release? release;
            try
            {
                using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                release = await FindNewerReleaseAsync(timeout.Token);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Update check failed: {ex.Message}");
                return;
            }

            if (release == null || Settings.Default.SkippedUpdate == release.Tag)
                return;

            var answer = AppDialog.Show(
                AppDialog.Text("UpdateAvailableMsg", release.Version.ToString(3), CurrentVersion.ToString(3)),
                AppDialog.Text("UpdateAvailableTitle"),
                MessageBoxButton.YesNo, MessageBoxImage.Information,
                AppDialog.Text("UpdateSkipVersion"), out bool skip);

            if (answer != MessageBoxResult.Yes)
            {
                if (skip)
                {
                    Settings.Default.SkippedUpdate = release.Tag;
                    Settings.Default.Save();
                }
                return;
            }

            await DownloadAndInstallAsync(release);
        }

        /// <summary>The newest release above this version that has a ZIP for this architecture, or null.</summary>
        private static async Task<Release?> FindNewerReleaseAsync(CancellationToken token)
        {
            using var response = await Http.GetAsync(ReleasesApi, token);
            response.EnsureSuccessStatusCode();
            using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync(token));

            string rid = RuntimeId();
            Release? best = null;

            foreach (var item in json.RootElement.EnumerateArray())
            {
                if (item.GetProperty("draft").GetBoolean())
                    continue;

                string tag = item.GetProperty("tag_name").GetString() ?? "";
                if (!Version.TryParse(tag.TrimStart('v', 'V'), out var version) || version <= CurrentVersion)
                    continue;
                if (best != null && version <= best.Version)
                    continue;

                string zipName = $"Foscamun-{rid}-{tag}.zip";
                string? zipUrl = null, sumsUrl = null;
                foreach (var asset in item.GetProperty("assets").EnumerateArray())
                {
                    string name = asset.GetProperty("name").GetString() ?? "";
                    string url = asset.GetProperty("browser_download_url").GetString() ?? "";
                    if (name.Equals(zipName, StringComparison.OrdinalIgnoreCase)) zipUrl = url;
                    else if (name.Equals("SHA256SUMS.txt", StringComparison.OrdinalIgnoreCase)) sumsUrl = url;
                }

                if (zipUrl != null)
                    best = new Release(version, tag, item.GetProperty("html_url").GetString() ?? "", zipName, zipUrl, sumsUrl);
            }

            return best;
        }

        private static string RuntimeId() => RuntimeInformation.ProcessArchitecture switch
        {
            Architecture.X86 => "win-x86",
            Architecture.Arm64 => "win-arm64",
            _ => "win-x64"
        };

        private static async Task DownloadAndInstallAsync(Release release)
        {
            string appFolder = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);

            // The new files go where the app is: without write access, the user installs by hand
            if (!CanWriteTo(appFolder))
            {
                OfferManualDownload(release, AppDialog.Text("UpdateNoAccessMsg", appFolder));
                return;
            }

            string work = Path.Combine(Path.GetTempPath(), "Foscamun-update");
            string zipPath = Path.Combine(work, release.ZipName);
            string filesPath = Path.Combine(work, "files");

            var progress = new UpdateProgressWindow(release.Version.ToString(3));
            progress.Show();
            try
            {
                if (Directory.Exists(work))
                    Directory.Delete(work, recursive: true);
                Directory.CreateDirectory(work);

                await DownloadAsync(release.ZipUrl, zipPath, progress);

                progress.SetStatus(AppDialog.Text("UpdateVerifying"));
                if (release.SumsUrl != null)
                    await VerifyAsync(zipPath, release, progress.Token);

                ZipFile.ExtractToDirectory(zipPath, filesPath);
                if (!File.Exists(Path.Combine(filesPath, "Foscamun.exe")))
                    throw new InvalidDataException("Foscamun.exe not found in the downloaded package.");
            }
            catch (OperationCanceledException)
            {
                progress.Close();
                return;
            }
            catch (Exception ex)
            {
                progress.Close();
                OfferManualDownload(release, AppDialog.Text("UpdateFailedMsg", ex.Message));
                return;
            }

            progress.Close();
            AppDialog.Show(AppDialog.Text("UpdateReadyMsg"), AppDialog.Text("UpdateAvailableTitle"));

            StartInstaller(work, filesPath, appFolder);
            MainWindow.CloseForUpdate();
        }

        private static async Task DownloadAsync(string url, string path, UpdateProgressWindow progress)
        {
            using var response = await Http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, progress.Token);
            response.EnsureSuccessStatusCode();
            long? total = response.Content.Headers.ContentLength;

            await using var source = await response.Content.ReadAsStreamAsync(progress.Token);
            await using var target = File.Create(path);
            var buffer = new byte[81920];
            long done = 0;
            int read;
            while ((read = await source.ReadAsync(buffer, progress.Token)) > 0)
            {
                await target.WriteAsync(buffer.AsMemory(0, read), progress.Token);
                done += read;
                progress.Report(done, total);
            }
        }

        // The ZIP must match the checksum published with the release
        private static async Task VerifyAsync(string zipPath, Release release, CancellationToken token)
        {
            string sums = await Http.GetStringAsync(release.SumsUrl, token);
            string? expected = sums.Split('\n')
                .Select(line => line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries))
                .FirstOrDefault(parts => parts.Length == 2 && parts[1].Equals(release.ZipName, StringComparison.OrdinalIgnoreCase))?[0];
            if (expected == null)
                return; // no line for this file: nothing to compare with

            await using var stream = File.OpenRead(zipPath);
            string actual = Convert.ToHexString(await SHA256.HashDataAsync(stream, token));
            if (!actual.Equals(expected, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException(AppDialog.Text("UpdateChecksumMsg"));
        }

        /// <summary>
        /// A small script waits for the app to close, copies the new files over the old ones,
        /// starts the new version and removes the downloaded files.
        /// </summary>
        private static void StartInstaller(string work, string filesPath, string appFolder)
        {
            string script = Path.Combine(Path.GetTempPath(), "Foscamun-update.cmd");
            string exe = Path.Combine(appFolder, "Foscamun.exe");
            var lines = new[]
            {
                "@echo off",
                "chcp 65001 >nul", // the paths below are UTF-8
                ":wait",
                $"tasklist /fi \"PID eq {Environment.ProcessId}\" | find \"{Environment.ProcessId}\" >nul && (timeout /t 1 /nobreak >nul & goto wait)",
                $"robocopy \"{filesPath}\" \"{appFolder}\" /e /r:10 /w:1 /njh /njs /nfl /ndl >nul",
                $"start \"\" \"{exe}\"",
                $"rmdir /s /q \"{work}\"",
                "(goto) 2>nul & del \"%~f0\""
            };
            File.WriteAllLines(script, lines, new UTF8Encoding(false));

            Process.Start(new ProcessStartInfo("cmd.exe", $"/c \"{script}\"")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                WorkingDirectory = Path.GetTempPath()
            });
        }

        private static bool CanWriteTo(string folder)
        {
            try
            {
                string probe = Path.Combine(folder, $".write-test-{Guid.NewGuid():N}");
                File.WriteAllText(probe, "");
                File.Delete(probe);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void OfferManualDownload(Release release, string reason)
        {
            if (AppDialog.Show(reason + "\n\n" + AppDialog.Text("UpdateOpenPageQuestion"),
                               AppDialog.Text("UpdateAvailableTitle"),
                               MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Process.Start(new ProcessStartInfo(release.PageUrl) { UseShellExecute = true });
            }
        }
    }
}
