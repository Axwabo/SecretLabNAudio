namespace SecretLabNAudio.FFmpeg;

public readonly partial record struct FFmpegArguments(bool ShowLogs, string? InputOptions, string? Input, int SampleRate, int Channels, string? OutputOptions, string? MuxerFormat, string? Output);
