using NAudio.Wave.SampleProviders;

namespace SecretLabNAudio.Demo;

public sealed class DiscJockeySampleProvider : ISampleProvider, IDisposable
{

    private readonly WaveStream _music;
    private readonly PlayerVoiceSampleProvider _voice;

    private readonly VolumeSampleProvider _musicVolume;
    private readonly SpeedChangingSampleProvider _musicSpeed;
    private readonly VolumeSampleProvider _voiceVolume;
    private readonly SmbPitchShiftingSampleProvider _voicePitch;
    private readonly VolumeSampleProvider _master;

    public float MusicVolume
    {
        set => _musicVolume.Volume = value;
    }

    public float MusicSpeed
    {
        set => _musicSpeed.Speed = value;
    }

    public float VoiceVolume
    {
        set => _voiceVolume.Volume = value;
    }

    public float VoicePitch
    {
        set => _voicePitch.PitchFactor = value;
    }

    public float MasterVolume
    {
        set => _master.Volume = value;
    }

    public TimeSpan CurrentTime => _music.CurrentTime;

    public TimeSpan TotalTime => _music.TotalTime;

    public DiscJockeySampleProvider(WaveStream music, Player owner)
    {
        _music = music;
        _voice = new PlayerVoiceSampleProvider(owner);

        _musicSpeed = new SpeedChangingSampleProvider(music.ToPlayerCompatible());
        _musicVolume = _musicSpeed.Volume();
        _voicePitch = new SmbPitchShiftingSampleProvider(_voice);
        _voiceVolume = _voicePitch.Volume();
        _master = _musicVolume.MixWith(_voiceVolume).Volume();
    }

    public WaveFormat WaveFormat => _master.WaveFormat;

    public int Read(float[] buffer, int offset, int count) => _master.Read(buffer, offset, count);

    public void Dispose()
    {
        _music.Dispose();
        _voice.Dispose();
    }

}
