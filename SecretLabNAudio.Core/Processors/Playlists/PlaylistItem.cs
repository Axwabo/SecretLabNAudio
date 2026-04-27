using SecretLabNAudio.Core.Extensions.Processors;
using SecretLabNAudio.Core.FileReading;

namespace SecretLabNAudio.Core.Processors.Playlists;

public abstract record PlaylistItem(string? Name = null)
{

    public abstract ISampleProvider CreateProvider(int sampleRate, int channels);

}

public sealed record FilePlaylistItem(string FilePath) : PlaylistItem(Path.GetFileNameWithoutExtension(FilePath))
{

    public override ISampleProvider CreateProvider(int sampleRate, int channels) => CreateAudioProcessor.FromFile(FilePath);

}

public sealed record ShortClipPlaylistItem(ClipName ClipName) : PlaylistItem(ClipName.ToString())
{

    public override ISampleProvider CreateProvider(int sampleRate, int channels) => ShortClipCache.Get(ClipName);

}

public sealed record ProcessedPlaylistItem(PlaylistItem Inner, ModifyChain Process) : PlaylistItem(Inner.Name)
{

    public override ISampleProvider CreateProvider(int sampleRate, int channels)
    {
        var provider = Inner.CreateProvider(sampleRate, channels);
        var chain = ProviderToProcessor.SampleProviderToProcessor(provider, true).ToChain().ToFormat(sampleRate, channels);
        Process(chain);
        return chain;
    }

}
