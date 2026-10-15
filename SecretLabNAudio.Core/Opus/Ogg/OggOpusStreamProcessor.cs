using VoiceChat.Codec;

namespace SecretLabNAudio.Core.Opus.Ogg;

public sealed class OggOpusStreamProcessor : IStreamAudioProcessor
{

    private readonly OggOpusPacketProvider _provider;

    private readonly OpusDecoder _decoder = new();

    private readonly byte[] _packetBuffer = new byte[AudioConstants.MaxEncodedBytesPerPacket];

    private readonly float[] _sampleBuffer = new float[AudioConstants.VoiceChatBufferSize];

    private int _remainingStart;
    private int _remainingLength;

    private int _skip;

    private readonly long _total;

    public long SamplePosition { get; private set; }

    public OggOpusStreamProcessor(OggOpusPacketProvider provider)
    {
        _provider = provider;
        _skip = provider.PreSkip;
        _total = provider.TotalSamples * provider.Channels;
        TotalTime = provider.TotalTime;
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
            SamplePosition += read / _provider.Channels;
            _remainingStart = _remainingLength = 0;
        }

        while (total < count)
        {
            var packetLength = _provider.ReadPacket(_packetBuffer);
            if (packetLength == 0)
                break;
            var decoded = _decoder.Decode(_packetBuffer, packetLength, _sampleBuffer) * _provider.Channels;
            var read = _provider.TotalSamples == 0 ? decoded : Math.Min(decoded, (int) (_provider.TotalSamples - SamplePosition));
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
            SamplePosition += write / _provider.Channels;
            if (read <= needed)
                continue;
            _remainingStart = sampleOffset + write;
            _remainingLength = read - needed;
        }

        return total;
    }

    public WaveFormat WaveFormat { get; }

    public TimeSpan CurrentTime
    {
        get => TimeSpan.FromSeconds(SamplePosition / OggOpusPacketProvider.SecondsPerSample);
        set => throw new NotImplementedException();
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
