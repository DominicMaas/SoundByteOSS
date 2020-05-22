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

## Build Status

|Platform|CI|Staging|Store|
|---|---|---|---|
| UWP | [![Build status](https://dev.azure.com/soundbyte/SoundByte%20Build%20Process/_apis/build/status/UWP-CI)](https://dev.azure.com/soundbyte/SoundByte%20Build%20Process/_build/latest?definitionId=21) | - | - |
| Android | [![Build status](https://dev.azure.com/soundbyte/SoundByte%20Build%20Process/_apis/build/status/Android-CI)](https://dev.azure.com/soundbyte/SoundByte%20Build%20Process/_build/latest?definitionId=24) | - | - |
| iOS | [![Build status](https://dev.azure.com/soundbyte/SoundByte%20Build%20Process/_apis/build/status/iOS-CI)](https://dev.azure.com/soundbyte/SoundByte%20Build%20Process/_build/latest?definitionId=27) | ![TestFlight](https://vsrm.dev.azure.com/SoundByte/_apis/public/Release/badge/f48f04c8-e1e9-4eec-84a1-29f6abd2ec99/1/1) | -  |
| Web | [![Build status](https://dev.azure.com/soundbyte/SoundByte%20Build%20Process/_apis/build/status/Web-CI)](https://dev.azure.com/soundbyte/SoundByte%20Build%20Process/_build/latest?definitionId=19) | - | - |
| Dashboard | [![Build status](https://dev.azure.com/soundbyte/SoundByte%20Build%20Process/_apis/build/status/Dashboard-CI)](https://dev.azure.com/soundbyte/SoundByte%20Build%20Process/_build/latest?definitionId=30) | - | - |

## Music Providers
If you have any issues or feature requests for certain music providers (e.g. playback bug with YouTube), please create an issue.

## Feature Requests
Try to provide as much information as possible, including screenshots or mock ups when applicable.
