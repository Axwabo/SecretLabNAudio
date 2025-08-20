using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using SecretLabNAudio.Core.Providers;
using VoiceChat.Codec;

namespace SecretLabNAudio.Demo;

public sealed class PlayerVoiceSampleProvider : ISampleProvider, IDisposable
{

    private readonly Player _owner;
    private readonly PlaybackBufferSampleProvider _provider;

    private readonly OpusDecoder _decoder = new();
    private readonly float[] _decoderBuffer = new float[AudioPlayer.SamplesPerPacket];

    public PlayerVoiceSampleProvider(Player owner)
    {
        _provider = new PlaybackBufferSampleProvider(1d, AudioPlayer.SampleRate) {ReadFully = true};
        _owner = owner;
        PlayerEvents.SendingVoiceMessage += OnSendingMessage;
    }

    public WaveFormat WaveFormat => _provider.WaveFormat;

    public int Read(float[] buffer, int offset, int count) => _provider.Read(buffer, offset, count);

    public void Dispose()
    {
        _decoder.Dispose();
        PlayerEvents.SendingVoiceMessage -= OnSendingMessage;
    }

    private void OnSendingMessage(PlayerSendingVoiceMessageEventArgs ev)
    {
        if (ev.Player != _owner)
            return;
        var decoded = _decoder.Decode(ev.Message.Data, ev.Message.DataLength, _decoderBuffer);
        _provider.Write(_decoderBuffer, 0, decoded);
    }

}
