namespace SecretLabNAudio.Core.Extensions.Processors;

public static class ProcessorInputExtensions
{

    extension<T>(ICollection<T> inputs) where T : ProcessorLayer
    {

        public void DisposeAllAndClear()
        {
            foreach (var input in inputs)
                input.Dispose();
            inputs.Clear();
        }

    }

    internal static void Dispose(this ProcessorLayer layer)
    {
        if (layer.IsOwned)
            (layer.Provider as IDisposable)?.Dispose();
    }

}
