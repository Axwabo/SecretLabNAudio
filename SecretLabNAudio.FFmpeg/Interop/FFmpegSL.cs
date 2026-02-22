using System.Diagnostics;

namespace SecretLabNAudio.FFmpeg.Interop;

public sealed partial class FFmpegSL : IDisposable
{

    private readonly Process _process;

    private bool _disposed;

    private FFmpegSL(Process process) => _process = process;

    public StreamWriter Stdin => _process.StandardInput;

    public StreamReader Stdout => _process.StandardOutput;

    public StreamReader Stderr => _process.StandardError;

    public bool HasExited => _process.HasExited;

    public void WaitForExit() => _process.WaitForExit();

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        if (!HasExited)
            _process.Kill();
        _process.Dispose();
    }

}
