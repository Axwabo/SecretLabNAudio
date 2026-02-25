# SecretLabNAudio.FFmpeg

This module extends [SecretLabNAudio](https://github.com/Axwabo/SecretLabNAudio) to provide FFmpeg-based utilities.

FFmpeg is not shipped with this module. The module includes a command to install FFmpeg.

> [!NOTE]
> SecretLabNAudio is not sponsored nor endorsed by NAudio.
> SecretLabNAudio.FFmpeg is not sponsored nor endorsed by FFmpeg.

# Argument Building

`FFmpegSL` and extensions accept a string or an `FFmpegArguments` struct when starting the FFmpeg process.

The `FFmpegArguments` struct is a simple object to handle basic arguments.

> [!TIP]
> See also: [FFmpeg documentation](https://ffmpeg.org/documentation.html)

For more complex processing pipelines, check out [FFMpegCore](https://github.com/rosenbjerg/FFMpegCore)
which has an `FFMpegArguments` class that you can use (notice the difference in capitalization).
Pass its `Text` property to `FFmpegSL`
