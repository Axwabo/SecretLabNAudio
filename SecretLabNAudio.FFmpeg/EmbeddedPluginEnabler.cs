using System.Linq;
using LabApi.Loader;

namespace SecretLabNAudio.FFmpeg;

internal static class EmbeddedPluginEnabler
{

    private static bool IsAlreadyEnabled => PluginLoader.EnabledPlugins.Any(static e => e.GetType().FullName == typeof(FFmpegPlugin).FullName);

    public static void EnablePlugin()
    {
        if (IsAlreadyEnabled)
            return;
        var plugin = new FFmpegPlugin();
        var assembly = plugin.GetType().Assembly;
        PluginLoader.Dependencies.Remove(assembly);
        PluginLoader.EnablePlugin(plugin);
    }

}
