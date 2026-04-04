namespace SecretLabNAudio.FFmpeg;

/// <summary>
/// A simple container for some arguments to be passed to FFmpeg.
/// </summary>
/// <param name="ShowLogs">Whether to print logs. If false, logging verbosity is set to <c>error</c>.</param>
/// <param name="InputOptions">Arguments before the input.</param>
/// <param name="Input">The input source (e.g. file path, URL).</param>
/// <param name="SampleRate">The sample rate to output. 0 = no override.</param>
/// <param name="Channels">The channel count to output. 0 = no override.</param>
/// <param name="OutputOptions">Arguments before the output (format).</param>
/// <param name="MuxerFormat">The format to output as. null or whitespace = no override.</param>
/// <param name="Output">The destination to write to (e.g. file path, pipe).</param>
public readonly partial record struct FFmpegArguments(bool ShowLogs, string? InputOptions, string? Input, int SampleRate, int Channels, string? OutputOptions, string? MuxerFormat, string? Output);
