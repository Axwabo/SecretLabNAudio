using NAudio.Wave;
using SecretLabNAudio.Core.SendEngines;
using VoiceChat.Codec;
using VoiceChat.Codec.Enums;
using VoiceChat.Networking;
using SpeakerToy = AdminToys.SpeakerToy;

namespace SecretLabNAudio.Core;

public sealed partial class AudioPlayer : MonoBehaviour
{

    private const int PacketsPerSecond = 100;

    private const int PacketSamples = SampleRate / PacketsPerSecond;

    private const float PacketDuration = 1f / PacketsPerSecond;

    private static readonly float[] SendBuffer = new float[PacketSamples];

    private static readonly byte[] EncoderBuffer = new byte[1024];

    private ISampleProvider? _sampleProvider;

    public ISampleProvider? SampleProvider
    {
        get => _sampleProvider;
        set
        {
            if (value is {WaveFormat: not {SampleRate: SampleRate, Channels: Channels, Encoding: WaveFormatEncoding.IeeeFloat}})
                throw new ArgumentException($"Expected a mono provider with a sample rate of 48000Hz and IEEEFloat encoding, got format {value.WaveFormat}");
            _sampleProvider = value;
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

    private float _remainingTime;

    private readonly OpusEncoder _encoder = new(OpusApplicationType.Audio);

    private void Update()
    {
        if (IsPaused)
            return;
        _remainingTime += Time.deltaTime;
        while (_remainingTime > 0)
        {
            var read = SampleProvider?.Read(SendBuffer, 0, PacketSamples) ?? 0;
            if (read == 0)
            {
                ClearBuffer();
                break;
            }

            if (read < PacketSamples)
            {
                Array.Clear(SendBuffer, read, PacketSamples - read);
                _remainingTime = PacketDuration;
            }

            var encoded = _encoder.Encode(SendBuffer, EncoderBuffer);
            SendEngine?.Broadcast(new AudioMessage(Id, EncoderBuffer, encoded));
            _remainingTime -= PacketDuration;
        }
    }

    public void ClearBuffer() => _remainingTime = 0;

    public event Action? OnDestroyed;

    private void OnDestroy()
    {
        _encoder.Dispose();
        OnDestroyed?.Invoke();
    }

}
