using SecretLabNAudio.Core.Extensions.Processors;
using SecretLabNAudio.Core.FileReading;

namespace SecretLabNAudio.Core.Extensions;

public static partial class AudioPlayerExtensions
{

    /// <param name="player">The player to add the input to.</param>
    extension(AudioPlayer player)
    {

        /// <summary>
        /// Adds an anonymous input to the mixer.
        /// </summary>
        /// <param name="input">The provider to add.</param>
        /// <param name="isOwned">Whether to dispose of the input when the mixer is disposed or if the input is removed.</param>
        /// <returns>The player itself.</returns>
        /// <include file='../XmlDocs/Mixer.xml' path='doc/Player/remarks'/>
        /// <seealso cref="UseMixer(AudioPlayer,Action{Mixer},bool)"/>
        public AudioPlayer Mix(ISampleProvider input, bool isOwned = true)
            => player.UseMixer(mixer => mixer.AddAnonymous(input, isOwned));

        /// <summary>
        /// Adds a named input to the mixer.
        /// </summary>
        /// <param name="input">The provider to add.</param>
        /// <param name="inputName">The name of the mixer input.</param>
        /// <param name="isOwned">Whether to dispose of the input when the mixer is disposed or if the input is removed.</param>
        /// <returns>The player itself.</returns>
        /// <include file='../XmlDocs/Mixer.xml' path='doc/Player/remarks'/>
        /// <seealso cref="UseMixer(AudioPlayer,Action{Mixer},bool)"/>
        /// <seealso cref="MixerExtensions.AddFileNamed"/>
        public AudioPlayer Mix(ISampleProvider input, string inputName, bool isOwned = true)
            => player.UseMixer(mixer => mixer.AddNamed(input, inputName, isOwned));

        /// <summary>
        /// Adds an anonymous file input to the mixer.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="loop">Whether to loop the file.</param>
        /// <param name="volume">The volume of the input.</param>
        /// <returns>The player itself.</returns>
        /// <include file='../XmlDocs/Mixer.xml' path='doc/Player/remarks'/>
        /// <include file='../XmlDocs/Files.xml' path='doc/exception'/>
        /// <seealso cref="UseMixer(AudioPlayer,Action{Mixer},bool)"/>
        /// <seealso cref="MixerExtensions.AddFileAnonymous"/>
        public AudioPlayer MixFile(string path, bool loop = false, float volume = 1)
            => player.MixFile(path, ModifyChain.AmplifyIfNot1(volume), loop);

        /// <summary>
        /// Adds a named file input to the mixer.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="inputName">The name of the mixer input.</param>
        /// <param name="loop">Whether to loop the file.</param>
        /// <param name="volume">The volume of the input.</param>
        /// <returns>The player itself.</returns>
        /// <include file='../XmlDocs/Mixer.xml' path='doc/Player/remarks'/>
        /// <include file='../XmlDocs/Files.xml' path='doc/exception'/>
        /// <seealso cref="UseMixer(AudioPlayer,Action{Mixer},bool)"/>
        /// <seealso cref="MixerExtensions.AddFileNamed"/>
        public AudioPlayer MixFile(string path, string inputName, bool loop = false, float volume = 1)
            => player.MixFile(path, inputName, ModifyChain.AmplifyIfNot1(volume), loop);

        /// <summary>
        /// Adds an anonymous file input to the mixer with optional processing using a <see cref="ProcessorChain"/>.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="process">An optional <see cref="ModifyChain"/> specifying how to process the stream.</param>
        /// <param name="loop">Whether to loop the file.</param>
        /// <returns>The player itself.</returns>
        /// <include file='../XmlDocs/Mixer.xml' path='doc/Player/remarks'/>
        /// <include file='../XmlDocs/Files.xml' path='doc/exception'/>
        /// <seealso cref="UseMixer(AudioPlayer,Action{Mixer},bool)"/>
        /// <seealso cref="MixerExtensions.AddFileAnonymous"/>
        public AudioPlayer MixFile(string path, ModifyChain? process, bool loop = false)
            => player.UseMixer(mixer => mixer.AddFileAnonymous(path, loop, process.Prepend(ProcessorChainExtensions.ToPlayerCompatible)));

