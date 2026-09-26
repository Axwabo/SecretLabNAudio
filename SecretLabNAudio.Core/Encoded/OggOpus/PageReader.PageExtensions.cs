namespace SecretLabNAudio.Core.Encoded.OggOpus;

public static partial class PageReader
{

    extension(OggPage? page)
    {

        public bool IsIDHeader => page is [{Length: >= 19} packet] && packet[..OpusHead.Length].SequenceEqual(OpusHead);

        public bool IsCommentHeader => page is [{Length: >= 8} packet, ..] && packet[..OpusTags.Length].SequenceEqual(OpusTags);

        public bool IsContinuation => page.HasValue && (page.Value.Flags & HeaderType.Continuation) != 0;

    }

}
