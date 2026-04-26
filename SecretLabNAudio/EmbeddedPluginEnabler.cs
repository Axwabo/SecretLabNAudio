using System.Linq;
using LabApi.Loader;
using SecretLabNAudio.FFmpeg;

namespace SecretLabNAudio;

internal static class EmbeddedPluginEnabler
{

    private static bool IsAlreadyEnabled => PluginLoader.Plugins.Any(static e => e.Key.GetType().FullName == typeof(FFmpegPlugin).FullName);

    public static void EnableFFmpeg()
    {
        if (IsAlreadyEnabled)
            return;
        var plugin = new FFmpegPlugin();
        var assembly = plugin.GetType().Assembly;
        PluginLoader.Plugins[plugin] = assembly;
        PluginLoader.Dependencies.Remove(assembly);
        PluginLoader.EnablePlugins([plugin]);
    }

}