        /// <summary>
        /// Adds a named file input to the mixer with optional processing using a <see cref="ProcessorChain"/>.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="inputName">The name of the mixer input.</param>
        /// <param name="process">An optional <see cref="ModifyChain"/> specifying how to process the stream.</param>
        /// <param name="loop">Whether to loop the file.</param>
        /// <returns>The player itself.</returns>
        /// <include file='../XmlDocs/Mixer.xml' path='doc/Player/remarks'/>
        /// <include file='../XmlDocs/Files.xml' path='doc/exception'/>
        /// <seealso cref="UseMixer(AudioPlayer,Action{Mixer},bool)"/>
        /// <seealso cref="MixerExtensions.AddFileNamed"/>
        public AudioPlayer MixFile(string path, string inputName, ModifyChain? process, bool loop = false)
            => player.UseMixer(mixer => mixer.AddFileNamed(path, inputName, loop, process.Prepend(ProcessorChainExtensions.ToPlayerCompatible)));

        /// <summary>
        /// Attempts to add an anonymous file input to the mixer without throwing an exception if the file is not found or is not supported.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="loop">Whether to loop the file.</param>
        /// <param name="volume">The volume of the input.</param>
        /// <returns>The player itself.</returns>
        /// <include file='../XmlDocs/Mixer.xml' path='doc/Player/remarks'/>
        /// <seealso cref="UseMixer(AudioPlayer,Action{Mixer},bool)"/>
        /// <seealso cref="MixerExtensions.TryAddFileAnonymous"/>
        public AudioPlayer MixFileSafe(string path, bool loop = false, float volume = 1)
            => player.MixFileSafe(path, ModifyChain.AmplifyIfNot1(volume), loop);

        /// <summary>
        /// Attempts to add a named file input to the mixer without throwing an exception if the file is not found or is not supported.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="inputName">The name of the mixer input.</param>
        /// <param name="loop">Whether to loop the file.</param>
        /// <param name="volume">The volume of the input.</param>
        /// <returns>The player itself.</returns>
        /// <include file='../XmlDocs/Mixer.xml' path='doc/Player/remarks'/>
        /// <seealso cref="UseMixer(AudioPlayer,Action{Mixer},bool)"/>
        /// <seealso cref="MixerExtensions.TryAddFileNamed"/>
        public AudioPlayer MixFileSafe(string path, string inputName, bool loop = false, float volume = 1)
            => player.MixFileSafe(path, inputName, ModifyChain.AmplifyIfNot1(volume), loop);

        /// <summary>
        /// Attempts to add an anonymous file input to the mixer without throwing an exception if the file is not found or is not supported, with optional processing using a <see cref="ProcessorChain"/>.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="process">An optional <see cref="ModifyChain"/> specifying how to process the stream.</param>
        /// <param name="loop">Whether to loop the file.</param>
        /// <returns>The player itself.</returns>
        /// <include file='../XmlDocs/Mixer.xml' path='doc/Player/remarks'/>
        /// <seealso cref="UseMixer(AudioPlayer,Action{Mixer},bool)"/>
        /// <seealso cref="MixerExtensions.TryAddFileAnonymous"/>
        public AudioPlayer MixFileSafe(string path, ModifyChain? process, bool loop = false)
            => player.UseMixer(mixer => mixer.TryAddFileAnonymous(path, loop, process.Prepend(ProcessorChainExtensions.ToPlayerCompatible)));

