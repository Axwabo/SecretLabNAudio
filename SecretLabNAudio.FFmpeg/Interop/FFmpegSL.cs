using System.Diagnostics;

namespace SecretLabNAudio.FFmpeg.Interop;

/// <summary>
/// A convenience wrapper for the FFmpeg process.
/// The process will be killed if it hasn't exited when disposing.
/// </summary>
public sealed partial class FFmpegSL : IDisposable
{

    private readonly Process _process;
    private readonly bool _redirectStandardInput;

    private bool _disposed;

    private FFmpegSL(Process process, bool redirectStandardInput)
    {
        _process = process;
        _redirectStandardInput = redirectStandardInput;
    }

    /// <summary>The standard input of the process. Null if the standard input has not been redirected.</summary>
    public StreamWriter? Stdin => _redirectStandardInput ? _process.StandardInput : null;

    /// <summary>The standard output of the process.</summary>
    /// <remarks>FFmpeg logs most messages in <see cref="Stderr"/>.</remarks>
    public StreamReader Stdout => _process.StandardOutput;

    /// <summary>The standard error of the process.</summary>
    public StreamReader Stderr => _process.StandardError;

    /// <inheritdoc cref="Process.HasExited"/>
    public bool HasExited => _process.HasExited;

    public string? FinalErrorMessage => field ?? (_disposed || !HasExited ? null : field = Stderr.ReadToEnd());

    /// <inheritdoc cref="Process.WaitForExit(int)"/>
    public bool WaitForExit(int milliseconds = -1) => _process.WaitForExit(milliseconds);

    /// <summary>
    /// Kills the underlying process if it hasn't exited yet, and disposes of the managed process object.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        if (!HasExited)
            try
            {
                _process.Kill();
            }
            catch
            {
                // InvalidOperation: process has already exited
                // Win32Exception: Success
                // like this is just stupid
            }

        _process.Dispose();
    }

}
