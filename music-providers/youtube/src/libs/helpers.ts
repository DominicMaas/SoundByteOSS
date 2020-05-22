/// <reference path="../../../soundbyte.d.ts" />

const clientKey = "AIzaSyACcC1JE0krWn90rfk5kVpx-Y8qkoqta40";

function toSbTrack(item: any): soundbyte.Track {
  var user = new soundbyte.User();
  user.userId = item.snippet.channelId;
  user.username = item.snippet.channelTitle;

  var track = new soundbyte.Track();
  track.trackId = item.id;
  track.title = item.snippet.title;
  track.description = item.snippet.description;
  track.duration = getDuration(item);
  track.user = user;
  track.artworkUrl = getThumbnail(item.snippet.thumbnails);

  // Detect if live
  track.isLive =
    item.snippet.liveBroadcastContent != undefined &&
    item.snippet.liveBroadcastContent != "none";

  return track;
}

function toSbUser(item: any): soundbyte.User {
  return new soundbyte.User();
}

function toSbPlaylist(item: any): soundbyte.Playlist {
  var user = new soundbyte.User();
  user.userId = item.snippet.channelId;
  user.username = item.snippet.channelTitle;

  var playlist = new soundbyte.Playlist();
  playlist.playlistId = item.id;
  playlist.title = item.snippet.title;
  playlist.description = item.snippet.description;
  playlist.user = user;
  playlist.artworkUrl = getThumbnail(item.snippet.thumbnails);

  return playlist;
}

function getThumbnail(thumbnails: any): string {
  // Handle if the video was deleted
  if (thumbnails == undefined) {
    return "https://soundbytemedia.com/images/512x512-logo.png";
  }

  if (thumbnails.maxres != null) {
    return thumbnails.maxres.url;
  }

  if (thumbnails.high != null) {
    return thumbnails.high.url;
  }

  if (thumbnails.medium != null) {
    return thumbnails.medium.url;
  }

  if (thumbnails.default != null) {
    return thumbnails.default.url;
  }

  return "https://soundbytemedia.com/images/512x512-logo.png";
}

function getDuration(item: any) {
  // No content details
  if (item.contentDetails == undefined) {
    return soundbyte.timeFromMilliseconds(0);
  }

  // No duration
  if (isEmpty(item.contentDetails.duration)) {
    // No end at
    if (item.contentDetails.endAt == undefined) {
      return soundbyte.timeFromMilliseconds(0);
    }

    // endAt is in seconds
    return soundbyte.timeFromMilliseconds(item.contentDetails.endAt * 100);
  }

  // Handle YouTube format
  soundbyte.timeFromMilliseconds(
    ytDurationToSeconds(item.contentDetails.duration) * 100
  );
}

function isEmpty(str: string): boolean {
  return !str || 0 === str.length;
}

function ytDurationToSeconds(duration: string) {
  var match = duration.match(/PT(\d+H)?(\d+M)?(\d+S)?/);

  match = match.slice(1).map(function(x) {
    if (x != null) {
      return x.replace(/\D/, "");
    }
  });

  var hours = parseInt(match[0]) || 0;
  var minutes = parseInt(match[1]) || 0;
  var seconds = parseInt(match[2]) || 0;

  return hours * 3600 + minutes * 60 + seconds;
}
