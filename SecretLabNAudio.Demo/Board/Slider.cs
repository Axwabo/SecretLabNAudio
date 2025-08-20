namespace SecretLabNAudio.Demo.Board;

public sealed class Slider : MonoBehaviour
{

    private const string TextFormat = "{0}\n<size=0.6em>{1}";
    private const float KnobPositionZ = -0.005f;

    private static readonly Vector3 TrackScale = new(0.04f, 0.4f, 1);
    private static readonly Vector3 KnobScale = new(1.5f, 0.05f, 0.01f);

    private static readonly Color InactiveColor = new(10, 10, 10);
    private static readonly Color ActiveColor = new(20, 20, 20);

    private static float Clamp(float value) => Mathf.Clamp(value, -0.5f, 0.5f);

    public static Slider Create(
        Transform parent,
        Vector3 position,
        Quaternion rotation,
        string icon,
        string hint,
        float value = 0.5f
    )
    {
        var track = PrimitiveObjectToy.Create(position, rotation, TrackScale, parent);
        track.Type = PrimitiveType.Quad;
        track.Color = Color.gray;
        track.IsStatic = true;

        var knobPosition = new Vector3(0, Clamp(value), KnobPositionZ);
        var knob = PrimitiveObjectToy.Create(knobPosition, Quaternion.identity, KnobScale, track.Transform);
        knob.Type = PrimitiveType.Cube;
        knob.Color = InactiveColor;
        knob.MovementSmoothing = 230;

        var slider = knob.GameObject.AddComponent<Slider>();
        slider._knobPrimitive = knob;

        var reset = InteractableToy.Create(position + rotation * Vector3.down * 0.25f, rotation, Vector3.one * 0.05f, parent);
        reset.IsStatic = true;
        reset.OnInteracted += _ => slider.Value = value;

        var label = TextToy.Create(reset.Transform);
        label.TextFormat = TextFormat;
        label.Arguments.Add(icon);
        label.Arguments.Add(hint);
        label.IsStatic = true;
        return slider;
    }

#nullable disable

    private PrimitiveObjectToy _knobPrimitive;

    private Transform _knob;

    private Transform _track;

#nullable restore

    private Vector3 _trackPosition;

    private Vector3 _trackNormal;

    private Player? _holder;

    public float Value
    {
        get => _knob.localPosition.y;
        set
        {
            var localPosition = _knob.localPosition;
            var targetValue = Clamp(value);
            if (Mathf.Approximately(targetValue, localPosition.y))
                return;
            _knob.localPosition = localPosition with {y = targetValue};
            ValueChanged?.Invoke(targetValue);
        }
    }

    public event Action<float>? ValueChanged;

    private void Awake()
    {
        _knob = transform;
        _track = _knob.parent;
        _trackPosition = _track.position;
        _trackNormal = -_track.forward; // quads' backs are visible
        SliderSetting.Pressed += Grab;
        SliderSetting.Released += Release;
    }

    private void Update()
    {
        if (_holder == null)
            return;
        // NW forgot to add IsDestroyed to the Player wrapper
        if (!_holder.ReferenceHub)
        {
            Release(_holder);
            return;
        }

        if (TryGetOffset(_holder, out var value))
            Value = value.y;
    }

    private void OnDestroy()
    {
        SliderSetting.Pressed -= Grab;
        SliderSetting.Released -= Release;
    }

    private bool TryGetOffset(Player player, out Vector3 offset)
    {
        var cameraPosition = player.Camera.position;
        if (Vector3.Distance(cameraPosition, _trackPosition) > 20)
        {
            offset = Vector3.zero;
            return false;
        }

        var cameraForward = player.Camera.forward;
        var ray = new Ray(cameraPosition - _trackPosition, cameraForward);
        var plane = new Plane(_trackNormal, 0);
        if (!plane.Raycast(ray, out var enter))
        {
            offset = Vector3.zero;
            return false;
        }

        offset = _track.InverseTransformVector(ray.GetPoint(enter));
        return true;
    }

    private void Grab(Player player)
    {
        if (!TryGetOffset(player, out var offset)
            || offset.Abs() is not {x: <= 0.8f, y: <= 0.52f, z: <= 0.6f}) // track's bounding box check with some leniency
            return;
        _holder = player;
        _knobPrimitive.Color = ActiveColor;
        Value = offset.y;
    }

    private void Release(Player player)
    {
        if (_holder != player)
            return;
        _holder = null;
        _knobPrimitive.Color = InactiveColor;
    }

}
