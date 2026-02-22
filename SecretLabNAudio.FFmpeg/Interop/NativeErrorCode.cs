namespace SecretLabNAudio.FFmpeg.Interop;

/// <summary>
/// Some native error code mappings for <see cref="System.ComponentModel.Win32Exception.NativeErrorCode"/>
/// </summary>
/// <seealso href="https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-erref/18d8fbe8-a967-4f1c-ae50-99ca8e491d2d"/>
public enum NativeErrorCode
{

    /// <summary>
    /// Though not a win32 error code, indicates that the object returned by <see cref="System.Diagnostics.Process.Start(System.Diagnostics.ProcessStartInfo)"/> was null.
    /// </summary>
    ProcessStartNull = -1,

    /// <summary>
    /// The operation completed successfully.
    /// </summary>
    None = 0,

    /// <summary>
    /// The system cannot find the file specified. (ERROR_FILE_NOT_FOUND)
    /// </summary>
    FileNotFound = 2,

    /// <summary>
    /// The system cannot find the path specified. (ERROR_PATH_NOT_FOUND)
    /// </summary>
    PathNotFound = 3,
    
    /// <summary>
    /// Access is denied. (ERROR_ACCESS_DENIED)
    /// </summary>
    /// <remarks>This often occurs when the file's mode is not executable on UNIX-like systems.</remarks>
    AccessDenied = 5

}
