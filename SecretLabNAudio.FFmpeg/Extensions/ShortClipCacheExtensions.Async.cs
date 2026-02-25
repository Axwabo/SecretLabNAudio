using System.Linq;
using System.Threading.Tasks;
using SecretLabNAudio.Core.FileReading;
using SecretLabNAudio.Core.Providers;

namespace SecretLabNAudio.FFmpeg.Extensions;

public static partial class ShortClipCacheExtensions
{

    public static async Awaitable<RawSourceSampleProvider?> ReadWithFFmpegAsync(string path, TimeSpan? maxDuration)
    {
        try
        {
            await Awaitable.BackgroundThreadAsync();
            return ReadWithFFmpeg(path, maxDuration);
        }
        catch (Exception e)
        {
            Logger.Error(e);
            throw;
        }
    }

    public static async Awaitable<List<RawSourceSampleProvider?>> ReadWithFFmpegAsync(IEnumerable<string> paths, TimeSpan? maxDuration)
    {
        await Awaitable.MainThreadAsync();
        var tasks = new List<Task<RawSourceSampleProvider?>>();
        foreach (var path in paths)
        {
            var tcs = new TaskCompletionSource<RawSourceSampleProvider?>();
            var awaiter = ReadWithFFmpegAsync(path, maxDuration).GetAwaiter();
            awaiter.OnCompleted(() => tcs.SetResult(awaiter.GetResult())); // TODO catch
            tasks.Add(tcs.Task);
        }

        var acs = new AwaitableCompletionSource();
        _ = Task.WhenAll(tasks).ContinueWith(task =>
        {
            if (task.IsFaulted)
                acs.SetException(task.Exception);
            else if (task.IsCompleted)
                acs.SetCanceled();
            else
                acs.SetResult();
        });
        await acs.Awaitable;
        return tasks.Select(static e => e.IsCompletedSuccessfully ? e.Result : null).ToList();
    }

    extension(ShortClipCache)
    {

        public static Awaitable<List<RawSourceSampleProvider?>> AddAllWithFFmpegAsync(IEnumerable<string> paths, TimeSpan? maxDuration)
        {
            return ReadWithFFmpegAsync(paths, maxDuration);
        }

    }

}
