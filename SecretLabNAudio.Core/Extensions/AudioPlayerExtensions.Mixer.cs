using SecretLabNAudio.Core.Extensions.Processors;

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
        /// <include file='../XmlDocs/Files.xml' path='doc/exception'/>
        /// <include file='../XmlDocs/Mixer.xml' path='doc/Player/remarks'/>
        /// <seealso cref="UseMixer(AudioPlayer,Action{Mixer},bool)"/>
        public AudioPlayer MixFile(string path, string inputName, ModifyChain? process, bool loop = false)
            => player.UseMixer(mixer => mixer.AddFileNamed(path, inputName, loop, process.Prepend(ProcessorChainExtensions.ToPlayerCompatible)));

        /// <inheritdoc cref="MixFile(AudioPlayer,string,bool,float)"/>
        public AudioPlayer MixFileSafe(string path, bool loop = false)
            => player.UseMixer(mixer => mixer.TryAddFileAnonymous(path, loop, ProcessorChainExtensions.ToPlayerCompatible));

        /// <returns>The player itself.</returns>
        public AudioPlayer MixFileSafe(string path, string inputName, bool loop = false)
            => player.UseMixer(mixer => mixer.TryAddFileNamed(path, inputName, loop));

        /// <returns>The player itself.</returns>
        public AudioPlayer MixShortClip(string name, bool loop = false)
            => player.UseMixer(mixer => mixer.AddShortClip(name, loop));

        /// <returns>The player itself.</returns>
        public AudioPlayer MixShortClip(string clipName, string inputName, bool loop = false)
            => player.UseMixer(mixer => mixer.AddShortClipNamed(clipName, inputName, loop));

        /// <summary>
        /// Adds an anonymous short clip input to the mixer. 
        /// </summary>
        /// <param name="name">The name of the clip.</param>
        /// <param name="loop">Whether to loop the input.</param>
        /// <returns>The player itself.</returns>
        /// <seealso cref="MixerExtensions.AddShortClipAnonymous"/>
        public AudioPlayer MixShortClipAnonymous(string name, bool loop = false)
            => player.UseMixer(mixer => mixer.AddShortClipAnonymous(name, loop));

        /// <summary>
        /// Removes all mixer inputs if the immediate provider is a <see cref="Mixer"/>.
        /// </summary>
        /// <returns>The player itself.</returns>
        /// <seealso cref="ImmediateProviderAs{T}"/>
        public AudioPlayer ClearMixerInputs()
        {
            player.Mixer?.RemoveAll();
            return player;
        }

    }

}
