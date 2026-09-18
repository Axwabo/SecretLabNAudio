using System.Reflection;
using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.Providers;

namespace SecretLabNAudio.Core.FileReading;

public static partial class ShortClipCache
{

    /// <summary>
    /// Attempts to add the samples from the given embedded resource to the cache.
    /// The clip name will be based on <paramref name="resourceName"/> and <paramref name="trimExtension"/>.
    /// <b>Do not use this for storing lengthy audio, stream the resources instead.</b>
    /// </summary>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/One/param[@name='assembly']"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/One/param[@name='resourceName']"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/One/param[@name='maxDuration']"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/One/param[@name='trimExtension']"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/One/returns"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Time/remarks"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/seealso"/>
    public static RawSourceSampleProvider? AddFromEmbeddedResource(Assembly assembly, string resourceName, TimeSpan? maxDuration = null, bool trimExtension = true)
        => AddFromEmbeddedResource(assembly, resourceName, (resourceName, trimExtension), maxDuration);

    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/One/summary"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/One/param[@name='assembly']"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/One/param[@name='resourceName']"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/One/param[@name='clipName']"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/One/param[@name='maxDuration']"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/One/returns"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Time/remarks"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/seealso"/>
    public static RawSourceSampleProvider? AddFromEmbeddedResource(Assembly assembly, string resourceName, ClipName clipName, TimeSpan? maxDuration = null)
        => AddFromEmbeddedResource(assembly, resourceName, Path.GetExtension(resourceName), clipName, maxDuration);

    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/One/summary"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/One/param[@name='assembly']"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/One/param[@name='resourceName']"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/One/param[@name='fileType']"/>
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

    /// <summary>
    /// Attempts to add all clips from the embedded resources of the given assembly.
    /// <b>Do not use this for storing lengthy audio, stream the resources instead.</b>
    /// </summary>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/Multiple/param[@name='assembly']"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/Multiple/param[@name='maxDuration']"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/Multiple/param[@name='trimExtension']"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/Multiple/returns"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Time/remarks"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/seealso"/>
    public static int AddAllFromEmbeddedResources(Assembly assembly, TimeSpan? maxDuration = null, bool trimExtension = true)
        => AddAllFromEmbeddedResources(assembly, null, maxDuration, trimExtension);

    /// <summary>
    /// Attempts to add clips from the embedded resources of the given assembly.
    /// <b>Do not use this for storing lengthy audio, stream the resources instead.</b>
    /// </summary>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/Multiple/param[@name='assembly']"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/Multiple/param[@name='fileTypeFilter']"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/Multiple/param[@name='maxDuration']"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/Multiple/param[@name='trimExtension']"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/Multiple/returns"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Time/remarks"/>
    /// <include file="../XmlDocs/Clips.xml" path="doc/Add/Embedded/seealso"/>
    public static int AddAllFromEmbeddedResources(Assembly assembly, string? fileTypeFilter, TimeSpan? maxDuration = null, bool trimExtension = true)
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
