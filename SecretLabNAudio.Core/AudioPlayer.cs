using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.Providers;
using SecretLabNAudio.Core.SendEngines;
using VoiceChat.Codec;
using VoiceChat.Codec.Enums;

namespace SecretLabNAudio.Core;

/// <summary>A <see cref="SpeakerToy"/>-bound component playing audio using an <see cref="ISampleProvider"/>.</summary>
public sealed partial class AudioPlayer : MonoBehaviour
{

    private static readonly float[] ReadBuffer = new float[SamplesPerPacket];

    private static readonly byte[] EncoderBuffer = new byte[1024];

    /// <summary>The <see cref="SpeakerToy"/> this player is attached to.</summary>
    public SpeakerToy Speaker { get; private set; } = null!;

    /// <summary>The controller ID of this player.</summary>
    /// <seealso cref="SpeakerToy.ControllerId"/>
    public byte Id
    {
        get => Speaker.ControllerId;
        set => Speaker.ControllerId = value;
    }

    /// <summary>The provider this player will read from. Set to null to skip updates.</summary>
    /// <exception cref="ArgumentException">
    /// Thrown when the given sample provider is not null and does not match the following criteria:
    /// <para>
    /// Encoding = <see cref="WaveFormatEncoding.IeeeFloat"/><br/>
    /// Sample Rate = <see cref="SampleRate"/><br/>
    /// Channels = <see cref="Channels"/>
    /// </para>
    /// </exception>
    /// <remarks>Setting the provider changes the value of <see cref="OwnsProvider"/> to whether the given provider is an <see cref="IAudioProcessor"/>.</remarks>
    /// <seealso cref="AudioPlayerExtensions.Use"/>
    /// <seealso cref="AudioPlayerExtensions.UseFile(AudioPlayer,string,bool,float)"/>
    /// <seealso cref="AudioPlayerExtensions.UseShortClip(AudioPlayer,FileReading.ClipName,bool,float)"/>
    /// <seealso cref="AudioPlayerExtensions.UseMixer(AudioPlayer,bool)"/>
    /// <seealso cref="AudioPlayerExtensions.UseQueue(AudioPlayer)"/>
    /// <seealso cref="AudioPlayerExtensions.WithUnmanagedProvider(AudioPlayer,ISampleProvider)"/>
    public ISampleProvider? SampleProvider
    {
        get;
        set
        {
            if (value == field)
                return;
            ThrowIfIncompatible(value);
            try
            {
                if (OwnsProvider)
                    (field as IDisposable)?.Dispose();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            field = value;
            OwnsProvider = value is IAudioProcessor;
        }
    }

    /// <summary>Whether to dispose of the <see cref="SampleProvider"/> when the provider is changed or this component is pooled/destroyed.</summary>
    /// <remarks>This property is automatically set when the <see cref="SampleProvider"/> changes.</remarks>
    public bool OwnsProvider { get; set; }

    /// <summary>
    /// The <see cref="SendEngine"/> used to broadcast audio messages.
    /// If null, encoding and broadcasting is skipped.
    /// </summary>
    public SendEngine? SendEngine { get; set; } = SendEngine.DefaultEngine;

    /// <summary>An optional monitor to consume read audio samples.</summary>
    public IAudioPacketMonitor? OutputMonitor { get; set; }

    /// <summary>A scalar to multiply samples by before encoding.</summary>
    /// <remarks>
    /// This is different from <see cref="SpeakerToy.Volume"/> and runs after the <see cref="OutputMonitor"/> has received the samples.
    /// It can be used to amplify audio without requiring a volume sample provider.
    /// </remarks>
    public float MasterAmplification { get; set; } = 1;

    /// <summary>Whether the playback is paused.</summary>
    public bool IsPaused { get; set; }

    /// <summary>True if playback has finished on the previous frame (fewer samples were read than requested).</summary>
    public bool HasEnded { get; private set; }

    /// <summary>
    /// If true, the <see cref="SampleProvider"/> will be read from continuously.
    /// If false, the <see cref="SampleProvider"/> will be set to null upon reaching its end.
    /// </summary>
    /// <seealso cref="Ended"/>
    public bool AlwaysRead { get; set; } = true;

    /// <summary>Invoked every frame when no samples were read from the <see cref="SampleProvider"/>.</summary>
    /// <remarks>The provider is not set to null by default.</remarks>
    /// <seealso cref="AlwaysRead"/>
    /// <seealso cref="HasEnded"/>
    /// <seealso cref="Ended"/>
    public event Action? NoSamplesRead;

    /// <summary>
    /// Invoked when <see cref="HasEnded"/> becomes true if it was previously false.
    /// The provider is considered ended if it returns fewer samples than requested (or 0).
    /// </summary>
    /// <remarks>This event is called after <see cref="NoSamplesRead"/></remarks>
    public event Action? Ended;

    /// <summary>Invoked when this player is disabled or destroyed.</summary>
    public event Action? Destroyed;

    private float _remainingTime;

    private readonly OpusEncoder _encoder = new(OpusApplicationType.Audio);

    private void Awake()
    {
        Speaker = this.GetSpeaker("AudioPlayer must be attached to a SpeakerToy.");
        _ = Speaker.Base.destroyCancellationToken;
    }

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
        Destroyed.InvokeSafely();
        NoSamplesRead = null;
        Ended = null;
        Destroyed = null;
        HasEnded = IsPaused = false;
        SampleProvider = null;
        SendEngine = SendEngine.DefaultEngine;
        OutputMonitor = null;
        AlwaysRead = true;
        _remainingTime = 0;
    }

    private void OnDestroy() => _encoder.Dispose();

    private void ProcessPacket()
    {
        int read;
        try
        {
            read = SampleProvider!.Read(ReadBuffer, 0, SamplesPerPacket);
        }
        catch (Exception e)
        {
            read = 0;
            Debug.LogError(e);
        }

        if (read == 0)
        {
            End(true);
            return;
        }

        if (read < SamplesPerPacket)
        {
            Array.Clear(ReadBuffer, read, SamplesPerPacket - read);
            End(false);
        }
        else
        {
            HasEnded = false;
            _remainingTime -= PacketDuration;
        }

        OutputMonitor?.OnRead(ReadBuffer.AsSpan()[..read]);
        if (SendEngine == null)
            return;
        if (MasterAmplification is not 1f)
            for (var i = 0; i < read; i++)
                ReadBuffer[i] *= MasterAmplification;
        var encoded = _encoder.Encode(ReadBuffer, EncoderBuffer);
        SendEngine.Broadcast(new AudioMessage(Id, EncoderBuffer, encoded));
    }

    private void End(bool zero)
    {
        var hasEndedBefore = HasEnded;
        HasEnded = true;
        ClearBuffer();
        if (zero)
        {
            OutputMonitor?.OnEmpty();
            NoSamplesRead.InvokeSafely();
        }

        if (!hasEndedBefore) Ended.InvokeSafely();
        if (!AlwaysRead)
            SampleProvider = null;
    }

    /// <summary>Resets the amount of samples to send and clears the buffer of the single <see cref="BufferedSampleProvider"/> input (if present).</summary>
    public void ClearBuffer()
    {
        _remainingTime = 0;
        this.SingleInputAs<BufferedSampleProvider>()?.Clear();
    }

    /// <summary>Destroys the player and its <see cref="Speaker"/>.</summary>
    public void Destroy() => Speaker.Destroy();

}
