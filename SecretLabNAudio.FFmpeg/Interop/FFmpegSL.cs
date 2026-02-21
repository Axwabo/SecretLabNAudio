using System.Diagnostics;

namespace SecretLabNAudio.FFmpeg.Interop;

public sealed partial class FFmpegSL : IDisposable
{

    private readonly Process _process;

    private bool _disposed;

    /*
    public static FFmpegSL StartRaw(string inputs, string filters, string format)
    {
TODO
    }
    */

    private FFmpegSL(Process process) => _process = process;

    public StreamWriter Stdin => _process.StandardInput;

    public StreamReader Stdout => _process.StandardOutput;

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        if (!_process.HasExited)
            _process.CloseMainWindow();
        _process.Dispose();
    }

}
