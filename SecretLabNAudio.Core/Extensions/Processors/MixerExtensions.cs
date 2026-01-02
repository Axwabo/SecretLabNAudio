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

        /// <summary>
        /// Attempts to add an anonymous file input, avoiding nonexistent or unreadable files.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="loop">Whether to loop the file.</param>
        /// <param name="process">An optional <see cref="ModifyChain"/> specifying how to process the stream.</param>
        /// <returns>The mixer itself.</returns>
        /// <include file='../../XmlDocs/Mixer.xml' path='doc/AddProcess/exception'/>
        /// <remarks>Nothing happens if the processor couldn't be created.</remarks>
        /// <seealso cref="StreamProcessorExtensions.TryCreateFromFile"/>
        public Mixer TryAddFileAnonymous(string path, bool loop = false, ModifyChain? process = null)
            => StreamAudioProcessor.TryCreateFromFile(path, loop, out var processor)
                ? mixer.AddAnonymous(processor.Process(process))
                : mixer;

        /// <summary>
        /// Attempts to add a named file input, avoiding nonexistent or unreadable files.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="inputName">The name of the mixer input.</param>
        /// <param name="loop">Whether to loop the file.</param>
        /// <param name="process">An optional <see cref="ModifyChain"/> specifying how to process the stream.</param>
        /// <returns>The mixer itself.</returns>
        /// <include file='../../XmlDocs/Mixer.xml' path='doc/AddProcess/exception'/>
        /// <remarks>Nothing happens if the processor couldn't be created.</remarks>
        /// <seealso cref="StreamProcessorExtensions.TryCreateFromFile"/>
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

        /// <summary>
        /// Removes all inputs whose provider is of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to match.</typeparam>
        /// <returns>The mixer itself.</returns>
        public Mixer RemoveAllByImmediateType<T>()
            => mixer.RemoveAll(static e => e.Provider is T);

        /// <summary>
        /// Removes all inputs whose source type is of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of source to match.</typeparam>
        /// <remarks>
        /// If the provider is an <see cref="IAudioProcessor"/>, <see cref="AudioProcessorExtensions.TryGetSourceAs"/> will be used for checking.
        /// Otherwise, the type is directly checked against <typeparamref name="T"/>.
        /// </remarks>
        /// <returns>The mixer itself.</returns>
        public Mixer RemoveAllBySourceType<T>()
            => mixer.RemoveAll(static e => e.Provider is IAudioProcessor provider ? provider.TryGetSourceAs(out T? _) : e.Provider is T);

        /// <summary>
        /// Removes all inputs whose source is a <see cref="RawSourceSampleProvider"/>.
        /// </summary>
        /// <returns>The mixer itself.</returns>
        /// <seealso cref="RemoveAllBySourceType"/>
        public Mixer RemoveAllShortClips()
            => mixer.RemoveAllBySourceType<RawSourceSampleProvider>();

        /// <summary>
        /// Removes all inputs whose source is a <see cref="StreamAudioProcessor"/>.
        /// </summary>
        /// <returns>The mixer itself.</returns>
        /// <seealso cref="RemoveAllBySourceType"/>
        public Mixer RemoveAllStreamProcessors()
            => mixer.RemoveAllBySourceType<StreamAudioProcessor>();

        /// <summary><inheritdoc cref="Mixer.RemoveAllByName" path="summary"/></summary>
        /// <param name="name">The name to match.</param>
        /// <param name="ignoreCase">Whether to ignore case.</param>
        /// <remarks>
        /// If <paramref name="ignoreCase"/> is <see langword="true"/>, <see cref="StringComparison.OrdinalIgnoreCase"/> is used;
        /// otherwise, <see cref="StringComparison.Ordinal"/> is used.
        /// </remarks>
        /// <returns>The mixer itself.</returns>
        public Mixer RemoveAllByName(string name, bool ignoreCase = true)
            => mixer.RemoveAllByName(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

    }

}
