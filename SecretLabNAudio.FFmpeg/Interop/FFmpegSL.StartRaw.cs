using System.ComponentModel;
using System.Diagnostics;

namespace SecretLabNAudio.FFmpeg.Interop;

public sealed partial class FFmpegSL
{

    public static string Path { get; set; } = "ffmpeg";

    public static NativeErrorCode LastCaughtStartError { get; private set; }

    public static FFmpegSL? StartRaw(string arguments, bool redirectStandardInput = false)
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
            if (process == null)
            {
                Debug.Log("Failed to start FFmpeg: Process.Start returned null");
                LastCaughtStartError = NativeErrorCode.ProcessStartNull;
                return null;
            }

            LastCaughtStartError = NativeErrorCode.None;
            return new FFmpegSL(process);
        }
        catch (Win32Exception win32)
        {
            LastCaughtStartError = (NativeErrorCode) win32.NativeErrorCode;
            Debug.Log($"Failed to start FFmpeg: {LastCaughtStartError switch
            {
                NativeErrorCode.FileNotFound or NativeErrorCode.PathNotFound => "FFmpeg not found. Check your configuration, or use the \"installFFmpeg\" command to install FFmpeg.",
                NativeErrorCode.AccessDenied => "Access is denied. Use the \"chmodFFmpeg\" command to make it executable.",
                _ => $"Native error code {win32.NativeErrorCode}"
            }}");
            Debug.Log(win32);
            return null;
        }
    }

}
