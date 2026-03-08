namespace SecretLabNAudio.Core.FileReading;

/// <summary>
/// Represents a short clip's name with an option to trim the file extension.
/// </summary>
/// <param name="Original">The original name.</param>
/// <param name="TrimExtension">Whether to trim the file extension.</param>
public readonly record struct ClipName(string Original, bool TrimExtension = true)
{

    /// <summary>
    /// Converts a string into a <see cref="ClipName"/> with <see cref="TrimExtension"/> = true.
    /// </summary>
    /// <param name="original">The original name.</param>
    /// <returns>A new <see cref="ClipName"/>.</returns>
    public static implicit operator ClipName(string original) => new(original);

    /// <summary>
    /// Converts this name to its final form.
    /// </summary>
    /// <returns>The extension removed if <see cref="TrimExtension"/> is true, <see cref="Original"/> otherwise.</returns>
    public override string ToString() => TrimExtension
        ? Path.ChangeExtension(Original, null)
        : Original;

    /// <summary>
    /// Converts a name and trim flag tuple to a <see cref="ClipName"/>.
    /// </summary>
    /// <param name="tuple">The values to convert.</param>
    /// <returns>An equivalent <see cref="ClipName"/>.</returns>
    public static implicit operator ClipName((string Original, bool TrimExtension) tuple)
        => new(tuple.Original, tuple.TrimExtension);

    /// <summary>
    /// Converts the path to a <see cref="ClipName"/> based on the file's name.
    /// </summary>
    /// <param name="path">The path (absolute or relative) to convert.</param>
    /// <param name="trimExtension">The value of <see cref="TrimExtension"/> in the result.</param>
    /// <returns>A new <see cref="ClipName"/> corresponding to the file name.</returns>
    public static ClipName FromPath(string path, bool trimExtension = true) => new(Path.GetFileName(path), trimExtension);

}
