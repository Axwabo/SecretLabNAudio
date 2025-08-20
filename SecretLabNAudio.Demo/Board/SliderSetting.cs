using UserSettings.ServerSpecific;

namespace SecretLabNAudio.Demo.Board;

public static class SliderSetting
{

    private static readonly SSKeybindSetting Setting = new(null, "Grab Slider", KeyCode.Mouse0);

    public static event Action<Player>? Pressed;

    public static event Action<Player>? Released;

    internal static void Register()
    {
        ServerSpecificSettingsSync.DefinedSettings ??= [];
        ServerSpecificSettingsSync.DefinedSettings = [..ServerSpecificSettingsSync.DefinedSettings, Setting];
        ServerSpecificSettingsSync.ServerOnSettingValueReceived += OnValueReceived;
    }

    internal static void Unregister()
    {
        ServerSpecificSettingsSync.DefinedSettings ??= [];
        ServerSpecificSettingsSync.DefinedSettings = ServerSpecificSettingsSync.DefinedSettings.Except([Setting]).ToArray();
    }

    private static void OnValueReceived(ReferenceHub hub, ServerSpecificSettingBase setting)
    {
        if (setting.SettingId != Setting.SettingId || setting is not SSKeybindSetting keybind)
            return;
        var @event = keybind.SyncIsPressed ? Pressed : Released;
        @event?.Invoke(Player.Dictionary[hub]);
    }

}
