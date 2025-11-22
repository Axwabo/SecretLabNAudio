using System.Collections.Generic;
using SecretLabNAudio.Core.Processors;

namespace SecretLabNAudio.Core.Extensions;

public static class ProcessorInputExtensions
{

    extension<T>(ICollection<T> inputs) where T : ProcessorInput
    {

        public void DisposeAllAndClear()
        {
            foreach (var input in inputs)
                if (input is {IsOwned: true, Provider: IDisposable disposable})
                    disposable.Dispose();
            inputs.Clear();
        }

    }

}
