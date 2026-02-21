using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Utils;
using SecretLabNAudio.Core;
using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.Processors;

namespace SecretLabNAudio.FFmpeg;

public sealed class AsyncBufferedFFmpegAudioProcessor : IAudioProcessor
{

    [ThreadStatic]
    private static byte[]? _readBuffer;

    private bool _disposed;

    private bool _anyRead;

    private FFmpegSL? _processor;

    private readonly CircularBuffer _buffer;

    private readonly CancellationTokenSource _cts = new();

    public AsyncBufferedFFmpegAudioProcessor(string path, double capacitySeconds, bool endless)
    {
        _buffer = new CircularBuffer(AudioPlayer.SupportedFormat.SampleCount(capacitySeconds * sizeof(float)));
        Task.Factory.StartNew(ReadLoop, _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        return;

        void ReadLoop()
        {
            _processor = FFmpegSL.StartRaw($"-i \"{path}\" -ar 48000 -ac 1 -f f32le -")!; // TODO
            while (true)
            {
                if (_buffer.Count > _buffer.MaxLength * .75)
                {
                    Thread.Sleep(100);
                    _anyRead = true;
                }

                _readBuffer = BufferHelpers.Ensure(_readBuffer, 1920);
                var read = _processor.Stdout.BaseStream.Read(_readBuffer, 0, _readBuffer.Length);
                if (endless && read <= 0)
                    break;
                _buffer.Write(_readBuffer, 0, read);
            }
        }
    }

    public int Read(float[] buffer, int offset, int count)
    {
        if (!_anyRead)
        {
            buffer.AsSpan(offset, count).Clear();
            return count;
        }

        var bytes = count * sizeof(float);
        _readBuffer = BufferHelpers.Ensure(_readBuffer, bytes);
        var read = _buffer.Read(_readBuffer, 0, bytes);
        var floatsRead = read / sizeof(float);
        var floatSpan = MemoryMarshal.Cast<float, byte>(buffer.AsSpan(offset, floatsRead));
        _readBuffer[..read].CopyTo(floatSpan);
        return floatsRead;
    }

    public WaveFormat WaveFormat => AudioPlayer.SupportedFormat;

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        _processor?.Dispose();
        _processor = null;
        _cts.Cancel();
        _cts.Dispose();
    }

}
