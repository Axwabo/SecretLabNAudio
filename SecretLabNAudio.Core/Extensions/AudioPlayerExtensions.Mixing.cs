using System.Linq;
using NAudio.Wave.SampleProviders;
using SecretLabNAudio.Core.Extensions.Processors;
using SecretLabNAudio.Core.FileReading;
using SecretLabNAudio.Core.Providers;

namespace SecretLabNAudio.Core.Extensions;

public static partial class AudioPlayerExtensions
{

    /// <param name="player">The player to add the mixer input to.</param>
    extension(AudioPlayer player)
    {

        /// <summary>
        /// Adds a mixer input to the <see cref="AudioPlayer"/>.
        /// </summary>
        /// <param name="input">The input to add to the mixer.</param>
        /// <returns>The player itself.</returns>
        /// <remarks>
        /// The method changes the provider to a new <see cref="MixingSampleProvider"/> only if it's not already that type.
        /// The input is run through <see cref="SampleProviderExtensions.ToPlayerCompatible"/> to ensure the correct wave format.
        /// </remarks>
        /// <seealso cref="RemoveMixerInput(SecretLabNAudio.Core.AudioPlayer,NAudio.Wave.ISampleProvider)"/>
        /// <seealso cref="MixingSampleProvider.AddMixerInput(ISampleProvider)"/>
        [Obsolete($"Use {nameof(Mix)} instead.", true)]
        public AudioPlayer AddMixerInput(ISampleProvider input)
        {
            player.SampleProvider = player.SampleProvider == null
                ? new MixingSampleProvider([input.ToPlayerCompatible()])
                : player.SampleProvider.MixWith(input.ToPlayerCompatible());
            return player;
        }

        /// <summary>
        /// Removes a mixer input from the <see cref="AudioPlayer"/>.
        /// </summary>
        /// <param name="input">The input to remove from the mixer.</param>
        /// <returns>The player itself.</returns>
        /// <remarks>No operation is performed if the <see cref="AudioPlayer.SampleProvider"/> is not a <see cref="MixingSampleProvider"/>.</remarks>
        /// <seealso cref="AddMixerInput(SecretLabNAudio.Core.AudioPlayer,ISampleProvider)"/>
        /// <seealso cref="MixingSampleProvider.RemoveMixerInput"/>
        [Obsolete("", true)] // TODO: add error message
        public AudioPlayer RemoveMixerInput(ISampleProvider input)
        {
            player.ProviderAs<MixingSampleProvider>()?.RemoveMixerInput(input);
            return player;
        }

        /// <summary>
        /// Removes all mixer inputs from the <see cref="AudioPlayer"/>.
        /// </summary>
        /// <returns>The player itself.</returns>
        /// <remarks>No operation is performed if the <see cref="AudioPlayer.SampleProvider"/> is not a <see cref="MixingSampleProvider"/>.</remarks>
        /// <seealso cref="RemoveMixerInput(AudioPlayer,ISampleProvider)"/>
        /// <seealso cref="MixingSampleProvider.RemoveAllMixerInputs"/>
        [Obsolete($"Use {nameof(ClearMixerInputs)} instead.", true)]
        public AudioPlayer RemoveAllMixerInputs()
        {
            player.ProviderAs<MixingSampleProvider>()?.RemoveAllMixerInputs();
            return player;
        }

        /// <summary>
        /// Removes all <see cref="RawSourceSampleProvider"/> inputs which have the <see cref="RawSourceSampleProvider.ClipName"/> property equal to <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name to search for.</param>
        /// <param name="removed">The number of removed inputs.</param>
        /// <param name="trimExtension">Whether to trim the file extension from the name.</param>
        /// <param name="ignoreCase">Whether to ignore case when comparing the names.</param>
        /// <returns>The player itself.</returns>
        [Obsolete("", true)] // TODO: add error message
        public AudioPlayer RemoveMixerInputsByName(string name, out int removed, bool trimExtension = true, bool ignoreCase = true)
        {
            if (player.SampleProvider is not MixingSampleProvider mixing)
            {
                removed = 0;
                return player;
            }

            if (trimExtension)
                name = Path.ChangeExtension(name, null);
            var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            var matches = mixing.MixerInputs
                .Where(e => e switch
                {
                    LoopingRawSampleProvider looping => name.Equals(looping.Provider.ClipName, comparison),
                    RawSourceSampleProvider raw => name.Equals(raw.ClipName, comparison),
                    _ => false
                })
                .ToArray();
            foreach (var provider in matches)
                mixing.RemoveMixerInput(provider);
            removed = matches.Length;
            return player;
        }

        /// <summary>
        /// Removes all <see cref="RawSourceSampleProvider"/> inputs which have the <see cref="RawSourceSampleProvider.ClipName"/> property equal to <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name to search for.</param>
        /// <param name="trimExtension">Whether to trim the file extension from the name.</param>
        /// <param name="ignoreCase">Whether to ignore case when comparing the names.</param>
        /// <returns>The player itself.</returns>
        [Obsolete("", true)] // TODO: add error message
        public AudioPlayer RemoveMixerInputsByName(string name, bool trimExtension = true, bool ignoreCase = true)
            => player.RemoveMixerInputsByName(name, out _, trimExtension, ignoreCase);

        /// <inheritdoc cref="AddMixerInput(AudioPlayer,ISampleProvider)"/>
        [Obsolete($"Convert the provider to a processor via {nameof(ProviderToProcessor)}.{nameof(ProviderToProcessor.ToCompatibleChain)}, and call {nameof(Mix)} instead.", true)]
        public AudioPlayer AddMixerInput(IWaveProvider input)
            => player.AddMixerInput(input.ToSampleProvider());

        /// <summary>
        /// Adds a <see cref="ShortClipCache">short clip</see> as a mixer input to the <see cref="AudioPlayer"/>.
        /// </summary>
        /// <param name="name">The key to search for.</param>
        /// <param name="trimExtension">Whether to trim the file extension from the inputted <paramref name="name"/>.</param>
        /// <returns>The player itself.</returns>
        [Obsolete($"Use {nameof(MixShortClip)} or {nameof(MixShortClipAnonymous)} instead.", true)]
        public AudioPlayer AddMixerShortClip(string name, bool trimExtension = true)
            => !ShortClipCache.TryGet(name, out var provider, trimExtension)
                ? player
                : player.AddMixerInput(provider);

    }

}
