using System.Buffers;
using Logger = LabApi.Features.Console.Logger;

namespace SecretLabNAudio.Core.Opus.Ogg;

public sealed class OggOpusPacketProvider : IOpusPacketProvider
{

    public const double SecondsPerSample = 1d / AudioConstants.SampleRate;
    private const int PageSize = ushort.MaxValue;

    public static OggOpusPacketProvider? TryCreate(Stream source)
    {
        var buffer = ArrayPool<byte>.Shared.Rent(PageSize);
        try
        {
            if (!OggPage.Read(source, buffer).IsIDHeader)
                return null;
            var channels = buffer[9];
            if (!OggPage.Read(source, buffer).IsCommentHeader)
                return null;
            long dataPosition;
            OggPage? next;
            do
            {
                dataPosition = source.Position;
                next = OggPage.Read(source, buffer);
            }
            while (next.IsContinuation);

            return next == null
                ? null
                : new OggOpusPacketProvider(source, channels, next.Value, dataPosition);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    private readonly Stream _source;

    private readonly long _dataPosition;

    private readonly byte[] _rawPacketBuffer = new byte[PageSize];

    private readonly byte[] _singlePacketBuffer = new byte[AudioConstants.MaxEncodedBytesPerPacket];

    private OggPage? _page;

    private int _packetIndex;

    private int _packetLength;

    public int Channels { get; }

    public int TotalSamples { get; }

    private OggOpusPacketProvider(Stream source, int channels, OggPage next, long dataPosition)
    {
        _source = source;
        _dataPosition = dataPosition;
        Channels = channels;
        TotalSamples = CalculateTotalSamples();
        _page = next.CopyTo(_rawPacketBuffer);
    }

    private int CalculateTotalSamples()
    {
        if (!_source.CanSeek)
            return 0;
        if (_source.Length < PageSize)
            _source.Seek(0, SeekOrigin.Begin);
        else
            _source.Seek(-PageSize, SeekOrigin.End);
        var read = _source.Read(_rawPacketBuffer);
        _source.Position = _dataPosition;
        var span = _rawPacketBuffer.AsSpan(0, read);
        var header = OggPage.FindLastIndex(span);
        if (header == -1)
            return 0;
        var granule = OggPage.GetGranulePosition(_rawPacketBuffer);
        return granule;
    }

    public int ReadPacket(Span<byte> buffer)
    {
        while (_page is { } page)
        {
            if (AdvancePage(buffer, page, out var length))
                return length;
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
