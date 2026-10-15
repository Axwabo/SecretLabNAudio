using SecretLabNAudio.Core.Providers;
using VoiceChat.Codec;

namespace SecretLabNAudio.Core.Opus.Ogg;

public sealed class OggOpusStreamProcessor : IStreamAudioProcessor
{

    private readonly OggOpusPacketProvider _provider;

    private readonly OpusDecoder _decoder = new();

    private readonly PlaybackBufferSampleProvider _remaining;

    private readonly byte[] _packetBuffer = new byte[AudioConstants.MaxEncodedBytesPerPacket];

    private readonly float[] _sampleBuffer = new float[AudioConstants.VoiceChatBufferSize];

    private int _skip;

    public long SamplePosition { get; private set; }

    public OggOpusStreamProcessor(OggOpusPacketProvider provider)
    {
        _provider = provider;
        _remaining = new PlaybackBufferSampleProvider(AudioConstants.VoiceChatBufferSize, true, AudioConstants.SampleRate, provider.Channels);
        _skip = provider.PreSkip;
        TotalTime = provider.TotalTime;
        WaveFormat = provider.Channels == AudioConstants.Channels
            ? AudioConstants.SupportedFormat
            : WaveFormat.CreateIeeeFloatWaveFormat(AudioConstants.SampleRate, provider.Channels);
    }

    public int Read(float[] buffer, int offset, int count)
    {
        var total = 0;
        if (_remaining.Length != 0)
        {
            var read = _remaining.Read(buffer, offset, count);
            offset += read;
            total += read;
            count -= read;
        }

        while (total < count)
        {
            var packetLength = _provider.ReadPacket(_packetBuffer);
            if (packetLength == 0)
                break;
            var read = _decoder.Decode(_packetBuffer, packetLength, _sampleBuffer);
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
            if (read > needed)
                _remaining.Write(_sampleBuffer, sampleOffset + read, read - needed);
        }

        SamplePosition += total;
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