        /// <summary>
        /// Attempts to add a named file input to the mixer without throwing an exception if the file is not found or is not supported, with optional processing using a <see cref="ProcessorChain"/>.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="inputName">The name of the mixer input.</param>
        /// <param name="process">An optional <see cref="ModifyChain"/> specifying how to process the stream.</param>
        /// <param name="loop">Whether to loop the file.</param>
        /// <returns>The player itself.</returns>
        /// <include file='../XmlDocs/Mixer.xml' path='doc/Player/remarks'/>
        /// <seealso cref="UseMixer(AudioPlayer,Action{Mixer},bool)"/>
        /// <seealso cref="MixerExtensions.TryAddFileNamed"/>
        public AudioPlayer MixFileSafe(string path, string inputName, ModifyChain? process, bool loop = false)
            => player.UseMixer(mixer => mixer.TryAddFileNamed(path, inputName, loop, process.Prepend(ProcessorChainExtensions.ToPlayerCompatible)));

        /// <summary>
        /// Adds a short clip input to the mixer (if a clip with the given name was registered).
        /// The mixer input's name will be set to the original clip name.
        /// </summary>
        /// <param name="name">The name of the clip.</param>
        /// <param name="loop">Whether to loop the clip.</param>
        /// <param name="volume">The volume of the input.</param>
        /// <returns>The player itself.</returns>
        /// <include file='../XmlDocs/Mixer.xml' path='doc/Player/remarks'/>
        /// <seealso cref="UseMixer(AudioPlayer,Action{Mixer},bool)"/>
        /// <seealso cref="MixerExtensions.AddShortClip"/>
        public AudioPlayer MixShortClip(ClipName name, bool loop = false, float volume = 1)
            => player.MixShortClip(name, ModifyChain.AmplifyIfNot1(volume), loop);

        /// <summary>
        /// Adds a short clip input to the mixer (if a clip with the given name was registered).
        /// The mixer input's name will be set to the original clip name.
        /// </summary>
        /// <param name="name">The name of the clip.</param>
        /// <param name="process">An optional <see cref="ModifyChain"/> specifying how to process the clip.</param>
        /// <param name="loop">Whether to loop the clip.</param>
        /// <returns>The player itself.</returns>
        /// <include file='../XmlDocs/Mixer.xml' path='doc/Player/remarks'/>
        /// <seealso cref="UseMixer(AudioPlayer,Action{Mixer},bool)"/>
        /// <seealso cref="MixerExtensions.AddShortClip"/>
        public AudioPlayer MixShortClip(ClipName name, ModifyChain? process, bool loop = false)
            => player.UseMixer(mixer => mixer.AddShortClip(name, loop, process));

        /// <summary>
        /// Adds a named short clip input to the mixer (if a clip with the given name was registered).
        /// </summary>
        /// <param name="clipName">The name of the clip.</param>
        /// <param name="inputName">The name of the mixer input.</param>
        /// <param name="loop">Whether to loop the clip.</param>
        /// <param name="volume">The volume of the input.</param>
        /// <returns>The player itself.</returns>
        /// <include file='../XmlDocs/Mixer.xml' path='doc/Player/remarks'/>
        /// <seealso cref="UseMixer(AudioPlayer,Action{Mixer},bool)"/>
        /// <seealso cref="MixerExtensions.AddShortClipNamed"/>
        public AudioPlayer MixShortClip(ClipName clipName, string inputName, bool loop = false, float volume = 1)
            => player.MixShortClip(clipName, inputName, ModifyChain.AmplifyIfNot1(volume), loop);

        /// <summary>
        /// Adds a named short clip input to the mixer (if a clip with the given name was registered).
        /// </summary>
        /// <param name="clipName">The name of the clip.</param>
        /// <param name="inputName">The name of the mixer input.</param>
        /// <param name="process">An optional <see cref="ModifyChain"/> specifying how to process the clip.</param>
        /// <param name="loop">Whether to loop the clip.</param>
        /// <returns>The player itself.</returns>
        /// <include file='../XmlDocs/Mixer.xml' path='doc/Player/remarks'/>
        /// <seealso cref="UseMixer(AudioPlayer,Action{Mixer},bool)"/>
        /// <seealso cref="MixerExtensions.AddShortClipNamed"/>
        public AudioPlayer MixShortClip(ClipName clipName, string inputName, ModifyChain? process, bool loop = false)
            => player.UseMixer(mixer => mixer.AddShortClipNamed(clipName, inputName, loop, process));

