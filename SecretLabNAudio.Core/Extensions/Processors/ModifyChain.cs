namespace SecretLabNAudio.Core.Extensions.Processors;

public delegate ProcessorChain ModifyChain(ProcessorChain chain);

public static class ModifyChainExtensions
{

    extension(ModifyChain? process)
    {

        [return: NotNullIfNotNull(nameof(process)), NotNullIfNotNull(nameof(other))]
        public ModifyChain? Then(ModifyChain? other)
            => process == null
                ? other
                : other == null
                    ? process
                    : chain => other(process(chain));

        public ModifyChain PreAmplify(float volume) => volume.ModifyChain.Then(process);

    }

    extension(float volume)
    {

        public ModifyChain? ModifyChainIfNot1 => Mathf.Approximately(1, volume)
            ? null
            : chain => chain.Volume(volume);

        public ModifyChain ModifyChain => chain => chain.Volume(volume);

    }

}
