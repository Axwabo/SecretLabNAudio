using SecretLabNAudio.Core.Providers;
using SecretLabNAudio.Core.SendEngines;
using VoiceChat.Codec;
using VoiceChat.Codec.Enums;
using VoiceChat.Networking;

namespace SecretLabNAudio.Core;

/// <summary>A <see cref="SpeakerToy"/>-bound component playing audio using an <see cref="ISampleProvider"/>.</summary>
public sealed partial class AudioPlayer : MonoBehaviour
{

    private static readonly float[] ReadBuffer = new float[SamplesPerPacket];

    private static readonly byte[] EncoderBuffer = new byte[1024];

    private ISampleProvider? _sampleProvider;

    /// <summary>The provider this player will read from. Set to null to skip updates.</summary>
    /// <exception cref="ArgumentException"><inheritdoc cref="ThrowIfIncompatible" path="exception"/></exception>
    public ISampleProvider? SampleProvider
    {
        get => _sampleProvider;
        set
        {
            ThrowIfIncompatible(value);
            _sampleProvider = value;
        }
    }

    /// <summary>The <see cref="SpeakerToy"/> this player is attached to.</summary>
    public SpeakerToy Speaker { get; private set; } = null!;

    /// <summary>
    /// The <see cref="SendEngine"/> used to broadcast audio messages.
    /// If null, encoding and broadcasting is skipped.
    /// </summary>
    public SendEngine? SendEngine { get; set; } = SendEngine.DefaultEngine;

    /// <summary>An optional monitor to consume read audio samples.</summary>
    public IAudioPacketMonitor? OutputMonitor { get; set; }

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

    private void Update()
    {
        if (IsPaused || SampleProvider == null)
            return;
        _remainingTime += Time.deltaTime;
        while (_remainingTime > 0)
            ProcessPacket();
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
        SendEngine = SendEngine.DefaultEngine;
        _remainingTime = 0;
    }

    private void OnDestroy()
    {
        SampleProvider = null;
        SendEngine = null;
        _encoder.Dispose();
        Destroyed?.Invoke();
    }

    private void ProcessPacket()
    {
        var read = SampleProvider!.Read(ReadBuffer, 0, SamplesPerPacket);
        if (read == 0)
        {
            ClearBuffer();
            OutputMonitor?.OnEmpty();
            NoSamplesRead?.Invoke();
            return;
        }

        if (read < SamplesPerPacket)
        {
            Array.Clear(ReadBuffer, read, SamplesPerPacket - read);
            _remainingTime = PacketDuration;
        }

        _remainingTime -= PacketDuration;
        OutputMonitor?.OnRead(ReadBuffer.AsSpan()[..read]);
        if (SendEngine == null)
            return;
        var encoded = _encoder.Encode(ReadBuffer, EncoderBuffer);
        SendEngine.Broadcast(new AudioMessage(Id, EncoderBuffer, encoded));
    }

    /// <summary>Resets the amount of samples to send and clears the <see cref="SampleProvider"/>'s buffer if it's a <see cref="BufferedSampleProvider"/>.</summary>
    public void ClearBuffer()
    {
        _remainingTime = 0;
        (SampleProvider as BufferedSampleProvider)?.Clear();
    }

    /// <summary>Destroys the player and its <see cref="Speaker"/>.</summary>
    public void Destroy() => Speaker.Destroy();

}
