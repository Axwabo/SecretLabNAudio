namespace SecretLabNAudio.Core.Encoded.OggOpus;

public static partial class PageReader
{

    extension(Stream stream)
    {

        private void ReadExactly(Span<byte> buffer, string exceptionMessage)
        {
            int start;
            int num;
            for (start = 0; start < buffer.Length; start += num)
            {
                num = stream.Read(buffer[start..]);
                if (num == 0)
                    throw new EndOfStreamException(exceptionMessage);
            }
        }

    }

}
