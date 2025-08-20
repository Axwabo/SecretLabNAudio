using LabApi.Events.CustomHandlers;
using LabApi.Loader.Features.Plugins;
using SecretLabNAudio.Demo.Board;

namespace SecretLabNAudio.Demo;

public sealed class SecretLabNAudioDemo : Plugin
{

    public override string Name => "SecretLabNAudio.Demo";

    public override string Description => "Demo plugin for SecretLabNAudio";

    public override string Author => "Axwabo";

    public override Version Version => GetType().Assembly.GetName().Version;

    public override Version RequiredApiVersion { get; } = new(1, 0, 0);

    private readonly EventHandlers _handlers = new();

    public override void Enable()
    {
        CustomHandlersManager.RegisterEventsHandler(_handlers);
        SliderSetting.Register();
    }

    public override void Disable()
    {
        CustomHandlersManager.UnregisterEventsHandler(_handlers);
        SliderSetting.Unregister();
    }

}
