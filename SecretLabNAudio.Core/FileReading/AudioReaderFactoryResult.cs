using NAudio.Wave;

namespace SecretLabNAudio.Core.FileReading;

public interface IWaveStreamResult
{

    WaveStream Stream { get; }

}

public interface ISampleProviderResult
{

    ISampleProvider Provider { get; }

}

public abstract record AudioReaderFactoryResult;

public sealed record WaveStreamResult(WaveStream Stream) : AudioReaderFactoryResult, IWaveStreamResult;

public sealed record SampleProviderResult(ISampleProvider Provider) : AudioReaderFactoryResult, ISampleProviderResult;

public sealed record StreamAndProviderResult(WaveStream Stream, ISampleProvider Provider) : AudioReaderFactoryResult, IWaveStreamResult, ISampleProviderResult;
