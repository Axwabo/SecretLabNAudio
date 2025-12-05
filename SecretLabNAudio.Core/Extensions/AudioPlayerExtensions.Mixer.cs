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
        /// <remarks>
        /// If the <see cref="AudioPlayer.SampleProvider"/> is not a mixer, it will be replaced with one.
        /// The existing provider (if present) will be added as an anonymous mixer input.
        /// </remarks>
        /// <seealso cref="UseMixer(AudioPlayer,bool)"/>
        public AudioPlayer Mix(ISampleProvider input, bool isOwned = true)
            => player.UseMixer(mixer => mixer.AddAnonymous(input, isOwned));

        /// <summary>
        /// Adds a named input to the mixer.
        /// </summary>
        /// <param name="input">The provider to add.</param>
        /// <param name="inputName">The name of the mixer input.</param>
        /// <param name="isOwned">Whether to dispose of the input when the mixer is disposed or if the input is removed.</param>
        /// <returns>The player itself.</returns>
        /// <remarks><inheritdoc cref="Mix(AudioPlayer,ISampleProvider,bool)" path="remarks"/></remarks>
        /// <seealso cref="UseMixer(AudioPlayer,bool)"/>
        public AudioPlayer Mix(ISampleProvider input, string inputName, bool isOwned = true)
            => player.UseMixer(mixer => mixer.AddNamed(input, inputName, isOwned));

        /// <summary>
        /// Adds an anonymous file input to the mixer.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="loop">Whether to loop the file.</param>
        /// <param name="volume">The volume of the input.</param>
        /// <returns>The player itself.</returns>
        /// <include file='../XmlDocs/Files.xml' path='doc/NotSupported/exception'/>
        /// <exception cref="FileNotFoundException">Thrown if the file does not exist.</exception>
        /// <seealso cref="UseMixer(AudioPlayer,bool)"/>
        public AudioPlayer MixFile(string path, bool loop = false, float volume = 1)
            => player.MixFile(path, volume.ModifyChainIfNot1, loop);

        /// <summary>
        /// Adds a named file input to the mixer.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="inputName">The name of the mixer input.</param>
        /// <param name="loop">Whether to loop the file.</param>
        /// <param name="volume">The volume of the input.</param>
        /// <returns>The player itself.</returns>
        /// <include file='../XmlDocs/Files.xml' path='doc/NotSupported/exception'/>
        public AudioPlayer MixFile(string path, string inputName, bool loop = false, float volume = 1)
            => player.MixFile(path, inputName, volume.ModifyChainIfNot1, loop);

        /// <summary>
        /// Adds an anonymous file input to the mixer with optional processing using a <see cref="ProcessorChain"/>.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="process">An optional <see cref="ModifyChain"/> specifying how to process the stream.</param>
        /// <param name="loop">Whether to loop the file.</param>
        /// <returns>The player itself.</returns>
        /// <include file='../XmlDocs/Files.xml' path='doc/NotSupported/exception'/>
        public AudioPlayer MixFile(string path, ModifyChain? process, bool loop = false)
            => player.UseMixer(mixer => mixer.AddFileAnonymous(path, loop, process));

        /// <summary>
        /// Adds a named file input to the mixer with optional processing using a <see cref="ProcessorChain"/>.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="inputName">The name of the mixer input.</param>
        /// <param name="process">An optional <see cref="ModifyChain"/> specifying how to process the stream.</param>
        /// <param name="loop">Whether to loop the file.</param>
        /// <returns>The player itself.</returns>
        /// <include file='../XmlDocs/Files.xml' path='doc/NotSupported/exception'/>
        public AudioPlayer MixFile(string path, string inputName, ModifyChain? process, bool loop = false)
            => player.UseMixer(mixer => mixer.AddFileNamed(path, inputName, loop, process));

        /// <inheritdoc cref="MixFile(AudioPlayer,string,bool,float)"/>
        public AudioPlayer MixFileSafe(string path, bool loop = false)
            => player.UseMixer(mixer => mixer.TryAddFileAnonymous(path, loop));

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
        /// Adds an anonymous short clip to the mixer. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="loop"></param>
        /// <returns>The player itself.</returns>
        public AudioPlayer MixShortClipAnonymous(string name, bool loop = false)
            => player.UseMixer(mixer => mixer.AddShortClipAnonymous(name, loop));

    }

}
