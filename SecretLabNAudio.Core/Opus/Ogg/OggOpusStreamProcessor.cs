using SecretLabNAudio.Core.Extensions;
using VoiceChat.Codec;

namespace SecretLabNAudio.Core.Opus.Ogg;

public sealed class OggOpusStreamProcessor : IStreamAudioProcessor
{

    public const double PreRollSeconds = 0.08;
    public const int PreRollSamples = (int) (PreRollSeconds * AudioConstants.SampleRate);

    private readonly OggOpusPacketProvider _provider;

    private readonly OpusDecoder _decoder = new();

    private readonly byte[] _packetBuffer = new byte[AudioConstants.MaxEncodedBytesPerPacket];

    private readonly float[] _sampleBuffer = new float[AudioConstants.VoiceChatBufferSize];

    private readonly long _total;
    private readonly int _totalPreSkip;

    private int _remainingStart;
    private int _remainingLength;

    private int _skip;

    public long SamplePosition { get; private set; }

    public void Seek(long sample)
    {
        sample = Math.Clamp(sample, 0, _provider.TotalSamples - _provider.PreSkip);
        var granule = sample - PreRollSamples;
        _provider.Seek(granule);
        _skip = granule <= 0 ? _totalPreSkip : PreRollSamples * _provider.Channels;
        SamplePosition = sample;
    }

    public OggOpusStreamProcessor(OggOpusPacketProvider provider)
    {
        _provider = provider;
        _total = provider.TotalSamples * provider.Channels;
        _totalPreSkip = _skip = provider.PreSkip * provider.Channels;
        TotalTime = TimeSpan.FromSamples(provider.TotalSamples - provider.PreSkip);
        WaveFormat = provider.Channels == AudioConstants.Channels
            ? AudioConstants.SupportedFormat
            : WaveFormat.CreateIeeeFloatWaveFormat(AudioConstants.SampleRate, provider.Channels);
    }

    public int Read(float[] buffer, int offset, int count)
    {
        var total = 0;
        if (_remainingLength != 0)
        {
            var read = Math.Min(count, _remainingLength);
            _sampleBuffer.AsSpan(_remainingStart, read).CopyTo(buffer.AsSpan(offset));
            offset += read;
            total += read;
            _remainingStart += read;
            _remainingLength -= read;
            SamplePosition += read / _provider.Channels;
        }

        while (total < count)
        {
            var packetLength = _provider.ReadPacket(_packetBuffer);
            if (packetLength != 0)
            {
                DecodePacket(packetLength, buffer, ref offset, count, ref total);
                continue;
            }

            if (Loop && _provider.CanSeek)
                Seek(0);
            break;
        }

        return total;
    }

    private void DecodePacket(int packetLength, float[] buffer, ref int offset, int count, ref int total)
    {
        var decoded = _decoder.Decode(_packetBuffer, packetLength, _sampleBuffer);
        var read = _total == 0 ? decoded : Math.Min(decoded, (int) (_total - SamplePosition));
        var sampleOffset = 0;
        if (_skip != 0)
        {
            var skip = Math.Min(_skip, read);
            read -= skip;
            sampleOffset = skip;
            _skip -= skip;
        }

        var needed = count - total;
        var write = Math.Min(needed, read);
        _sampleBuffer.AsSpan(sampleOffset, write).CopyTo(buffer.AsSpan(offset));
        total += write;
        offset += write;
        SamplePosition = _provider.SamplePosition - _provider.PreSkip;
        if (read <= needed)
            return;
        _remainingStart = sampleOffset + write;
        _remainingLength = read - needed;
        SamplePosition -= _remainingLength / _provider.Channels;
    }

    public WaveFormat WaveFormat { get; }

    public TimeSpan CurrentTime
    {
        get => TimeSpan.FromSamples(SamplePosition);
        set => SamplePosition = value.Samples;
    }

    public TimeSpan TotalTime { get; }

    public bool Loop { get; set; }

    public string? FilePath { get; init; }

    public void Dispose()
    {
        _provider.Dispose();
        _decoder.Dispose();
    }

}
