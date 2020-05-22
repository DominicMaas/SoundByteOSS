# SoundCloud for SoundByte

This extension is a music provider for SoundByte that provides support for SoundCloud. The code at the moment will change a lot because the extension system is still being developed.

## Provided Features

- [ ] Trending and top tracks on the explore page.
- [ ] SoundCloud stream on the "for you" page.
- [ ] SoundCloud weekly on the "for you" page.

## How to build

This extension does not use TypeScript, but instead makes use of the TypeScript CLI.

1. `npm install -g typescript`
2. `tsc`

## How to run

1. Run SoundByte.
2. Click on the extensions icon on the top right (or click on the '...' button and click 'Extensions').
3. Click "Load unpacked extension".
4. navigate to the root folder containing the `manifest.json` file.

**Please Note**: Depending on your SoundByte version, SoundCloud is considered a core extension and cannot be overridden. If this is the case, please upgrade your SoundByte client.