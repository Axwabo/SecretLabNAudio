using SecretLabNAudio.Core.Providers;
using SecretLabNAudio.Core.SendEngines;
using VoiceChat.Codec;
using VoiceChat.Codec.Enums;
using VoiceChat.Networking;

namespace SecretLabNAudio.Core;

/// <summary>A <see cref="SpeakerToy"/>-bound component playing audio using an <see cref="ISampleProvider"/>.</summary>
public sealed partial class AudioPlayer : MonoBehaviour
{

    private static readonly float[] SendBuffer = new float[PacketSamples];

    private static readonly byte[] EncoderBuffer = new byte[1024];

    private ISampleProvider? _sampleProvider;

    /// <summary>
    /// 
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Thrown when the given sample provider is not null and does not match the following criteria:
    /// <para>
    /// Encoding = <see cref="WaveFormatEncoding.IeeeFloat"/><br/>
    /// Sample Rate = <see cref="SampleRate"/><br/>
    /// Channels = <see cref="Channels"/>
    /// </para>
    /// </exception>
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

    /// <summary>The <see cref="SpeakerToy"/> this player is attached to.</summary>
    public SpeakerToy Speaker { get; private set; } = null!;

    /// <summary>
    /// The <see cref="SendEngine"/> used to broadcast audio messages.
    /// Will be set to <see cref="SendEngines.SendEngine.DefaultEngine"/> if not provided.
    /// </summary>
    public SendEngine? SendEngine { get; set; }

    /// <summary>Whether the playback is paused.</summary>
    public bool IsPaused { get; set; }

    /// <summary>The controller ID of this player.</summary>
    /// <seealso cref="SpeakerToy.ControllerId"/>
    public byte Id
    {
        get => Speaker.ControllerId;
        set => Speaker.ControllerId = value;
    }

    /// <summary>Invoked every frame when no samples were read from the <see cref="SampleProvider"/>.</summary>
    /// <remarks>The provider is not set to null by default.</remarks>
    /// <seealso cref="AudioPlayerExtensions.UnsetProviderOnEnd"/>
    public event Action? NoSamplesRead;

    /// <summary>Invoked when this player is disabled or destroyed.</summary>
    public event Action? Destroyed;

    private float _remainingTime;

    private readonly OpusEncoder _encoder = new(OpusApplicationType.Audio);

    private void Awake() => Speaker = this.GetSpeaker("AudioPlayer must be attached to a SpeakerToy.");

    private void Start() => SendEngine ??= SendEngine.DefaultEngine;

    private void Update()
    {
        if (IsPaused || SampleProvider == null)
            return;
        _remainingTime += Time.deltaTime;
        while (_remainingTime > 0)
        {
            var read = SampleProvider.Read(SendBuffer, 0, PacketSamples);
            if (read == 0)
            {
                ClearBuffer();
                NoSamplesRead?.Invoke();
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

    private void OnDisable()
    {
        try
        {
            Destroyed?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        NoSamplesRead = null;
        Destroyed = null;
        IsPaused = false;
        SampleProvider = null;
        SendEngine = null;
        _remainingTime = 0;
    }

    private void OnDestroy()
    {
        _encoder.Dispose();
        Destroyed?.Invoke();
    }

    /// <summary>Resets the amount of samples to send and clears the <see cref="SampleProvider"/>'s buffer if it's a <see cref="BufferedSampleProvider"/>.</summary>
    public void ClearBuffer()
    {
        _remainingTime = 0;
        (SampleProvider as BufferedSampleProvider)?.Clear();
    }

}
