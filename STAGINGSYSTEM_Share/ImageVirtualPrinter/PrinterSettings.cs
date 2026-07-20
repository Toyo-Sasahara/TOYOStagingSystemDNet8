using System.Text.Json;

namespace SasaImagePrinter;

internal sealed class PrinterSettings
{
    public int ConfigurationVersion { get; set; } = 2;
    public string OutputDirectory { get; set; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Image Printer Output");
    public uint Dpi { get; set; } = 400;
    public int JpegPort { get; set; } = 19101;
    public int TiffPort { get; set; } = 19102;

    public static string SettingsPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "SasaImagePrinter", "settings.json");

    public static PrinterSettings Load()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                PrinterSettings loaded = JsonSerializer.Deserialize<PrinterSettings>(File.ReadAllText(SettingsPath)) ?? new();
                if (loaded.ConfigurationVersion < 2)
                {
                    loaded.Dpi = 400;
                    loaded.ConfigurationVersion = 2;
                }
                return loaded;
            }
        }
        catch { }
        return new();
    }

    public void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
        File.WriteAllText(SettingsPath, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
    }
}
