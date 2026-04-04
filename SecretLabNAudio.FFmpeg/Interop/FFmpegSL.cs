namespace SecretLabNAudio.FFmpeg.Interop;

/// <summary>
/// A convenience wrapper for the FFmpeg process.
/// The process will be killed if it hasn't exited when disposing.
/// </summary>
public sealed partial class FFmpegSL : IDisposable, IFFmpegWrapper
{

    private readonly Process _process;

    /// <inheritdoc/>
    public bool IsDisposed { get; private set; }

    private FFmpegSL(Process process) => _process = process;

    /// <summary>The standard input of the process.</summary>
    public StreamWriter Stdin => _process.StandardInput;

    /// <summary>The standard output of the process.</summary>
    /// <remarks>FFmpeg logs most messages in <see cref="Stderr"/>.</remarks>
    public StreamReader Stdout => _process.StandardOutput;

    /// <summary>The standard error of the process.</summary>
    public StreamReader Stderr => _process.StandardError;

    /// <inheritdoc/>
    /// <seealso cref="WaitForExit"/>
    public bool HasExited => _process.HasExited;

    /// <inheritdoc/>
    public int ExitCode => _process.ExitCode;

    /// <inheritdoc/>
    public string? FinalErrorMessage => field ?? (IsDisposed || !HasExited ? null : field = Stderr.ReadToEnd());

    /// <summary>Instructs the process to wait the specified number of milliseconds for the associated process to exit.</summary>
    /// <param name="milliseconds">
    /// The amount of time, in milliseconds, to wait for the associated process to exit.
    /// A value of 0 specifies an immediate return, and a value of -1 specifies an infinite wait.
    /// </param>
    /// <returns>true if the associated process has exited; otherwise, false.</returns>
    /// <remarks>This method does not instruct the process to exit.</remarks>
    /// <exception cref="System.ComponentModel.Win32Exception">The wait setting could not be accessed.</exception>
    /// <exception cref="SystemException">There is no process associated with this Process object.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="milliseconds"/> is a negative number other than -1, which represents an infinite time-out.</exception>
    public bool WaitForExit(int milliseconds = -1) => _process.WaitForExit(milliseconds);

    /// <summary>
    /// Kills the underlying process if it hasn't exited yet, and disposes of the managed process object.
    /// </summary>
    public void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
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
