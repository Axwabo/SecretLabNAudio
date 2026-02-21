using System.ComponentModel;
using System.Diagnostics;

namespace SecretLabNAudio.FFmpeg.Interop;

public sealed class FFmpegSL : IDisposable
{

    public static string Path { get; set; } = "ffmpeg";

    private readonly Process _process;

    private bool _disposed;

    public static StartResult StartRaw(string arguments, bool redirectStandardInput = false)
    {
        try
        {
            var process = Process.Start(new ProcessStartInfo(Path)
            {
                Arguments = arguments,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = redirectStandardInput
            });
            return process == null ? (null, NativeErrorCode.ProcessStartFailed) : (new FFmpegSL(process), NativeErrorCode.None);
        }
        catch (Win32Exception win32)
        {
            return (null, (NativeErrorCode) win32.NativeErrorCode);
        }
    }

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
