using SecretLabNAudio.Core.FileReading;
using SecretLabNAudio.Core.Providers;

namespace SecretLabNAudio.Core.Extensions.Processors;

/// <summary>Extension methods for the <see cref="Mixer"/> class.</summary>
public static class MixerExtensions
{

    /// <param name="mixer">The mixer to modify.</param>
    extension(Mixer mixer)
    {

        /// <summary>
        /// Adds an anonymous file input.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="loop">Whether to loop the file.</param>
        /// <param name="process">An optional <see cref="ModifyChain"/> specifying how to process the stream.</param>
        /// <returns>The mixer itself.</returns>
        /// <include file='../../XmlDocs/Files.xml' path='doc/exception'/>
        /// <include file='../../XmlDocs/Mixer.xml' path='doc/AddProcess/exception'/>
        public Mixer AddFileAnonymous(string path, bool loop = false, ModifyChain? process = null)
            => mixer.AddAnonymous(StreamAudioProcessor.CreateFromFile(path, loop).Process(process));

        /// <summary>
        /// Adds a named file input.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="inputName">The name of the mixer input.</param>
        /// <param name="loop">Whether to loop the file.</param>
        /// <param name="process">An optional <see cref="ModifyChain"/> specifying how to process the stream.</param>
        /// <returns>The mixer itself.</returns>
        /// <include file='../../XmlDocs/Files.xml' path='doc/exception'/>
        /// <include file='../../XmlDocs/Mixer.xml' path='doc/AddProcess/exception'/>
        public Mixer AddFileNamed(string path, string inputName, bool loop = false, ModifyChain? process = null)
            => mixer.AddNamed(StreamAudioProcessor.CreateFromFile(path, loop).Process(process), inputName);

        public Mixer TryAddFileAnonymous(string path, bool loop = false, ModifyChain? process = null)
            => StreamAudioProcessor.TryCreateFromFile(path, loop, out var processor)
                ? mixer.AddAnonymous(processor.Process(process))
                : mixer;

        public Mixer TryAddFileNamed(string path, string inputName, bool loop = false, ModifyChain? process = null)
            => StreamAudioProcessor.TryCreateFromFile(path, loop, out var processor)
                ? mixer.AddNamed(processor.Process(process), inputName)
                : mixer;

        /// <summary>
        /// Adds an anonymous short clip input.
        /// </summary>
        /// <param name="name">The name of the short clip.</param>
        /// <param name="loop">Whether to loop the clip.</param>
        /// <param name="process">An optional <see cref="ModifyChain"/> specifying how to process the clip.</param>
        /// <returns>The mixer itself.</returns>
        /// <include file='../../XmlDocs/Mixer.xml' path='doc/AddClip/exception'/>
        /// <seealso cref="ShortClipCache.TryGet"/>
        public Mixer AddShortClipAnonymous(string name, bool loop = false, ModifyChain? process = null)
            => ShortClipCache.TryGet(name, out var provider)
                ? mixer.AddAnonymous(provider.WithLoop(loop).Process(process), false)
                : mixer;

        /// <summary>
        /// Adds a short clip input with the mixer input name set to <seealso cref="RawSourceSampleProvider.ClipName"/>.
        /// </summary>
        /// <param name="name">The name of the short clip.</param>
        /// <param name="loop">Whether to loop the clip.</param>
        /// <param name="process">An optional <see cref="ModifyChain"/> specifying how to process the clip.</param>
        /// <returns>The mixer itself.</returns>
        /// <include file='../../XmlDocs/Mixer.xml' path='doc/AddClip/exception'/>
        /// <seealso cref="ShortClipCache.TryGet"/>
        public Mixer AddShortClip(string name, bool loop = false, ModifyChain? process = null)
            => ShortClipCache.TryGet(name, out var provider)
                ? mixer.AddNamed(provider.WithLoop(loop).Process(process), provider.ClipName!, false)
                : mixer;

        /// <summary>
        /// Adds a named short clip input.
        /// </summary>
        /// <param name="clipName">The name of the short clip.</param>
        /// <param name="inputName">The name of the mixer input.</param>
        /// <param name="loop">Whether to loop the clip.</param>
        /// <param name="process">An optional <see cref="ModifyChain"/> specifying how to process the clip.</param>
        /// <returns>The mixer itself.</returns>
        /// <include file='../../XmlDocs/Mixer.xml' path='doc/AddClip/exception'/>
        /// <seealso cref="ShortClipCache.TryGet"/>
        public Mixer AddShortClipNamed(string clipName, string inputName, bool loop = false, ModifyChain? process = null)
            => ShortClipCache.TryGet(clipName, out var provider)
                ? mixer.AddNamed(provider.WithLoop(loop).Process(process), inputName, false)
                : mixer;

        public Mixer RemoveAllByImmediateType<T>()
            => mixer.RemoveAll(static e => e.Provider is T);

        public Mixer RemoveAllBySourceType<T>()
            => mixer.RemoveAll(static e => e.Provider is IAudioProcessor provider ? provider.TryGetSourceAs(out T? _) : e.Provider is T);

        public Mixer RemoveAllShortClips()
            => mixer.RemoveAllByImmediateType<RawSourceSampleProvider>();

        public Mixer RemoveAllStreamProcessors()
            => mixer.RemoveAllBySourceType<StreamAudioProcessor>();

        public Mixer RemoveAllByName(string name, bool ignoreCase = true)
            => mixer.RemoveAllByName(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

    }

}
