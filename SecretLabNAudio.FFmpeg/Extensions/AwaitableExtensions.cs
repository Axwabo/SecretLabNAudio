using System.Linq;
using System.Threading.Tasks;

namespace SecretLabNAudio.FFmpeg.Extensions;

public static class AwaitableExtensions
{

    extension<T>(Awaitable<T> awaitable)
    {

        public async Task<T> AsTask() => await awaitable;

    }

    extension(Awaitable)
    {

        public static async Awaitable<T[]> WhenAll<T>(params IEnumerable<Awaitable<T>> awaitables)
            => await Task.WhenAll(awaitables.Select(static e => e.AsTask()));

    }

}
