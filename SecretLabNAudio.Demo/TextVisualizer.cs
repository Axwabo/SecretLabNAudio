namespace SecretLabNAudio.Demo;

public sealed class TextVisualizer : IAudioPacketMonitor
{

    private const int Count = 120;
    private const int Offset = AudioPlayer.PacketsPerSecond / 10; // client-side delay

    private const string Suffix = "<alpha=#00><size=1em>|"; // "jumping" prevention

    public static TextVisualizer Attach(TextToy text)
    {
        var bars = string.Join("", Enumerable.Range(0, Count).Select(e => $"<size={{{e + Offset}}}em>|</size>"));
        text.TextFormat = $"<mspace=0.2px><line-height=1em>{bars}{Suffix}";
        text.Arguments.AddRange(Enumerable.Range(0, Count + Offset).Select(_ => "0"));
        return new TextVisualizer(text);
    }

    private readonly TextToy _text;

    private TextVisualizer(TextToy text) => _text = text;

    public void OnRead(ReadOnlySpan<float> buffer)
    {
        var total = 0f;
        foreach (var f in buffer)
            total += f * f;
        var rms = Mathf.Sqrt(total / buffer.Length);
        Queue(Mathf.Min(0.5f, rms).ToString("F4")); // clamp to prevent going outside the board
    }

    public void OnEmpty() => Queue("0");

    private void Queue(string item)
    {
        _text.Arguments.RemoveAt(0);
        _text.Arguments.Add(item);
    }

}
