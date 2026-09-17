using System.Reflection;
using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.Providers;

namespace SecretLabNAudio.Core.FileReading;

public static partial class ShortClipCache
{

    public static RawSourceSampleProvider? AddFromEmbeddedResource(Assembly assembly, string resourceName, TimeSpan? maxDuration = null, bool trimExtension = true)
        => AddFromEmbeddedResource(assembly, resourceName, Path.GetExtension(resourceName), (resourceName, trimExtension), maxDuration);

    public static RawSourceSampleProvider? AddFromEmbeddedResource(Assembly assembly, string resourceName, ClipName clipName, TimeSpan? maxDuration = null)
        => AddFromEmbeddedResource(assembly, resourceName, Path.GetExtension(resourceName), clipName, maxDuration);

    public static RawSourceSampleProvider? AddFromEmbeddedResource(Assembly assembly, string resourceName, string fileType, ClipName clipName, TimeSpan? maxDuration = null)
    {
        using var resource = assembly.GetManifestResourceStream(resourceName);
        if (resource == null)
            return null;
        if (!TryCreateAudioReader.Stream(resource, fileType, false, out var stream)
            || maxDuration.HasValue && stream.TotalTime > maxDuration.Value)
            return null;
        try
        {
            var provider = stream.ReadPlayerCompatibleSamples();
            Add(clipName, provider);
            return provider;
        }
        finally
        {
            stream.Dispose();
        }
    }

    public static int AddFromEmbeddedResources(Assembly assembly, TimeSpan? maxDuration = null, bool trimExtension = true)
        => AddFromEmbeddedResources(assembly, null, maxDuration, trimExtension);

    public static int AddFromEmbeddedResources(Assembly assembly, string? prefixToTrim, TimeSpan? maxDuration = null, bool trimExtension = true)
        => AddFromEmbeddedResources(assembly, null, prefixToTrim, maxDuration, trimExtension);

    public static int AddFromEmbeddedResources(Assembly assembly, string? fileTypeFilter, string? prefixToTrim = null, TimeSpan? maxDuration = null, bool trimExtension = true)
    {
        var count = 0;
        var type = fileTypeFilter.AsSpan().TrimStart('.');
        foreach (var resource in assembly.GetManifestResourceNames())
        {
            if (!type.IsEmpty && !Path.GetExtension(resource.AsSpan()).Equals(type, StringComparison.OrdinalIgnoreCase))
                continue;
            var name = string.IsNullOrEmpty(prefixToTrim) ? resource : resource.RemoveStart(prefixToTrim!);
            if (AddFromEmbeddedResource(assembly, name, maxDuration, trimExtension) != null)
                count++;
        }

        return count;
    }

    extension(string s)
    {

        private string RemoveStart(string start) => s.StartsWith(start, StringComparison.OrdinalIgnoreCase)
            ? s.AsSpan(start.Length).TrimStart('.').ToString()
            : s;

    }

}
