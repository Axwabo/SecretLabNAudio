using SecretLabNAudio.Core.Opus;
using SecretLabNAudio.Core.Opus.Ogg;

namespace SecretLabNAudio.HighPerformance.Extensions;

public static class PreEncodedExtensions
{

    extension(PreEncodedAudioPlayer player)
    {

        public PreEncodedAudioPlayer UseFile(string path, bool loop = false)
        {
            var file = File.OpenRead(path);
            if (OggOpusPacketProvider.TryCreate(file) is { } provider)
                return player.Use(provider);
            file.Dispose();
            throw new IOException("Not an Ogg Opus file");
        }

        public PreEncodedAudioPlayer Use(IOpusPacketProvider provider, bool isOwned = true)
        {
            player.PacketProvider = provider;
            return player;
        }

    }

}
