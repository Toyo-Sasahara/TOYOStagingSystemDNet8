using System.Drawing.Printing;
using System.Text.Json;

namespace SasaPdfRelayPrinter;

internal sealed class RelaySettings
{
    public int ConfigurationVersion { get; set; } = 1;
    public int ListenPort { get; set; } = 19111;
    public uint RenderDpi { get; set; } = 300;
    public string? TargetPrinterName { get; set; }

    public static string SettingsPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "SasaPdfRelayPrinter", "settings.json");

    public static RelaySettings Load()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                RelaySettings loaded = JsonSerializer.Deserialize<RelaySettings>(File.ReadAllText(SettingsPath)) ?? new();
                if (loaded.ConfigurationVersion < 1)
                {
                    loaded.ConfigurationVersion = 1;
                }
                return loaded;
            }
        }
        catch
        {
        }

        return new();
    }

    public void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
        File.WriteAllText(SettingsPath, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
    }

    public static IReadOnlyList<string> GetInstalledPrinters()
    {
        return PrinterSettings.InstalledPrinters.Cast<string>().OrderBy(static x => x).ToArray();
    }
}
