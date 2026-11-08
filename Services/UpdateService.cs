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
    /// Looks for newer releases on GitHub at startup and, if the user agrees, downloads the new
    /// version for this architecture and checks it against SHA256SUMS.txt. An installed copy runs
    /// the new installer silently; a portable copy (from the ZIP) gets its files replaced. Both
    /// happen once the app has closed. The database and the logos in %LocalAppData% are not touched.
    /// </summary>
    public static class UpdateService
    {
        private const string ReleasesApi = "https://api.github.com/repos/nanedb/Foscamun/releases?per_page=30";

        /// <param name="AssetName">The installer (installed copy) or the ZIP (portable copy) to download.</param>
        private record Release(Version Version, string Tag, string PageUrl, string AssetName, string AssetUrl,
                               bool IsInstaller, string? SumsUrl);

        private static string AppFolder => AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);

        /// <summary>Installed with the installer (its uninstaller sits next to the exe), not extracted from a ZIP.</summary>
        private static bool IsInstalled => File.Exists(Path.Combine(AppFolder, "unins000.exe"));

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

        /// <summary>The newest release above this version with a package for this copy and architecture, or null.</summary>
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
                string setupName = $"Foscamun-Setup-{rid}-{tag}.exe";
                string? zipUrl = null, setupUrl = null, sumsUrl = null;
                foreach (var asset in item.GetProperty("assets").EnumerateArray())
                {
                    string name = asset.GetProperty("name").GetString() ?? "";
                    string url = asset.GetProperty("browser_download_url").GetString() ?? "";
                    if (name.Equals(zipName, StringComparison.OrdinalIgnoreCase)) zipUrl = url;
                    else if (name.Equals(setupName, StringComparison.OrdinalIgnoreCase)) setupUrl = url;
                    else if (name.Equals("SHA256SUMS.txt", StringComparison.OrdinalIgnoreCase)) sumsUrl = url;
                }

                // An installed copy updates with the installer (so that Windows keeps the right
                // version under Apps), a portable one with the ZIP; failing that, the other package
                string page = item.GetProperty("html_url").GetString() ?? "";
                if (setupUrl != null && (IsInstalled || zipUrl == null))
                    best = new Release(version, tag, page, setupName, setupUrl, true, sumsUrl);
                else if (zipUrl != null)
                    best = new Release(version, tag, page, zipName, zipUrl, false, sumsUrl);
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
            string appFolder = AppFolder;

            // The new files go where the app is: without write access, the user installs by hand
            if (!CanWriteTo(appFolder))
            {
                OfferManualDownload(release, AppDialog.Text("UpdateNoAccessMsg", appFolder));
                return;
            }

            string work = Path.Combine(Path.GetTempPath(), "Foscamun-update");
            string packagePath = Path.Combine(work, release.AssetName);
            string filesPath = Path.Combine(work, "files");

            var progress = new UpdateProgressWindow(release.Version.ToString(3));
            progress.Show();
            try
            {
                if (Directory.Exists(work))
                    Directory.Delete(work, recursive: true);
                Directory.CreateDirectory(work);

                await DownloadAsync(release.AssetUrl, packagePath, progress);

                progress.SetStatus(AppDialog.Text("UpdateVerifying"));
                if (release.SumsUrl != null)
                    await VerifyAsync(packagePath, release, progress.Token);

                if (!release.IsInstaller)
                {
                    ZipFile.ExtractToDirectory(packagePath, filesPath);
                    if (!File.Exists(Path.Combine(filesPath, "Foscamun.exe")))
                        throw new InvalidDataException("Foscamun.exe not found in the downloaded package.");
                }
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

            StartInstaller(work, release.IsInstaller ? packagePath : null, filesPath, appFolder);
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

        // The package must match the checksum published with the release
        private static async Task VerifyAsync(string packagePath, Release release, CancellationToken token)
        {
            string sums = await Http.GetStringAsync(release.SumsUrl, token);
            string? expected = sums.Split('\n')
                .Select(line => line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries))
                .FirstOrDefault(parts => parts.Length == 2 && parts[1].Equals(release.AssetName, StringComparison.OrdinalIgnoreCase))?[0];
            if (expected == null)
                return; // no line for this file: nothing to compare with

            await using var stream = File.OpenRead(packagePath);
            string actual = Convert.ToHexString(await SHA256.HashDataAsync(stream, token));
            if (!actual.Equals(expected, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException(AppDialog.Text("UpdateChecksumMsg"));
        }

        /// <summary>
        /// A small script waits for the app to close, then either runs the new installer silently
        /// (it starts the new version itself, see /RELAUNCH in installer\Foscamun.iss) or copies the
        /// new files over the old ones and starts the new version; finally it removes the downloads.
        /// </summary>
        private static void StartInstaller(string work, string? setupPath, string filesPath, string appFolder)
        {
            string script = Path.Combine(Path.GetTempPath(), "Foscamun-update.cmd");
            string exe = Path.Combine(appFolder, "Foscamun.exe");
            string[] install = setupPath != null
                ? new[] { $"start \"\" /wait \"{setupPath}\" /VERYSILENT /SUPPRESSMSGBOXES /NORESTART /RELAUNCH" }
                : new[]
                {
                    $"robocopy \"{filesPath}\" \"{appFolder}\" /e /r:10 /w:1 /njh /njs /nfl /ndl >nul",
                    $"start \"\" \"{exe}\""
                };
            var lines = new[]
            {
                "@echo off",
                "chcp 65001 >nul", // the paths below are UTF-8
                ":wait",
                $"tasklist /fi \"PID eq {Environment.ProcessId}\" | find \"{Environment.ProcessId}\" >nul && (timeout /t 1 /nobreak >nul & goto wait)"
            }
            .Concat(install)
            .Concat(new[]
            {
                $"rmdir /s /q \"{work}\"",
                "(goto) 2>nul & del \"%~f0\""
            })
            .ToArray();
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
