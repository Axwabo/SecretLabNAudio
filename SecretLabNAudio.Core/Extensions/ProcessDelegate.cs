using SecretLabNAudio.Core.Extensions.Processors;

namespace SecretLabNAudio.Core.Extensions;

public delegate ProcessorChain Process(ProcessorChain chain);

public static class ProcessDelegate
{

    extension(Process? process)
    {

        public Process? Then(Process? other)
            => process == null
                ? other
                : other == null
                    ? process
                    : chain => other(process(chain));

        public Process? PreAmplify(float volume) => volume.AmplifyProcessorChain.Then(process);

    }

    extension(float volume)
    {

        public Process? AmplifyProcessorChain => Mathf.Approximately(1, volume)
            ? null
            : chain => ProcessorChainExtensions.Volume(chain, volume);

    }

}
