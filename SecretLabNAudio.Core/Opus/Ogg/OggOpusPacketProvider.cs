using System.Buffers;
using Logger = LabApi.Features.Console.Logger;

namespace SecretLabNAudio.Core.Opus.Ogg;

public sealed class OggOpusPacketProvider : IOpusPacketProvider, ISeekable
{

    public const double SecondsPerSample = 1d / AudioConstants.SampleRate;

    public static OggOpusPacketProvider? TryCreate(Stream source)
    {
        var buffer = ArrayPool<byte>.Shared.Rent(OggPageExtensions.BufferSize);
        try
        {
            if (!OggPage.Read(source, buffer).IsIDHeader)
                return null;
            var channels = buffer[9];
            var preSkip = BitConverter.ToUInt16(buffer, 10);
            if (!OggPage.Read(source, buffer).IsCommentHeader)
                return null;
            OggPage? next;
            do
            {
                next = OggPage.Read(source, buffer);
            }
            while (next.IsContinuation);

            return next == null
                ? null
                : new OggOpusPacketProvider(source, channels, preSkip, next.Value);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    private readonly Stream _source;

    private readonly long _dataPosition;

    private readonly byte[] _rawPacketBuffer = new byte[OggPageExtensions.BufferSize];

    private readonly byte[] _singlePacketBuffer = new byte[AudioConstants.MaxEncodedBytesPerPacket];

    private OggPage? _page;

    private int _packetIndex;

    private int _packetLength;

    public int Channels { get; }

    public ushort PreSkip { get; }

    public long TotalSamples { get; }

    public TimeSpan CurrentTime
    {
        get => TimeSpan.FromSeconds(SamplePosition * SecondsPerSample);
        set => Seek((long) Math.Clamp(value.TotalSeconds * AudioConstants.SampleRate, 0, TotalSamples));
    }

    public TimeSpan TotalTime { get; }

    public long SamplePosition { get; private set; }

    public bool CanSeek => _source.CanSeek;

    private OggOpusPacketProvider(Stream source, int channels, ushort preSkip, OggPage next)
    {
        _source = source;
        Channels = channels;
        PreSkip = preSkip;
        if (source.CanSeek)
        {
            _dataPosition = source.Position - next.TotalSize;
            if (OggPage.TryGetLastGranulePosition(source, _rawPacketBuffer, out var granule))
                TotalSamples = granule;
        }

        TotalTime = TimeSpan.FromSeconds(TotalSamples * SecondsPerSample);
        _page = next.CopyTo(_rawPacketBuffer);
    }

    public int ReadPacket(Span<byte> buffer)
    {
        while (_page is { } page)
        {
            if (AdvancePage(buffer, page, out var length))
                return length;
            SamplePosition = page.GranulePosition;
            if (page.IsEndOfStream)
                break;
            _page = OggPage.Read(_source, _rawPacketBuffer);
            _packetLength = _packetIndex = 0;
        }

        _page = null;
        return 0;
    }

    public void Seek(long samplePosition)
    {
        if (samplePosition == 0)
        {
            _source.Position = _dataPosition;
            SamplePosition = 0;
            return;
        }

        throw new NotImplementedException(); // TODO
    }

    private bool AdvancePage(Span<byte> buffer, OggPage page, out int length)
    {
        for (; _packetIndex < page.Count; _packetIndex++)
        {
            var span = page[_packetIndex];
            if (_packetLength + span.Length >= AudioConstants.MaxEncodedBytesPerPacket)
            {
                // TODO: improve logging
                Logger.Warn("Packet too large");
                _packetLength = 0;
                continue;
            }

            span.CopyTo(_singlePacketBuffer.AsSpan(_packetLength));
            length = _packetLength += span.Length;
            if (span.Length >= byte.MaxValue)
                continue;
            SamplePosition += TocByte.TotalSamples(_singlePacketBuffer);
            _singlePacketBuffer.CopyTo(buffer);
            _packetLength = 0;
            _packetIndex++;
            return true;
        }

        length = 0;
        return false;
    }

    public void Dispose() => _source.Dispose();

}
