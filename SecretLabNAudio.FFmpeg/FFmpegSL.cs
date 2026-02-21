using System.ComponentModel;
using System.Diagnostics;

namespace SecretLabNAudio.FFmpeg;

public sealed class FFmpegSL : IDisposable
{

    public static string Path { get; set; } = "ffmpeg";

    private readonly Process _process;

    public static FFmpegSL? StartRaw(string arguments)
    {
        try
        {
            var process = Process.Start(new ProcessStartInfo(Path)
            {
                Arguments = arguments,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            });
            return process == null ? null : new FFmpegSL(process);
        }
        catch (Win32Exception)
        {
            return null;
        }
    }

    /*
    public static FFmpegSL StartRaw(string inputs, string filters, string format)
    {
TODO
    }
    */

    private FFmpegSL(Process process) => _process = process;

    public StreamReader Stdout => _process.StandardOutput;

    public void Dispose()
    {
        _process.CloseMainWindow();
        _process.Dispose();
    }

}
