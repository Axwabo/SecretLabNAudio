using System.Diagnostics;
using System.Runtime.InteropServices;
using SecretLabNAudio.Core;
using SecretLabNAudio.Core.Processors;

namespace SecretLabNAudio.FFmpeg;

public class FFmpegAudioProcessor : IAudioProcessor
{

    private readonly Process _process;
    private bool _exitRequested;

    public static FFmpegAudioProcessor Create(string path)
    {
        var process = Process.Start(new ProcessStartInfo("ffmpeg", $"-i \"{path}\" -ar 48000 -ac 1 -f f32le -")
        {
            RedirectStandardOutput = true,
            CreateNoWindow = true,
            UseShellExecute = false
        });
        return process == null ? throw new InvalidOperationException("watafak bro") : new FFmpegAudioProcessor(process);
    }

    private FFmpegAudioProcessor(Process process) => _process = process;

    public int Read(float[] buffer, int offset, int count)
    {
        var floatSpan = buffer.AsSpan(offset, count);
        var byteSpan = MemoryMarshal.Cast<float, byte>(floatSpan);
        return sizeof(float) * _process.StandardOutput.BaseStream.Read(byteSpan);
    }

    public WaveFormat WaveFormat => AudioPlayer.SupportedFormat;

    public void Dispose()
    {
        if (!_exitRequested)
            _process.CloseMainWindow();
        _exitRequested = true;
        _process.Dispose();
    }

}
