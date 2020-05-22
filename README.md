# SoundByte
The current released SoundByte App for UWP

TODO: Explain everything

## Reporting Bugs

To leave feedback, click on the `Issues` tab above. You will need a GitHub account. If you don't have an account, you can also tweet the issue to @SoundByteUWP.

## TODO
- [ ] Setup the CI engine to process legacy builds.
- [ ] Use GitHub releases
- [ ] Setup automatic staging releases
- [ ] Move all repos to this repo

## What's included
- The publicly released SoundByte app source code (for UWP)
- The core library 

## What's not included
- The web application (server side functionality)
- The new Amalthea engine (rewritten core library to be fully service modular (via extensions) and cross platform)
- The in beta iOS app
- The in beta UWP app

The Amalthea engine, iOS and UWP will eventually be open sourced (the UWP app still requires a lot of code to be ported from the legacy app). The web application is not planned to be open sourced.

## Build Status (Legacy)

|Platform|CI|Staging|Store|
|---|---|---|---|
| UWP | [![Build status](https://dev.azure.com/SoundByte/SoundByte/_apis/build/status/UWP%20Build%20(Legacy))](https://dev.azure.com/SoundByte/SoundByte/_build/latest?definitionId=14) | - | - |

## Build Status (Amalthea)

|Platform|CI|Staging|Release|
|---|---|---|---|
| UWP | [![Build status](https://dev.azure.com/soundbyte/SoundByte%20Build%20Process/_apis/build/status/UWP-CI)](https://dev.azure.com/soundbyte/SoundByte%20Build%20Process/_build/latest?definitionId=21) | [![Build status](https://dev.azure.com/soundbyte/SoundByte%20Build%20Process/_apis/build/status/UWP-Staging)](https://dev.azure.com/soundbyte/SoundByte%20Build%20Process/_build/latest?definitionId=22) | [![Build status](https://dev.azure.com/soundbyte/SoundByte%20Build%20Process/_apis/build/status/UWP-Production)](https://dev.azure.com/soundbyte/SoundByte%20Build%20Process/_build/latest?definitionId=23) |
| Android | [![Build status](https://dev.azure.com/soundbyte/SoundByte%20Build%20Process/_apis/build/status/Android-CI)](https://dev.azure.com/soundbyte/SoundByte%20Build%20Process/_build/latest?definitionId=24) | [![Build status](https://dev.azure.com/soundbyte/SoundByte%20Build%20Process/_apis/build/status/Android-Staging)](https://dev.azure.com/soundbyte/SoundByte%20Build%20Process/_build/latest?definitionId=25) | [![Build status](https://dev.azure.com/soundbyte/SoundByte%20Build%20Process/_apis/build/status/Android-Production)](https://dev.azure.com/soundbyte/SoundByte%20Build%20Process/_build/latest?definitionId=26) |
| iOS | [![Build status](https://dev.azure.com/soundbyte/SoundByte%20Build%20Process/_apis/build/status/iOS-CI)](https://dev.azure.com/soundbyte/SoundByte%20Build%20Process/_build/latest?definitionId=27) | [![Build status](https://dev.azure.com/soundbyte/SoundByte%20Build%20Process/_apis/build/status/iOS-Staging)](https://dev.azure.com/soundbyte/SoundByte%20Build%20Process/_build/latest?definitionId=28) | [![Build status](https://dev.azure.com/soundbyte/SoundByte%20Build%20Process/_apis/build/status/iOS-Production)](https://dev.azure.com/soundbyte/SoundByte%20Build%20Process/_build/latest?definitionId=29) |
| Web | [![Build status](https://dev.azure.com/soundbyte/SoundByte%20Build%20Process/_apis/build/status/Web-CI)](https://dev.azure.com/soundbyte/SoundByte%20Build%20Process/_build/latest?definitionId=19) | [![Build status](https://dev.azure.com/soundbyte/SoundByte%20Build%20Process/_apis/build/status/Web-Staging)](https://dev.azure.com/soundbyte/SoundByte%20Build%20Process/_build/latest?definitionId=18) | [![Build status](https://dev.azure.com/soundbyte/SoundByte%20Build%20Process/_apis/build/status/Web-Production)](https://dev.azure.com/soundbyte/SoundByte%20Build%20Process/_build/latest?definitionId=20) |

## Music Providers
If you have any issues or feature requests for certain music providers (e.g. playback bug with YouTube), please create an issue.

## Feature Requests
Try to provide as much information as possible, including screenshots or mock ups when applicable.
