namespace SecretLabNAudio.Core.FileReading;

/// <summary>Stores results returned by an <see cref="IAudioReaderFactory"/>.</summary>
/// <param name="Stream">The <see cref="WaveStream"/>, or <see langword="null"/> if no stream was created.</param>
/// <param name="Provider">The <see cref="ISampleProvider"/>, or <see langword="null"/> if no provider was created.</param>
public readonly record struct AudioReaderFactoryResult(WaveStream? Stream, ISampleProvider? Provider)
{

    /// <summary>
    /// Stores the <see cref="WaveStream"/> and a corresponding <see cref="ISampleProvider"/>.
    /// </summary>
    /// <param name="stream">The <see cref="WaveStream"/> to store.</param>
    /// <returns>An <see cref="AudioReaderFactoryResult"/> with both <see cref="Stream"/> and <see cref="Provider"/> set.</returns>
    /// <seealso cref="WaveExtensionMethods.ToSampleProvider"/>
    public static implicit operator AudioReaderFactoryResult(WaveStream stream) => new(stream, stream.ToSampleProvider());

}
