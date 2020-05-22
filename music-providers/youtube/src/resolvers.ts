/// <reference path="../../soundbyte.d.ts" />

/**
 * Gets the video stream url for the music provider.
 * @param {*} trackId The track we need the music stream for.
 */
function getVideoStream(trackId: string) {
  return soundbyte.interop.youtube.getVideoStream(trackId);
}

/**
 * Gets the audio stream url for the music provider.
 * @param {*} trackId The track we need the music stream for.
 */
function getAudioStream(trackId: string) {
  return soundbyte.interop.youtube.getAudioStream(trackId);
}

/**
 * Resolves a list of tracks for a playlist
 * @param count How many items to get
 * @param token Where the next data is
 * @param parameters The playlist id to get tracks for parameters["playlistId"]
 */
function resolvePlaylistTracks(count: number, token: string, parameters: any) {
  const playlistId = parameters["playlistId"];

  // Temp array that will store the return items
  var returnItems = new Array<soundbyte.Media>();

  // Construct the URL
  var uri =
    "https://www.googleapis.com/youtube/v3/playlistItems?part=id,snippet,contentDetails&maxResults=" +
    count +
    "&pageToken=" +
    token +
    "&playlistId=" +
    playlistId +
    "&key=" +
    clientKey;

  // Get a response from the YouTube API, and parse
  // it into an object.
  var data = JSON.parse(soundbyte.network.performRequest(uri));

  // Handle when there are no items
  if (data.items.length == 0) {
    return new soundbyte.SourceResponse(
      "No Music",
      "There is no music in this playlist"
    );
  }

  // Convert the YouTube playlists into SoundByte objects.
  data.items.forEach(function(item: any) {
    // Only YouTube videos / playlist items
    if (item.kind == "youtube#video") {
      returnItems.push(toSbTrack(item));
    }

    if (item.kind == "youtube#playlistItem") {
      // Playlist item stores ID under resourceId
      let t = toSbTrack(item);
      t.trackId = item.snippet.resourceId.videoId;

      returnItems.push(t);
    }
  });

  // Return the items back to SoundByte
  return new soundbyte.SourceResponse(returnItems, data.nextPageToken);
}
