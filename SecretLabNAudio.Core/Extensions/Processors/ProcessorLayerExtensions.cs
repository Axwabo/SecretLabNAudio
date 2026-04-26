namespace SecretLabNAudio.Core.Extensions.Processors;

/// <summary>
/// Extension methods for <see cref="ProcessorLayer"/>s.
/// </summary>
public static class ProcessorLayerExtensions
{

    /// <param name="inputs">The inputs to dispose of.</param>
    /// <typeparam name="T">The type of processor layer.</typeparam>
    extension<T>(ICollection<T> inputs) where T : ProcessorLayer
    {

        /// <summary>Disposes of all owned layers and clears the collection.</summary>
        /// <seealso cref="ProcessorLayer.IsOwned"/>
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
