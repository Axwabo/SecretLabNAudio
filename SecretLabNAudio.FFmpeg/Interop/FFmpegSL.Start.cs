using System.ComponentModel;

namespace SecretLabNAudio.FFmpeg.Interop;

public sealed partial class FFmpegSL
{

    /// <summary>Path to the FFmpeg executable.</summary>
    public static string Path { get; set; } = "ffmpeg";

    /// <summary>
    /// The last <see cref="Win32Exception.NativeErrorCode"/> that was caught while starting the FFmpeg process.
    /// <see cref="NativeErrorCode.None"/> if the last FFmpeg startup was successful.
    /// This property does not change if a different exception was thrown.
    /// </summary>
    public static NativeErrorCode LastCaughtStartError { get; private set; }

    /// <summary>
    /// Starts FFmpeg with the specified arguments.
    /// </summary>
    /// <param name="arguments">The arguments to pass to FFmpeg.</param>
    /// <returns>A new <see cref="FFmpegSL"/> wrapper if the process was launched. Null if startup fails due to a <see cref="Win32Exception"/>.</returns>
    /// <remarks>Only <see cref="Win32Exception"/> exceptions are handled.</remarks>
    public static FFmpegSL? Start(string arguments)
    {
        Process? process = null;
        try
        {
            process = Process.Start(new ProcessStartInfo(Path, arguments)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true
            });
            if (process == null)
            {
                Debug.LogError("Failed to start FFmpeg: Process.Start returned null");
                LastCaughtStartError = NativeErrorCode.ProcessStartNull;
                return null;
            }

            LastCaughtStartError = NativeErrorCode.None;
            return new FFmpegSL(process);
        }
        catch (Win32Exception win32)
        {
            LastCaughtStartError = (NativeErrorCode) win32.NativeErrorCode;
            Debug.LogError($"Failed to start FFmpeg: {LastCaughtStartError switch
            {
                NativeErrorCode.FileNotFound or NativeErrorCode.PathNotFound => "FFmpeg not found. Check your configuration, or use the \"installFFmpeg\" command to install FFmpeg.",
                NativeErrorCode.AccessDenied => "Access is denied. Use the \"chmodFFmpeg\" command to make it executable.",
                _ => $"Native error code 0x{win32.NativeErrorCode:X8}"
            }}");
            Debug.LogError(win32);
            process?.Dispose();
            return null;
        }
        catch
        {
            process?.Dispose();
            throw;
        }
    }

    /// <summary>
    /// Starts FFmpeg with the specified arguments.
    /// </summary>
    /// <param name="arguments">The arguments to pass to FFmpeg.</param>
    /// <returns>A new <see cref="FFmpegSL"/> wrapper if the process was launched. Null if startup fails due to a <see cref="Win32Exception"/>.</returns>
    /// <remarks>Only <see cref="Win32Exception"/> exceptions are handled.</remarks>
    /// <include file="../XmlDocs/Args.xml" path="doc/InOut/exception"/>
    public static FFmpegSL? Start(FFmpegArguments arguments)
        => Start(arguments.ToArgumentsString());

}
