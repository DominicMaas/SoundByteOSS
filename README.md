<h1 align="center">
SoundByte
</h1>

<img src="https://urdzyq.dm.files.1drv.com/y4mHnS7xpObcTANZwpA20OzxBmVfBwH29na9RL-7sc7VwUSv2avOzn2lEvtAtbfnzBRr_aXeu0IyaKDRNqY4yeeT0LrR16wgbzilxFpZlE5VQxckBdkgxa_EiklMZ6218rXVlW1k_oKZnlvSlFtRX48_ysKj6vIKyZRQ-_KoQJRLDwR-Siu7nmMCBUEFUYmGem0y97uapYO01h0GDk-BxpGdA?width=3840&height=1790&cropmode=none" alt="SoundByte Screenshot">

<h4 align="center">An extensible music client for Windows 10 &amp; Xbox One. Future support for iOS &amp; Android.</h4>

## Introduction

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
- The legacy core library 
- The new Amalthea engine (rewritten core library to be fully service modular (via extensions) and cross platform)
- The in beta iOS app
- The in beta UWP app

## What's not included
- The web application (server side functionality)
- `Constants.cs`
- `google-services.json`
- `GoogleService-Info.plist`

## Build Status (Legacy)

|Platform|CI|Staging|Store|
|---|---|---|---|
| UWP | [![Build status](https://dev.azure.com/SoundByte/SoundByte/_apis/build/status/UWP%20Build%20(Legacy))](https://dev.azure.com/SoundByte/SoundByte/_build/latest?definitionId=14) | ![Build Status](https://vsrm.dev.azure.com/SoundByte/_apis/public/Release/badge/1e68b765-71a7-467e-b205-edb6c6ce9d91/2/2) | - |

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
