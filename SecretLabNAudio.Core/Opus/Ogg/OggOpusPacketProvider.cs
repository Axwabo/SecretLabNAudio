using System.Buffers;
using Logger = LabApi.Features.Console.Logger;

namespace SecretLabNAudio.Core.Opus.Ogg;

public sealed class OggOpusPacketProvider : IOpusPacketProvider
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
            var dataPosition = 0L;
            var canSeek = source.CanSeek;
            OggPage? next;
            do
            {
                if (canSeek)
                    dataPosition = source.Position;
                next = OggPage.Read(source, buffer);
            }
            while (next.IsContinuation);

            return next == null
                ? null
                : new OggOpusPacketProvider(source, channels, preSkip, next.Value, dataPosition);
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

    public TimeSpan TotalTime { get; }

    public long LastPagePosition { get; private set; }

    private OggOpusPacketProvider(Stream source, int channels, ushort preSkip, OggPage next, long dataPosition)
    {
        _source = source;
        _dataPosition = dataPosition;
        Channels = channels;
        PreSkip = preSkip;
        if (OggPage.TryGetLastGranulePosition(source, _rawPacketBuffer, out var total))
            TotalSamples = total - preSkip;
        TotalTime = TimeSpan.FromSeconds(TotalSamples * SecondsPerSample / Channels); // TODO: divide or not
        _page = next.CopyTo(_rawPacketBuffer);
    }

    public int ReadPacket(Span<byte> buffer)
    {
        while (_page is { } page)
        {
            if (AdvancePage(buffer, page, out var length))
                return length;
            if (page.GranulePosition != -1)
                LastPagePosition = page.GranulePosition - PreSkip;
            if (page.IsEndOfStream)
                break;
            _page = OggPage.Read(_source, _rawPacketBuffer);
            _packetLength = _packetIndex = 0;
        }

        _page = null;
        return 0;
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
