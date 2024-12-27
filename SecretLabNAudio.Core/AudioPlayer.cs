using AdminToys;
using Mirror;
using NAudio.Wave;
using UnityEngine;

namespace SecretLabNAudio.Core;

public sealed class AudioPlayer : MonoBehaviour
{

    public const int SampleRate = 48000;
    public const int Channels = 1;

    public static WaveFormat SupportedFormat { get; } = WaveFormat.CreateIeeeFloatWaveFormat(SampleRate, Channels);

    public static AudioPlayer Create(AudioPlayerSettings settings, Vector3 position = default)
    {
        foreach (var value in NetworkClient.prefabs.Values)
            if (value.TryGetComponent(out SpeakerToy toy))
            {
                var o = Instantiate(toy, position, Quaternion.identity).gameObject;
                NetworkServer.Spawn(o);
                var player = o.AddComponent<AudioPlayer>();
                player.ApplySettings(settings);
                return player;
            }

        throw new MissingComponentException("SpeakerToy not found");
    }

    private ISampleProvider? _sampleProvider;

    public ISampleProvider? SampleProvider
    {
        get => _sampleProvider;
        set
        {
            if (value is {WaveFormat: {SampleRate: not SampleRate, Channels: not Channels}})
                throw new ArgumentException("Expected a mono provider with a sample rate of 48000Hz and.");
            _sampleProvider = value;
        }
    }

    public SpeakerToy Speaker { get; private set; } = null!;

    private void Awake()
    {
        Speaker = GetComponent<SpeakerToy>();
        if (!Speaker)
            throw new InvalidOperationException("AudioPlayer must be attached to a SpeakerToy.");
    }

    private void Update()
    {
    }

}
