using SecretLabNAudio.FFmpeg.Caches;

namespace SecretLabNAudio.FFmpeg.Extensions;

public static class CacheExtensions
{

    extension(OptimizeFor optimizeFor)
    {

        public string Extension => optimizeFor == OptimizeFor.FileSize ? "ogg" : "wav";

    }

}
