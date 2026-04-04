# SecretLabNAudio

An advanced audio player API for SCP: Secret Laboratory using [NAudio](https://github.com/naudio/NAudio)

This library has a number of open-source dependencies. See [Attributions](ATTRIBUTIONS.md)

> [!NOTE]
> SecretLabNAudio is not sponsored nor endorsed by NAudio.

> [!IMPORTANT]
> If you're still using v1, [migrate to v2](https://github.com/Axwabo/SecretLabNAudio/wiki/Migration-Guide)

# Features

[Watch the demo](https://youtu.be/6btaXm3BU8s)

- Fully customizable audio provider interfaces
- Real-time audio processing
- Streaming files from disk
- Resampling and downmixing
- Ogg Vorbis support using [NVorbis](https://github.com/NVorbis/NVorbis)
- MP3 support using [NLayer](https://github.com/naudio/NLayer)
- Speaker personalization
- SpeakerToy pooling
- Automatic reader resolution by file type
- Cache for short audio clips
- FFmpeg-based audio processing for (almost) all formats, even over the network
- Windows-only Media Foundation support for a wider range of formats, and decoding over the network

> [!TIP]
> Check the [wiki](https://github.com/Axwabo/SecretLabNAudio/wiki) for more information.
> See [this page](https://github.com/Axwabo/SecretLabNAudio/wiki/Examples) for examples.

# Installation

## Single-File

1. Download the `SecretLabNAudio.zip` file from the [releases page](https://github.com/Axwabo/SecretLabNAudio/releases)
2. Extract `bin/SecretLabNAudio.dll` from the archive into the **global plugins** directory
   - Linux: `~/.config/SCP Secret Laboratory/LabAPI/plugins/global/`
   - Windows: `%appdata%/SCP Secret Laboratory/LabAPI/plugins/global/`
3. Restart the server

> [!IMPORTANT]
> The plugin **must** be placed in the **global plugins** directory to ensure that embedded dependencies are loaded.
> If you have a global plugin depending on SecretLabNAudio, rename it so it loads after SecretLabNAudio.

## Modular

1. Download the `SecretLabNAudio.Core.dll` file from the [releases page](https://github.com/Axwabo/SecretLabNAudio/releases)
2. Download the `SecretLabNAudio.zip` file from the releases page
3. Extract the necessary DLLs from the `bin/` directory
   - See the [table below](#modules) for what you need
   - Place dependencies into the **dependencies** directory
       - Linux: `~/.config/SCP Secret Laboratory/LabAPI/dependencies/<port>/`
       - Windows: `%appdata%/SCP Secret Laboratory/LabAPI/dependencies/<port>/`
   - Place plugins into the **plugins** directory
       - Linux: `~/.config/SCP Secret Laboratory/LabAPI/plugins/<port>/`
       - Windows: `%appdata%/SCP Secret Laboratory/LabAPI/plugins/<port>/`
4. Restart the server

### Modules

To support reading from some file formats, install the modules you need.

FFmpeg supports effectively all formats at the cost of running as a separate process.
The FFmpeg module's APIs must be invoked separately.

| Usage        | Plugin                             | Dependencies                                  |
|--------------|------------------------------------|-----------------------------------------------|
| **required** | (none)                             | `SecretLabNAudio.Core` `NAudio.Core`          |
| mp3          | `SecretLabNAudio.NLayer`           | `NLayer` `NLayer.NAudioSupport`               |
| ogg          | `SecretLabNAudio.NVorbis`          | `NVorbis` `NAudio.Vorbis` `System.ValueTuple` |
| most formats | `SecretLabNAudio.MediaFoundation`* | `NAudio.Wasapi`*                              |
| FFmpeg       | `SecretLabNAudio.FFmpeg`**         | (none)                                        |

> [!NOTE]
> *MediaFoundation is only available on Windows.
>
> **FFmpeg itself is not shipped with SecretLabNAudio.
> See the [wiki](https://github.com/Axwabo/SecretLabNAudio/wiki/FFmpeg-Installation) on how to install it.

## Development

Simply install the `SecretLabNAudio.Core` package from NuGet.
You can also add the `SecretLabNAudio.FFmpeg` package, which references the former one.

Manual installation:

1. Reference the `SecretLabNAudio.Core.dll` file from the [releases page](https://github.com/Axwabo/SecretLabNAudio/releases)
2. Install the `NAudio.Core` package from NuGet

> [!CAUTION]
> Most official NAudio packages are Windows-specific. Use the `NAudio.Core` package for cross-platform support.

> [!IMPORTANT]
> If you reference other NAudio packages, make sure you copy those dependencies to the LabAPI dependencies directory.
