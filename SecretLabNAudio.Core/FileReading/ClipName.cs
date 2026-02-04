namespace SecretLabNAudio.Core.FileReading;

public record struct ClipName(string Original, bool TrimExtension = true)
{

    public static implicit operator ClipName(string original) => new(original);

    public static implicit operator ClipName((string Original, bool TrimExtension) tuple)
        => new(tuple.Original, tuple.TrimExtension);

    public override string ToString() => TrimExtension
        ? Path.ChangeExtension(Original, null)
        : Original;

}
