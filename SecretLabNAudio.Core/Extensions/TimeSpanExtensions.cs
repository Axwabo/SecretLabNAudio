namespace SecretLabNAudio.Core.Extensions;

public static class TimeSpanExtensions
{

    public const double SecondsPerSample = 1d / AudioConstants.SampleRate;

    extension(TimeSpan timeSpan)
    {

        public static TimeSpan FromSamples(long samples) => TimeSpan.FromSeconds(samples * SecondsPerSample);

        public long Samples => (long) (timeSpan.TotalSeconds * AudioConstants.SampleRate);

    }

}
