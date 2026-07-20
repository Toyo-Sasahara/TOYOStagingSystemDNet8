using System.Diagnostics;

namespace SasaImagePrinter;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        using var singleInstance = new Mutex(true, "Local\\SasaImagePrinter", out bool created);
        if (!created) return;
        ApplicationConfiguration.Initialize();
        Application.Run(new PrinterApplicationContext());
    }
}

internal sealed class PrinterApplicationContext : ApplicationContext
{
    private readonly PrinterSettings _settings = PrinterSettings.Load();
    private readonly NotifyIcon _icon;
    private readonly PrintReceiver _jpeg;
    private readonly PrintReceiver _tiff;

    public PrinterApplicationContext()
    {
        // ISO A0・A1 フォームを登録（存在しない場合のみ）
        try
        {
            PrinterFormHelper.RegisterIsoAForms();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"[PrinterApplicationContext] Warning: Printer form registration failed: {ex.Message}");
            // フォーム登録失敗は非致命的 (continue)
        }

        _settings.Save();
        var menu = new ContextMenuStrip();
        menu.Items.Add("Open output folder", null, (_, _) => OpenOutput());
        menu.Items.Add("Settings", null, (_, _) => ShowSettings());
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Exit", null, async (_, _) => await ExitAsync());
        _icon = new NotifyIcon
        {
            Icon = SystemIcons.Application,
            Text = "Sasa Image Printer",
            Visible = true,
            ContextMenuStrip = menu
        };
        _icon.DoubleClick += (_, _) => OpenOutput();

        _jpeg = new PrintReceiver(_settings.JpegPort, "JPEG", _settings);
        _tiff = new PrintReceiver(_settings.TiffPort, "TIFF", _settings);
        _jpeg.Status += ShowStatus;
        _tiff.Status += ShowStatus;
        try { _jpeg.Start(); _tiff.Start(); }
        catch (Exception ex)
        {
            MessageBox.Show($"Could not start the image printer receiver.\n\n{ex.Message}",
                "Sasa Image Printer", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ShowStatus(string text)
    {
        if (_icon is null) return;
        _icon.Text = text.Length > 63 ? text[..63] : text;
        _icon.ShowBalloonTip(3000, "Sasa Image Printer", text, ToolTipIcon.Info);
    }

    private void OpenOutput()
    {
        Directory.CreateDirectory(_settings.OutputDirectory);
        Process.Start(new ProcessStartInfo("explorer.exe", _settings.OutputDirectory) { UseShellExecute = true });
    }

    private void ShowSettings()
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "Select the folder where printed images will be saved.",
            SelectedPath = _settings.OutputDirectory,
            UseDescriptionForTitle = true
        };
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            _settings.OutputDirectory = dialog.SelectedPath;
            _settings.Save();
            ShowStatus("Output folder updated.");
        }
    }

    private async Task ExitAsync()
    {
        _icon.Visible = false;
        await _jpeg.DisposeAsync();
        await _tiff.DisposeAsync();
        _icon.Dispose();
        ExitThread();
    }
}
