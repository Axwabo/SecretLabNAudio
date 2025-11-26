# SecretLabNAudio Demo

[Watch the demo](https://youtu.be/6btaXm3BU8s)

This project showcases a semi-complex use case of SecretLabNAudio.

Check out the `DiscJockeyAudioProcessor` class.

The `Board` namespace contains a schematic builder and controller.
Audio processing itself is done in the aforementioned class.

> [!NOTE]
> The `DiscJockeySampleProvider` was made for SecretLabNAudio v1.
> While they do effectively the same thing, the design approach is different.
> With v2, you no longer have to worry about disposing each input yourself.

A DJ board is created in the "tutorial room" on surface.
To play a song, use the `DJ` command with a file path.
You can talk in-game to have your voice be mixed in with the music.
You can control the music's speed, your voice's pitch, and each input's volume,
as well as the master volume. A visualizer shows the waveform on the drawing board.
Audio is played through all speakers on surface spatially ("3D" sound),
and can be heard at full volume in the "tutorial room" (as "2D" sound).

To grab a slider, set a keybind in the Server-Specific Settings menu.
To reset a slider, interact (E) with the button below it.