        /// <summary>
        /// Adds an anonymous short clip input to the mixer.
        /// </summary>
        /// <param name="name">The name of the clip.</param>
        /// <param name="loop">Whether to loop the input.</param>
        /// <param name="volume">The volume of the input.</param>
        /// <returns>The player itself.</returns>
        /// <include file='../XmlDocs/Mixer.xml' path='doc/Player/remarks'/>
        /// <seealso cref="UseMixer(AudioPlayer,Action{Mixer},bool)"/>
        /// <seealso cref="MixerExtensions.AddShortClipAnonymous"/>
        public AudioPlayer MixShortClipAnonymous(ClipName name, bool loop = false, float volume = 1)
            => player.MixShortClipAnonymous(name, ModifyChain.AmplifyIfNot1(volume), loop);

        /// <summary>
        /// Adds an anonymous short clip input to the mixer.
        /// </summary>
        /// <param name="name">The name of the clip.</param>
        /// <param name="loop">Whether to loop the input.</param>
        /// <param name="process">An optional <see cref="ModifyChain"/> specifying how to process the clip.</param>
        /// <returns>The player itself.</returns>
        /// <include file='../XmlDocs/Mixer.xml' path='doc/Player/remarks'/>
        /// <seealso cref="UseMixer(AudioPlayer,Action{Mixer},bool)"/>
        /// <seealso cref="MixerExtensions.AddShortClipAnonymous"/>
        public AudioPlayer MixShortClipAnonymous(ClipName name, ModifyChain? process, bool loop = false)
            => player.UseMixer(mixer => mixer.AddShortClipAnonymous(name, loop, process));

    }

    /// <param name="player">The player to remove input(s) from if the <see cref="ImmediateProviderAs{T}">immediate provider</see> is a <see cref="Mixer"/>.</param>
    extension(AudioPlayer player)
    {

        /// <inheritdoc cref="Mixer.RemoveAll()"/>
        /// <returns>The player itself.</returns>
        public AudioPlayer RemoveMixerInputs()
        {
            player.Mixer?.RemoveAll();
            return player;
        }

        /// <inheritdoc cref="Mixer.RemoveAll(Func{MixerInput,bool})"/>
        /// <returns>The player itself.</returns>
        public AudioPlayer RemoveMixerInputs(Func<MixerInput, bool> match)
        {
            player.Mixer?.RemoveAll(match);
            return player;
        }

        /// <include file='../XmlDocs/Mixer.xml' path='doc/RemoveAllByName/summary'/>
        /// <param name="name">The name to match.</param>
        /// <param name="ignoreCase">Whether to ignore case.</param>
        /// <include file='../XmlDocs/Mixer.xml' path='doc/RemoveAllByName/remarks'/>
        /// <returns>The player itself.</returns>
        public AudioPlayer RemoveNamedMixerInputs(string name, bool ignoreCase = true)
        {
            player.Mixer?.RemoveAllByName(name, ignoreCase);
            return player;
        }

        /// <include file='../XmlDocs/Mixer.xml' path='doc/RemoveAllShortClips/summary'/>
        /// <returns>The player itself.</returns>
        public AudioPlayer RemoveShortClipMixerInputs()
        {
            player.Mixer?.RemoveAllShortClips();
            return player;
        }

        /// <include file='../XmlDocs/Mixer.xml' path='doc/RemoveAllStreamProcessors/summary'/>
        /// <returns>The player itself.</returns>
        public AudioPlayer RemoveStreamProcessorMixerInputs()
        {
            player.Mixer?.RemoveAllStreamProcessors();
            return player;
        }

    }

}
