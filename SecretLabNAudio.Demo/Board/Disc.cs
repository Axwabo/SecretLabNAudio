namespace SecretLabNAudio.Demo.Board;

public sealed class Disc : MonoBehaviour
{

    private const string UndefinedTime = "--:--";

    private static readonly Vector3 PlatterScale = new(0.4f, 0.02f, 0.4f);
    private static readonly Color PlatterColor = new(0.8f, 0.8f, 0.8f);

    private static readonly Vector3 SlipmatScale = new(0.4f, 1.01f, 0.4f);
    private static readonly Color SlipmatColor = new(0.1f, 0.1f, 0.1f);

    private static readonly Quaternion TextRotation = Quaternion.Euler(90, 0, 0);

    public static Disc Create(Transform parent)
    {
        var platter = PrimitiveObjectToy.Create(Vector3.zero, Quaternion.identity, PlatterScale, parent);
        platter.Type = PrimitiveType.Cylinder;
        platter.Color = PlatterColor;
        platter.MovementSmoothing = 230;

        var disc = platter.GameObject.AddComponent<Disc>();
        var transform = platter.Transform;

        var slipmat = PrimitiveObjectToy.Create(Vector3.zero, Quaternion.identity, SlipmatScale, transform);
        slipmat.Type = PrimitiveType.Cylinder;
        slipmat.Color = SlipmatColor;
        slipmat.IsStatic = true;

        var label = TextToy.Create(new Vector3(0, 1.1f, 0), TextRotation, Vector3.one * 0.1f, transform);
        label.TextFormat = "<mark=#00000077><color=#6f6><b>{0}";
        label.Arguments.Add("Use the DJ command");

        var time = TextToy.Create(Vector3.back * 0.25f, TextRotation, Vector3.one * 0.05f, parent);
        time.TextFormat = """
                          <line-height=0><pos=-4em>{0}
                          /
                          <pos=4em>{1}
                          """;
        time.Arguments.Add(UndefinedTime);
        time.Arguments.Add(UndefinedTime);
        time.IsStatic = true;

        disc._label = label;
        disc._time = time;
        return disc;
    }

#nullable disable

    private Transform _t;

    private TextToy _label;

    private TextToy _time;

#nullable restore

    private double _previousTime;

    private DiscJockeySampleProvider? _provider;

    public DiscJockeySampleProvider? Provider
    {
        get => _provider;
        set
        {
            _provider = value;
            _previousTime = 0;
            _time.Arguments[1] = value?.TotalTime.ToString("mm':'ss") ?? UndefinedTime;
        }
    }

    public string Label
    {
        set => _label.Arguments[0] = value;
    }

    private void Awake() => _t = transform;

    private void Update()
    {
        if (Provider == null)
            return;
        var currentTime = Provider.CurrentTime.TotalSeconds;
        _t.Rotate(Vector3.up, (float) (currentTime - _previousTime) * 360);
        _previousTime = currentTime;
        _time.Arguments[0] = Provider.CurrentTime.ToString("mm':'ss");
    }

    private void OnDestroy() => _provider = null;

}
