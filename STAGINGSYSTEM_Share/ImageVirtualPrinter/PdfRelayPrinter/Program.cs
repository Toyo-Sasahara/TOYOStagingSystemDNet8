using System.Diagnostics;
using System.Drawing.Printing;

namespace SasaPdfRelayPrinter;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        using var singleInstance = new Mutex(true, "Local\\SasaPdfRelayPrinter", out bool created);
        if (!created)
        {
            return;
        }

        ApplicationConfiguration.Initialize();
        Application.Run(new RelayApplicationContext());
    }
}

internal sealed class RelayApplicationContext : ApplicationContext
{
    private readonly RelaySettings _settings = RelaySettings.Load();
    private readonly NotifyIcon _icon;
    private readonly PdfRelayReceiver _receiver;

    public RelayApplicationContext()
    {
        if (string.IsNullOrWhiteSpace(_settings.TargetPrinterName))
        {
            _settings.TargetPrinterName = GetDefaultPrinterName();
        }

        _settings.Save();

        var menu = new ContextMenuStrip();
        menu.Items.Add("Select target printer", null, (_, _) => SelectTargetPrinter());
        menu.Items.Add("Open settings file", null, (_, _) => OpenSettingsFolder());
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Exit", null, async (_, _) => await ExitAsync());

        _icon = new NotifyIcon
        {
            Icon = SystemIcons.Application,
            Text = "Sasa PDF Relay Printer",
            Visible = true,
            ContextMenuStrip = menu
        };

        _receiver = new PdfRelayReceiver(_settings);
        _receiver.Status += ShowStatus;

        try
        {
            _receiver.Start();
            ShowStatus($"Target printer: {_settings.TargetPrinterName ?? "(not selected)"}");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Could not start the PDF relay receiver.\n\n{ex.Message}",
                "Sasa PDF Relay Printer", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static string? GetDefaultPrinterName()
    {
        using var printerSettings = new PrinterSettings();
        return printerSettings.IsValid ? printerSettings.PrinterName : RelaySettings.GetInstalledPrinters().FirstOrDefault();
    }

    private void SelectTargetPrinter()
    {
        using var dialog = new Form
        {
            Text = "Select target printer",
            Width = 500,
            Height = 400,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition = FormStartPosition.CenterScreen,
            MaximizeBox = false,
            MinimizeBox = false
        };

        var list = new ListBox { Dock = DockStyle.Fill };
        list.Items.AddRange(RelaySettings.GetInstalledPrinters().Cast<object>().ToArray());
        if (!string.IsNullOrWhiteSpace(_settings.TargetPrinterName))
        {
            int index = list.Items.IndexOf(_settings.TargetPrinterName);
            if (index >= 0)
            {
                list.SelectedIndex = index;
            }
        }

        var ok = new Button { Text = "OK", DialogResult = DialogResult.OK, Dock = DockStyle.Right, Width = 80 };
        var cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Dock = DockStyle.Right, Width = 80 };
        var panel = new Panel { Dock = DockStyle.Bottom, Height = 42 };
        panel.Controls.Add(cancel);
        panel.Controls.Add(ok);

        dialog.Controls.Add(list);
        dialog.Controls.Add(panel);
        dialog.AcceptButton = ok;
        dialog.CancelButton = cancel;

        if (dialog.ShowDialog() == DialogResult.OK && list.SelectedItem is string selected)
        {
            _settings.TargetPrinterName = selected;
            _settings.Save();
            ShowStatus($"Target printer updated: {selected}");
        }
    }

    private void OpenSettingsFolder()
    {
        string path = Path.GetDirectoryName(RelaySettings.SettingsPath)!;
        Directory.CreateDirectory(path);
        Process.Start(new ProcessStartInfo("explorer.exe", path) { UseShellExecute = true });
    }

    private void ShowStatus(string text)
    {
        _icon.Text = text.Length > 63 ? text[..63] : text;
        _icon.ShowBalloonTip(3000, "Sasa PDF Relay Printer", text, ToolTipIcon.Info);
    }

    private async Task ExitAsync()
    {
        _icon.Visible = false;
        await _receiver.DisposeAsync();
        _icon.Dispose();
        ExitThread();
    }
}
