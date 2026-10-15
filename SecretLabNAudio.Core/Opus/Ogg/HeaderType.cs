namespace SecretLabNAudio.Core.Opus.Ogg;

[Flags]
public enum HeaderType : byte
{

    None = 0,
    Continuation = 0x01,
    BeginningOfStream = 0x02,
    EndOfStream = 0x04

}
