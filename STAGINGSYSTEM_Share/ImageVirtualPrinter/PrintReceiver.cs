using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SasaImagePrinter;

internal sealed class PrintReceiver(int port, string format, PrinterSettings settings) : IAsyncDisposable
{
    private readonly TcpListener _listener = new(IPAddress.Loopback, port);
    private readonly CancellationTokenSource _stop = new();
    private Task? _loop;
    public event Action<string>? Status;

    public void Start()
    {
        _listener.Start();
        _loop = AcceptLoopAsync();
        Status?.Invoke($"{format} printer is ready on 127.0.0.1:{port}");
    }

    private async Task AcceptLoopAsync()
    {
        while (!_stop.IsCancellationRequested)
        {
            try
            {
                TcpClient client = await _listener.AcceptTcpClientAsync(_stop.Token);
                _ = ProcessAsync(client);
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex) { Status?.Invoke($"Receive error: {ex.Message}"); }
        }
    }

    private async Task ProcessAsync(TcpClient client)
    {
        using (client)
        using (NetworkStream network = client.GetStream())
        using (var memory = new MemoryStream())
        {
            try
            {
                await network.CopyToAsync(memory, _stop.Token);
                byte[] payload = ExtractPdf(memory.ToArray());
                if (payload.Length == 0) throw new InvalidDataException("The print job did not contain PDF data.");
                Status?.Invoke($"Converting {format} job ({payload.Length:N0} bytes)...");
                IReadOnlyList<string> files = await PdfImageConverter.ConvertAsync(payload, format, settings, _stop.Token);
                Status?.Invoke($"Saved {files.Count} page(s): {settings.OutputDirectory}");
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                Status?.Invoke($"{format} job failed: {ex.Message}");
            }
        }
    }

    internal static byte[] ExtractPdf(byte[] raw)
    {
        ReadOnlySpan<byte> header = "%PDF-"u8;
        int start = raw.AsSpan().IndexOf(header);
        if (start < 0) return [];
        ReadOnlySpan<byte> eof = "%%EOF"u8;
        int relativeEnd = raw.AsSpan(start).LastIndexOf(eof);
        int length = relativeEnd < 0 ? raw.Length - start : relativeEnd + eof.Length;
        return raw.AsSpan(start, length).ToArray();
    }

    public async ValueTask DisposeAsync()
    {
        _stop.Cancel();
        _listener.Stop();
        if (_loop is not null) try { await _loop; } catch { }
        _stop.Dispose();
    }
}
