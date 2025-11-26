using NAudio.Wave.SampleProviders;
using SecretLabNAudio.Core.Extensions.Processors;
using SecretLabNAudio.Core.Processors;

namespace SecretLabNAudio.Demo;

public sealed class DiscJockeyProcessor : IAudioProcessor
{

    private readonly StreamAudioProcessor _stream;

    private readonly ProcessorChain _music;
    private readonly ProcessorChain _voice;

    private readonly SpeedChangingSampleProvider _musicSpeed;
    private readonly SmbPitchShiftingSampleProvider _voicePitch;

    private readonly ProcessorChain _master;

    public float MusicVolume
    {
        set => _music.Volume(value);
    }

    public float MusicSpeed
    {
        set => _musicSpeed.Speed = value;
    }

    public float VoiceVolume
    {
        set => _voice.Volume(value);
    }

    public float VoicePitch
    {
        set => _voicePitch.PitchFactor = value;
    }

    public float MasterVolume
    {
        set => _master.Volume(value);
    }

    public TimeSpan CurrentTime => _stream.CurrentTime;

    public TimeSpan TotalTime => _stream.TotalTime;

    public DiscJockeyProcessor(StreamAudioProcessor stream, Player owner)
    {
        _stream = stream;

        _music = stream
            .ToCompatibleChain()
            .Layer(static provider => new SpeedChangingSampleProvider(provider), out _musicSpeed);
        _voice = new PlayerVoiceSampleProvider(owner)
            .ToCompatibleChain()
            .Layer(static provider => new SmbPitchShiftingSampleProvider(provider), out _voicePitch);
        _master = _music.MixWith(_voice).ToChain();
    }

    public WaveFormat WaveFormat => _master.WaveFormat;

    public int Read(float[] buffer, int offset, int count) => _master.Read(buffer, offset, count);

    public void Dispose() => _master.Dispose();

}
