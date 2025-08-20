using SecretLabNAudio.Core.Pools;

namespace SecretLabNAudio.Demo.Board;

public sealed class DiscJockeyBoard : MonoBehaviour
{

    private const float HalfOffset = 0.5f;
    private const float StageRange = 10;

    private static readonly SpeakerSettings StageSettings = new()
    {
        IsSpatial = false,
        Volume = 1,
        MinDistance = StageRange,
        MaxDistance = StageRange
    };

    private static readonly Vector3 StagePosition = new(39, 315, -32);
    private static readonly Vector3 StageSize = new(8, 2, 8);

    private static readonly Vector3 VisualizerPosition = new(2.96413422f, -0.0985965729f, 3.21929169f);
    private static readonly Quaternion VisualizerRotation = Quaternion.Euler(9.60637665f, 30.1256428f, 0.309277654f);
    private static readonly Vector3 VisualizerScale = new(1.3f, 2, 1);

    private static readonly Vector3 BoardPosition = new(-3.28f, -0.794f, 2.96f);
    private static readonly Quaternion BoardRotation = Quaternion.Euler(0, -4.5f, 0);

    private static readonly Quaternion SliderRotation = Quaternion.Euler(90, 0, 0);

    public static DiscJockeyBoard? Instance { get; private set; }

    public static void SetUpStage()
    {
        var stage = SpawnableCullingParent.Create(Vector3.zero, StageSize);
        stage.Base.NetworkBoundsPosition = StagePosition; // NW couldn't make a properly working wrapper

        var text = TextToy.Create(VisualizerPosition, VisualizerRotation, VisualizerScale, stage.Transform);
        text.IsStatic = true;
        var visualizer = TextVisualizer.Attach(text);

        var board = PrimitiveObjectToy.Create(BoardPosition, BoardRotation, stage.Transform);
        board.IsStatic = true;
        board.Flags = 0; // it's just an anchor
        Instance = board.GameObject.AddComponent<DiscJockeyBoard>();
        var transform = board.Transform;

        Instance._speaker = AudioPlayerPool.Rent(StageSettings, stage.Transform)
            .WithFilteredSendEngine(p => !p.IsAlive || p.IsOutside())
            .WithOutputMonitor(visualizer);
        Outside.PlaceSpeakers(Instance._speaker.Id);

        Instance._music = Slider.Create(transform, Vector3.right * 0.4f, SliderRotation, "🎵", "Music");
        Instance._speed = Slider.Create(transform, Vector3.right * 0.5f, SliderRotation, "⏩", "Speed", 0);
        Instance._voice = Slider.Create(transform, Vector3.right * 0.65f, SliderRotation, "🎤", "Voice");
        Instance._pitch = Slider.Create(transform, Vector3.right * 0.75f, SliderRotation, "🎈️", "Pitch", 0);
        Instance._master = Slider.Create(transform, Vector3.right * 0.9f, SliderRotation, "🔊", "Master", 0);

        Instance._disc = Disc.Create(transform);
    }

    public static bool CanHearStageSpeaker(Player player) => Vector3.Distance(StagePosition, player.Camera.position) <= StageRange;

    private DiscJockeySampleProvider? _provider;

    public Player? Owner { get; private set; }

#nullable disable

    private AudioPlayer _speaker;

    private Slider _music;

    private Slider _speed;

    private Slider _voice;

    private Slider _pitch;

    private Slider _master;

    private Disc _disc;

#nullable restore

    private void Start()
    {
        _music.ValueChanged += UpdateMusic;
        _speed.ValueChanged += UpdateSpeed;
        _voice.ValueChanged += UpdateVoice;
        _pitch.ValueChanged += UpdatePitch;
        _master.ValueChanged += UpdateMaster;
    }

    private void OnDestroy()
    {
        Instance = null;
        _music.ValueChanged -= UpdateMusic;
        _speed.ValueChanged -= UpdateSpeed;
        _voice.ValueChanged -= UpdateVoice;
        _pitch.ValueChanged -= UpdatePitch;
        _master.ValueChanged -= UpdateMaster;
        DisposeProvider();
    }

    public void Play(Player player, WaveStream stream, string label)
    {
        var ownerChanged = Owner != player;
        if (Owner != null && ownerChanged)
            Outside.ClearMutes(Owner);
        Owner = player;
        if (ownerChanged)
            Outside.MuteSpeakers(player);

        DisposeProvider();
        _provider = new DiscJockeySampleProvider(stream, player);
        UpdateMusic(_music.Value);
        UpdateSpeed(_speed.Value);
        UpdateVoice(_voice.Value);
        UpdatePitch(_pitch.Value);
        UpdateMaster(_master.Value);
        _speaker.SampleProvider = _provider;
        _speaker.ClearBuffer();
        _disc.Provider = _provider;
        _disc.Label = label;
        Outside.RunEffects(destroyCancellationToken);
    }

    private void UpdateMusic(float value)
    {
        if (_provider != null)
            _provider.MusicVolume = value + HalfOffset;
    }

    private void UpdateSpeed(float value)
    {
        if (_provider != null)
            _provider.MusicSpeed = value * 2 + 1;
    }

    private void UpdateVoice(float value)
    {
        if (_provider != null)
            _provider.VoiceVolume = value + HalfOffset;
    }

    private void UpdatePitch(float value)
    {
        if (_provider != null)
            _provider.VoicePitch = value + 1;
    }

    private void UpdateMaster(float value)
    {
        if (_provider != null)
            _provider.MasterVolume = value * 2 + 1;
    }

    private void DisposeProvider()
    {
        _provider?.Dispose();
        _provider = null;
        _disc.Provider = null;
    }

}
