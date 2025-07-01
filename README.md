# SecretLabNAudio

An advanced audio player API for SCP: Secret Laboratory using [NAudio](https://github.com/naudio/NAudio)

This library has a number of open-source dependencies. See [Attributions](ATTRIBUTIONS.md)

> [!NOTE]
> SecretLabNAudio is not sponsored nor endorsed by NAudio.

# Features

- Fully customizable audio provider interfaces
- Real-time audio processing
- Streaming files from disk
- Ogg Vorbis support using [NVorbis](https://github.com/NVorbis/NVorbis)
- MP3 support using [NLayer](https://github.com/naudio/NLayer)
- Speaker personalization
- SpeakerToy pooling
- Automatic reader resolution by file type
- Windows-only Media Foundation support for a wider range of formats, and decoding over the network

> [!TIP]
> Check the [wiki](https://github.com/Axwabo/SecretLabNAudio/wiki) for more information.

# Installation

## Single-File

1. Download the `SecretLabNAudio.zip` file from the [releases page](https://github.com/Axwabo/SecretLabNAudio/releases)
2. Extract `bin/SecretLabNAudio.dll` from the archive into the **global plugins** directory
    - Linux: `.config/SCP Secret Laboratory/LabAPI/plugins/global/`
    - Windows: `%appdata%/SCP Secret Laboratory/LabAPI/plugins/global/`
3. Restart the server

> [!IMPORTANT]
> The plugin **must** be placed in the **global plugins** directory to ensure that embedded dependencies are loaded.
> If you have a global plugin depending on SecretLabNAudio, rename it so it loads after SecretLabNAudio.

## Modular

1. Download the `SecretLabNAudio.Core.dll` file from the [releases page](https://github.com/Axwabo/SecretLabNAudio/releases)
2. Place the file in the **dependencies** directory
    - Linux: `.config/SCP Secret Laboratory/LabAPI/dependencies/<port>/`
    - Windows: `%appdata%/SCP Secret Laboratory/LabAPI/dependencies/<port>/`
3. Download the `SecretLabNAudio.zip` file from the releases page
4. Extract the necessary files from the `bin/` directory of the archive
    - `NAudio.Core.dll` is **always required**
    - `NLayer` and `NLayer.NAudioSpport.dll` for `.mp3` support (optional)
    - `NAudio.Vorbis.dll` and `NVorbis.dll` for `.ogg` support (optional)
5. Optionally download the necessary plugin from the releases page
    - `SecretLabNAudio.NLayer.dll` for `.mp3` support
    - `SecretLabNAudio.NVorbis.dll` for `.ogg` support
6. Place the downloaded plugin(s) into the **plugins** directory
    - Linux: `.config/SCP Secret Laboratory/LabAPI/plugins/<port>/`
    - Windows: `%appdata%/SCP Secret Laboratory/LabAPI/plugins/<port>/`
7. Restart the server

## Development

1. Reference the `SecretLabNAudio.Core.dll` file from the [releases page](https://github.com/Axwabo/SecretLabNAudio/releases)
2. Install the `NAudio.Core` package from NuGet

> [!CAUTION]
> Most official NAudio packages are Windows-specific. Use the `NAudio.Core` package for cross-platform support.

> [!IMPORTANT]
> If you reference other NAudio packages, make sure you copy those dependencies to the LabAPI dependencies directory.
