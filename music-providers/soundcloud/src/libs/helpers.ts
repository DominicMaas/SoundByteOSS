/// <reference path="../../../soundbyte.d.ts" />

const clientId = "gU5Rw9VDiPPA4OcDlC8VVcb19sHDZFTT";
const playbackIds = [
  "gU5Rw9VDiPPA4OcDlC8VVcb19sHDZFTT",
  "ytXCP8DpxZPd96FN12KsjT1P2mSHglXH",
  "59f81c512bd8eda616a21851093b2f16",
  "8547e755a4a625d4be8f243c1c7756a9",
  "0452ba585c12c2a37a143aca3b426b19"
];

/**
 * Convert a SoundCloud track to a SoundByte
 * compatible track.
 */
function toSbTrack(item: any): soundbyte.Track {
  // Convert the user class
  var user = toSbUser(item.user);

  // Calculate the required image sizes
  var artworkUrl = item.artwork_url || user.artworkUrl;

  // If the artwork url contains large, replace with the correct size.
  if (artworkUrl.indexOf("large") != -1) {
    artworkUrl = artworkUrl.replace("large", "t500x500");
  }

  // Build the track object
  var track = new soundbyte.Track();
  track.trackId = item.id;
  track.link = item.permalink_url;
  track.artworkUrl = artworkUrl;
  track.created = item.created_at;
  track.duration = soundbyte.timeFromMilliseconds(item.duration);
  track.description = item.description;
  track.title = item.title;
  track.user = user;

  return track;
}

/**
 * Convert a SoundCloud user to a SoundByte
 * compatible user.
 */
function toSbUser(item: any): soundbyte.User {
  var defaultUser = "http://a1.sndcdn.com/images/default_avatar_large.png";

  var user = new soundbyte.User();
  user.userId = item.id;
  user.username = item.username;
  user.artworkUrl = item.avatar_url || defaultUser;

  return user;
}

/**
 * Convert a SoundCloud playlist to a SoundByte
 * compatible playlist.
 */
function toSbPlaylist(item: any): soundbyte.Playlist {
  // Convert the user class
  var user = toSbUser(item.user);

  // Calculate the required image sizes
  var artworkUrl = item.artwork_url || user.artworkUrl;

  // If the artwork url contains large, replace with the correct size.
  if (artworkUrl.indexOf("large") != -1) {
    artworkUrl = artworkUrl.replace("large", "t500x500");
  }

  // Build the playlist object
  var playlist = new soundbyte.Playlist();
  playlist.playlistId = item.id;
  playlist.link = item.permalink_url;
  playlist.artworkUrl = artworkUrl;
  playlist.created = item.created_at;
  playlist.duration = soundbyte.timeFromMilliseconds(item.duration);
  playlist.description = item.description;
  playlist.title = item.title;
  playlist.user = user;

  return playlist;
}

function isEmpty(str: string): boolean {
  return !str || 0 === str.length;
}
