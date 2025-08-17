using NAudio.Wave.SampleProviders;

namespace SecretLabNAudio.Core.Extensions;

public static partial class AudioPlayerExtensions
{

    /// <summary>
    /// Adds a mixer input to the <see cref="AudioPlayer"/>.
    /// </summary>
    /// <param name="player">The player to add the mixer input to.</param>
    /// <param name="input">The input to add to the mixer.</param>
    /// <returns>The <paramref name="player"/> itself.</returns>
    /// <remarks>
    /// The method changes the provider to a new <see cref="MixingSampleProvider"/> only if it's not already that type.
    /// The input is run through <see cref="SampleProviderExtensions.ToPlayerCompatible"/> to ensure the correct wave format.
    /// </remarks>
    /// <seealso cref="RemoveMixerInput(SecretLabNAudio.Core.AudioPlayer,NAudio.Wave.ISampleProvider)"/>
    /// <seealso cref="MixingSampleProvider.AddMixerInput(ISampleProvider)"/>
    public static AudioPlayer AddMixerInput(this AudioPlayer player, ISampleProvider input)
    {
        player.SampleProvider = player.SampleProvider == null
            ? new MixingSampleProvider([input.ToPlayerCompatible()])
            : player.SampleProvider.MixWith(input.ToPlayerCompatible());
        return player;
    }

    /// <summary>
    /// Removes a mixer input from the <see cref="AudioPlayer"/>.
    /// </summary>
    /// <param name="player">The player to remove the mixer input from.</param>
    /// <param name="input">The input to remove from the mixer.</param>
    /// <returns>The <paramref name="player"/> itself.</returns>
    /// <remarks>No operation is performed if the <see cref="AudioPlayer.SampleProvider"/> is not a <see cref="MixingSampleProvider"/>.</remarks>
    /// <seealso cref="AddMixerInput(SecretLabNAudio.Core.AudioPlayer,ISampleProvider)"/>
    /// <seealso cref="MixingSampleProvider.RemoveMixerInput"/>
    public static AudioPlayer RemoveMixerInput(this AudioPlayer player, ISampleProvider input)
    {
        player.ProviderAs<MixingSampleProvider>()?.RemoveMixerInput(input);
        return player;
    }

    /// <summary>
    /// Removes all mixer inputs from the <see cref="AudioPlayer"/>.
    /// </summary>
    /// <param name="player">The player to remove all mixer inputs from.</param>
    /// <returns>The <paramref name="player"/> itself.</returns>
    /// <remarks>No operation is performed if the <see cref="AudioPlayer.SampleProvider"/> is not a <see cref="MixingSampleProvider"/>.</remarks>
    /// <seealso cref="RemoveMixerInput(AudioPlayer,ISampleProvider)"/>
    /// <seealso cref="MixingSampleProvider.RemoveAllMixerInputs"/>
    public static AudioPlayer RemoveAllMixerInputs(this AudioPlayer player)
    {
        player.ProviderAs<MixingSampleProvider>()?.RemoveAllMixerInputs();
        return player;
    }

    /// <inheritdoc cref="AddMixerInput(AudioPlayer,ISampleProvider)"/>
    public static AudioPlayer AddMixerInput(this AudioPlayer player, IWaveProvider input)
        => player.AddMixerInput(input.ToSampleProvider());

}
