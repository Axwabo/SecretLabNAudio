using System.Reflection;
using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.Providers;

namespace SecretLabNAudio.Core.FileReading;

public static partial class ShortClipCache
{

    /// <summary>
    /// </summary>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/One/param[name='assembly']"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/One/param[name='resourceName']"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/One/param[@name='maxDuration']"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/One/param[@name='trimExtension']"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/One/returns"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Time/remarks"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/seealso"/>
    public static RawSourceSampleProvider? AddFromEmbeddedResource(Assembly assembly, string resourceName, TimeSpan? maxDuration = null, bool trimExtension = true)
        => AddFromEmbeddedResource(assembly, resourceName, Path.GetExtension(resourceName), (resourceName, trimExtension), maxDuration);

    /// <summary>
    /// </summary>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/One/param[name='assembly']"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/One/param[name='resourceName']"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/One/param[@name='clipName']"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/One/param[@name='maxDuration']"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/One/returns"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Time/remarks"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/seealso"/>
    public static RawSourceSampleProvider? AddFromEmbeddedResource(Assembly assembly, string resourceName, ClipName clipName, TimeSpan? maxDuration = null)
        => AddFromEmbeddedResource(assembly, resourceName, Path.GetExtension(resourceName), clipName, maxDuration);

    /// <summary>
    /// </summary>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/One/param[name='assembly']"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/One/param[name='resourceName']"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/One/param[name='fileType']"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/One/param[@name='clipName']"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/One/param[@name='maxDuration']"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/One/returns"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Time/remarks"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/seealso"/>
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

    public static int AddFromEmbeddedResources(Assembly assembly, string? fileTypeFilter, TimeSpan? maxDuration = null, bool trimExtension = true)
    {
        var count = 0;
        var type = fileTypeFilter.AsSpan().TrimStart('.');
        foreach (var resource in assembly.GetManifestResourceNames())
        {
            if (!type.IsEmpty && !Path.GetExtension(resource.AsSpan()).TrimStart('.').Equals(type, StringComparison.OrdinalIgnoreCase))
                continue;
            if (AddFromEmbeddedResource(assembly, resource, maxDuration, trimExtension) != null)
                count++;
        }

        return count;
    }

}
