using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Xml.Linq;

namespace Foscamun.Properties
{
    /// <summary>
    /// .NET keeps user settings per exe path: a copy in another folder (the installed app after the
    /// ZIP, a ZIP extracted elsewhere) would start with empty settings. When <see cref="Settings.Upgrade"/>
    /// finds nothing, this takes the settings of the most recently used copy on this computer.
    /// </summary>
    internal static class SettingsImport
    {
        public static void FromLatestCopy()
        {
            try
            {
                string own = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
                string root = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Foscamun");
                if (!Directory.Exists(root))
                    return;

                var latest = Directory.GetDirectories(root, "Foscamun_*")
                    .SelectMany(dir => Directory.GetFiles(dir, "user.config", SearchOption.AllDirectories))
                    .Where(file => !string.Equals(Path.GetFullPath(file), Path.GetFullPath(own), StringComparison.OrdinalIgnoreCase))
                    .Select(file => new FileInfo(file))
                    .OrderByDescending(file => file.LastWriteTimeUtc)
                    .FirstOrDefault();
                if (latest == null)
                    return;

                var section = XDocument.Load(latest.FullName).Root?
                    .Element("userSettings")?
                    .Element("Foscamun.Properties.Settings");
                if (section == null)
                    return;

                foreach (var setting in section.Elements("setting"))
                {
                    string? name = setting.Attribute("name")?.Value;
                    string? value = setting.Element("value")?.Value;
                    if (name == null || value == null || name == nameof(Settings.SettingsUpgraded))
                        continue;

                    var property = Settings.Default.Properties[name];
                    if (property == null)
                        continue; // a setting this version no longer has

                    var converter = TypeDescriptor.GetConverter(property.PropertyType);
                    Settings.Default[name] = converter.ConvertFromInvariantString(value);
                }
            }
            catch (Exception ex)
            {
                // Settings are a convenience: never block the start for them
                System.Diagnostics.Debug.WriteLine($"Settings import failed: {ex.Message}");
            }
        }
    }
}
