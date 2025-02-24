using AdminToys;
using NAudio.Wave;
using SecretLabNAudio.Core.SendEngines;
using VoiceChat.Codec;
using VoiceChat.Codec.Enums;
using VoiceChat.Networking;

namespace SecretLabNAudio.Core;

public sealed partial class AudioPlayer : MonoBehaviour
{

    private const int SendBufferSize = SampleRate / 100;

    private ISampleProvider? _sampleProvider;

    public ISampleProvider? SampleProvider
    {
        get => _sampleProvider;
        set
        {
            if (value is {WaveFormat: not {SampleRate: SampleRate, Channels: Channels, Encoding: WaveFormatEncoding.IeeeFloat}})
                throw new ArgumentException($"Expected a mono provider with a sample rate of 48000Hz and IEEEFloat encoding, got format {value.WaveFormat}");
            _sampleProvider = value;
            _samplesToSend = 0;
        }
    }

    public SpeakerToy Speaker { get; private set; } = null!;

    public SendEngine? SendEngine { get; set; }

    public byte Id
    {
        get => Speaker.NetworkControllerId;
        set => Speaker.NetworkControllerId = value;
    }

    public bool IsSpatial
    {
        get => Speaker.NetworkIsSpatial;
        set => Speaker.NetworkIsStatic = !(Speaker.NetworkIsSpatial = value);
    }

    public float Volume
    {
        get => Speaker.NetworkVolume;
        set => Speaker.NetworkVolume = value;
    }

    public float MinDistance
    {
        get => Speaker.NetworkMinDistance;
        set => Speaker.NetworkMinDistance = value;
    }

    public float MaxDistance
    {
        get => Speaker.NetworkMaxDistance;
        set => Speaker.NetworkMaxDistance = value;
    }

    public bool IsPaused { get; set; }

    private void Awake()
    {
        Speaker = GetComponent<SpeakerToy>();
        if (!Speaker)
            throw new InvalidOperationException("AudioPlayer must be attached to a SpeakerToy.");
    }

    private void Start() => SendEngine ??= new SendEngine();

    private int _samplesToSend;

    private readonly float[] _readBuffer = new float[SampleRate / 10];

    private readonly PlaybackBuffer _playbackBuffer = new();

    private readonly OpusEncoder _encoder = new(OpusApplicationType.Voip);

    private readonly float[] _sendBuffer = new float[SendBufferSize];

    private readonly byte[] _encoderBuffer = new byte[1024];

    private void Update()
    {
        if (SampleProvider == null || IsPaused)
            return;
        var delta = (int) (Time.deltaTime * SampleRate);
        _samplesToSend += delta;
        var read = ReadAudio(_readBuffer, Mathf.Min(_samplesToSend, _readBuffer.Length));
        if (read == 0)
        {
            _samplesToSend -= delta;
            return;
        }

        _playbackBuffer.Write(_readBuffer, read);
        _samplesToSend = Mathf.Max(_samplesToSend - read, 0);
        SendAudio();
    }

    private int ReadAudio(float[] buffer, int count) => SampleProvider!.Read(buffer, 0, count);

    private void SendAudio()
    {
        while (_playbackBuffer.Length > SendBufferSize)
        {
            _playbackBuffer.ReadTo(_sendBuffer, SendBufferSize);
            var encoded = _encoder.Encode(_sendBuffer, _encoderBuffer);
            var message = new AudioMessage(Speaker.NetworkControllerId, _encoderBuffer, encoded);
            SendEngine?.Broadcast(message);
        }
    }

    public void ClearBuffer()
    {
        _samplesToSend = 0;
        _playbackBuffer.Clear();
    }

    public event Action? OnDestroyed;

    private void OnDestroy()
    {
        _encoder.Dispose();
        _playbackBuffer.Dispose();
        OnDestroyed?.Invoke();
    }

}
