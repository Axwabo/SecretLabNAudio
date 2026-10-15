using System.Runtime.InteropServices;
using SecretLabNAudio.Core.Extensions.Providers;
using SecretLabNAudio.Core.FileReading;
using SecretLabNAudio.Core.Providers;

namespace SecretLabNAudio.Core.Extensions.Processors;

/// <summary>
/// Extension methods for the <see cref="WaveStreamAudioProcessor"/> class.
/// </summary>
public static class StreamProcessorExtensions
{

    private const int BufferLength = 48000;

    /// <param name="processor">The processor to modify.</param>
    extension(IStreamAudioProcessor processor)
    {

        /// <summary>
        /// Sets the <see cref="IStreamAudioProcessor.Loop"/> property.
        /// </summary>
        /// <param name="loop">Whether to loop the processor.</param>
        /// <returns>The processor itself.</returns>
        public IStreamAudioProcessor WithLoop(bool loop)
        {
            processor.Loop = loop;
            return processor;
        }

        public RawSourceSampleProvider ReadPlayerCompatibleSamples() => processor.ReadSamples(NonProcessorExtensions.ToPlayerCompatible);

        public RawSourceSampleProvider ReadSamples(Func<IStreamAudioProcessor, ISampleProvider> toProvider)
        {
            var provider = toProvider(processor);
            var length = provider.WaveFormat.SampleCount(processor.TotalTime.TotalSeconds);
            var buffer = new float[length];
            var position = 0;
            do
            {
                var requested = Math.Min(length - position, BufferLength);
                var read = processor.Read(buffer, position, requested);
                position += read;
                if (read == 0)
                    return new RawSourceSampleProvider(buffer, position, provider.WaveFormat);
            }
            while (position < length);

            using var memory = new MemoryStream(position * sizeof(float));
            while (position != 0)
            {
                memory.Write(MemoryMarshal.Cast<float, byte>(buffer.AsSpan(0, position)));
                var requested = Math.Min(length, BufferLength);
                position = processor.Read(buffer, 0, requested);
            }

            var bytes = memory.GetBuffer().AsSpan(0, (int) memory.Length);
            var array = MemoryMarshal.Cast<byte, float>(bytes).ToArray();
            return new RawSourceSampleProvider(array, provider.WaveFormat);
        }

    }

    /// <summary>Methods to create an <see cref="IStreamAudioProcessor"/> from a file.</summary>
    extension(IStreamAudioProcessor)
    {

        /// <summary>
        /// Creates an <see cref="IStreamAudioProcessor"/> from a file.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="loop">Whether to loop the processor.</param>
        /// <returns>An audio processor that reads from the file.</returns>
        /// <include file="../../XmlDocs/Files.xml" path="doc/exception"/>
        /// <remarks>The underlying file stream is automatically disposed when the processor is disposed.</remarks>
        public static IStreamAudioProcessor CreateFromFile(string path, bool loop = false)
            => CreateAudioProcessor.FromFile(path).WithLoop(loop);

        /// <summary>
        /// Attempts to create an <see cref="IStreamAudioProcessor"/> from the given file path.
        /// </summary>
        /// <param name="path">The file path to read the audio from.</param>
        /// <param name="loop">Whether to loop the processor.</param>
        /// <param name="processor">
        /// The resulting <see cref="IStreamAudioProcessor"/> if successful.
        /// <see langword="null"/> if the file was not found, if no factory was found for the file type, or if the factory didn't return a <see cref="WaveStream"/>.
        /// </param>
        /// <returns>Whether a <see cref="IStreamAudioProcessor"/> was successfully created.</returns>
        /// <remarks>The underlying file stream is automatically disposed when the processor is disposed.</remarks>
        public static bool TryCreateFromFile(string path, bool loop, [NotNullWhen(true)] out IStreamAudioProcessor? processor)
        {
            if (!File.Exists(path) || !TryCreateAudioProcessor.FromFile(path, out var result))
            {
                processor = null;
                return false;
            }

            processor = result.WithLoop(loop);
            return true;
        }

    }

}
