namespace SecretLabNAudio.Core.Extensions.Processors;

/// <summary>
/// A delegate to modify a <see cref="ProcessorChain"/> that returns the passed chain itself.
/// These delegates can be <b>chain</b>ed.
/// </summary>
public delegate ProcessorChain ModifyChain(ProcessorChain chain);

/// <summary>Extensions for the <see cref="ModifyChain"/> delegate.</summary>
public static class ModifyChainExtensions
{

    /// <param name="current">The delegate to chain, or <see langword="null"/>.</param>
    extension(ModifyChain? current)
    {

        /// <summary>
        /// Creates a new delegate that applies the current modification, then the <paramref name="next"/> one.
        /// </summary>
        /// <param name="next">The modification to follow up with.</param>
        /// <returns><see langword="null"/> if both delegates are null, the delegate itself if only one is specified, or a new delegate if neither are null.</returns>
        [return: NotNullIfNotNull(nameof(current)), NotNullIfNotNull(nameof(next))]
        public ModifyChain? Then(ModifyChain? next)
            => current == null
                ? next
                : next == null
                    ? current
                    : chain => next(current(chain));

        /// <summary>
        /// Creates a new delegate that applies the <paramref name="previous"/> modification, then the current one.
        /// </summary>
        /// <param name="previous">The modification to execute first.</param>
        /// <returns><see langword="null"/> if both delegates are null, the delegate itself if only one is specified, or a new delegate if neither are null.</returns>
        [return: NotNullIfNotNull(nameof(current)), NotNullIfNotNull(nameof(previous))]
        public ModifyChain? Prepend(ModifyChain? previous) => previous.Then(current);

        /// <summary>
        /// Sets the volume of the chain before the current modification.
        /// </summary>
        /// <param name="volume">The volume to set.</param>
        /// <returns><see cref="Amplify"/>, then the current modification.</returns>
        /// <seealso cref="ProcessorChainExtensions.Volume"/>
        public ModifyChain PreAmplify(float volume) => Amplify(volume).Then(current);

    }

    /// <summary>Static extension methods.</summary>
    extension(ModifyChain)
    {

        /// <summary>
        /// Creates a new <see cref="ModifyChain"/> that sets the volume of the chain.
        /// </summary>
        /// <param name="volume">The volume to set.</param>
        /// <returns>A new delegate that sets the volume.</returns>
        /// <seealso cref="ProcessorChainExtensions.Volume"/>
        public static ModifyChain Amplify(float volume) => chain => chain.Volume(volume);

        /// <summary>
        /// If <paramref name="volume"/> is not 1, creates a new <see cref="ModifyChain"/> that sets the volume of the chain.
        /// </summary>
        /// <param name="volume">The volume to set.</param>
        /// <returns><see langword="null"/> if <paramref name="volume"/> is approximately 1, a new delegate otherwise.</returns>
        /// <seealso cref="ProcessorChainExtensions.Volume"/>
        /// <seealso cref="Amplify"/>
        public static ModifyChain? AmplifyIfNot1(float volume) => Mathf.Approximately(1, volume) ? null : Amplify(volume);

    }

}
