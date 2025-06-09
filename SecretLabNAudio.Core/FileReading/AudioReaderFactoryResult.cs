using NAudio.Wave;

namespace SecretLabNAudio.Core.FileReading;

public readonly record struct AudioReaderFactoryResult(WaveStream? Stream, ISampleProvider? Provider)
{

    public static implicit operator AudioReaderFactoryResult(WaveStream stream) => new(stream, stream.ToSampleProvider());

}
