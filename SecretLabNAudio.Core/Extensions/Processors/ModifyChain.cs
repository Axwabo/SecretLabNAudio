namespace SecretLabNAudio.Core.Extensions.Processors;

/// <summary>
/// A delegate to modify a <see cref="ProcessorChain"/> that returns the passed chain itself.
/// These delegates can be <b>chain</b>ed.
/// </summary>
public delegate ProcessorChain ModifyChain(ProcessorChain chain);

public static class ModifyChainExtensions
{

    extension(ModifyChain? process)
    {

        [return: NotNullIfNotNull(nameof(process)), NotNullIfNotNull(nameof(next))]
        public ModifyChain? Then(ModifyChain? next)
            => process == null
                ? next
                : next == null
                    ? process
                    : chain => next(process(chain));

        [return: NotNullIfNotNull(nameof(process)), NotNullIfNotNull(nameof(previous))]
        public ModifyChain? Prepend(ModifyChain? previous) => previous.Then(process);

        public ModifyChain PreAmplify(float volume) => Amplify(volume).Then(process);

    }

    extension(ModifyChain)
    {

        public static ModifyChain Amplify(float volume) => chain => chain.Volume(volume);

        public static ModifyChain? AmplifyIfNot1(float volume) => Mathf.Approximately(1, volume)
            ? null
            : chain => chain.Volume(volume);

    }

}
