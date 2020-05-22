/// <reference path="../../soundbyte.d.ts" />
/// <reference path="resolvers.ts" />

// CONTENT GROUPS

function getTrending(count: number, token: string, parameters: any) {
  // Temp array that will store the return items
  var returnItems = new Array<soundbyte.Media>();

  // Construct the URL
  var uri =
    "https://www.googleapis.com/youtube/v3/videos?part=snippet,contentDetails&chart=mostPopular&maxResults=" +
    count +
    "&videoCategoryId=10&pageToken=" +
    token +
    "&key=" +
    clientKey;

  // Get a response from the YouTube API, and parse
  // it into an object.
  var data = JSON.parse(soundbyte.network.performRequest(uri));

  // Handle when there are no items
  if (data.items.length == 0) {
    return new soundbyte.SourceResponse(
      "No results found",
      "There are no trending YouTube videos."
    );
  }

  // Convert the YouTube tracks into SoundByte objects.
  data.items.forEach(function(item: any) {
    // Only YouTube videos
    if (item.kind == "youtube#video") {
      returnItems.push(toSbTrack(item));
    }
  });

  // Return the items back to SoundByte
  return new soundbyte.SourceResponse(returnItems, data.nextPageToken);
}

function getUserLikes(count: number, token: string, parameters: any) {
  // Temp array that will store the return items
  var returnItems = new Array<soundbyte.Media>();

  // Construct the URL
  var uri =
    "https://www.googleapis.com/youtube/v3/videos?part=id,snippet,contentDetails&myRating=like&maxResults=" +
    count +
    "&videoCategoryId=10&pageToken=" +
    token +
    "&key=" +
    clientKey;

  // Get a response from the YouTube API, and parse
  // it into an object.
  var data = JSON.parse(soundbyte.network.performRequest(uri));

  // Handle when there are no items
  if (data.items.length == 0) {
    return new soundbyte.SourceResponse(
      "No results found",
      "Like some videos on Youtube to get started"
    );
  }

  // Convert the YouTube tracks into SoundByte objects.
  data.items.forEach(function(item: any) {
    // Only YouTube videos
    if (item.kind == "youtube#video") {
      returnItems.push(toSbTrack(item));
    }
  });

  // Return the items back to SoundByte
  return new soundbyte.SourceResponse(returnItems, data.nextPageToken);
}

function getUserPlaylists(count: number, token: string, parameters: any) {
  // Temp array that will store the return items
  var returnItems = new Array<soundbyte.Media>();

  // Construct the URL
  var uri =
    "https://www.googleapis.com/youtube/v3/playlists?part=id,snippet,contentDetails&mine=true&maxResults=" +
    count +
    "&pageToken=" +
    token +
    "&key=" +
    clientKey;

  // Get a response from the YouTube API, and parse
  // it into an object.
  var data = JSON.parse(soundbyte.network.performRequest(uri));

  // Handle when there are no items
  if (data.items.length == 0) {
    return new soundbyte.SourceResponse(
      "No results found",
      "Create some playlists on Youtube to get started"
    );
  }

  // Convert the YouTube playlists into SoundByte objects.
  data.items.forEach(function(item: any) {
    // Only YouTube videos
    if (item.kind == "youtube#playlist") {
      returnItems.push(toSbPlaylist(item));
    }
  });

  // Return the items back to SoundByte
  return new soundbyte.SourceResponse(returnItems, data.nextPageToken);
}

function getSearchedTracks(count: number, token: string, parameters: any) {
  return handleSearch("video", parameters["query"], count, token);
}

function getSearchedPlaylists(count: number, token: string, parameters: any) {
  return handleSearch("playlist", parameters["query"], count, token);
}

function getSearchedUsers(count: number, token: string, parameters: any) {
  return new soundbyte.SourceResponse("Not Implemented", "Not Implemented");
}

function handleSearch(
  type: string,
  query: string,
  count: number,
  token: string
) {
  // Setup
  let returnItems = new Array<soundbyte.Media>();

  // Construct the URL and get a list of ids from the search
  const uri = `https://www.googleapis.com/youtube/v3/search?part=id&maxResults=${count}&pageToken=${token}&type=${type}&key=${clientKey}&q=${query}`;
  let data = JSON.parse(soundbyte.network.performRequest(uri));

  // Handle when there are no items
  if (data.items.length == 0) {
    return new soundbyte.SourceResponse(
      "No results found",
      "Could not find any results matching " + query
    );
  }

  // Get list of ids
  let idList = "";
  data.items.forEach((item: any) => {
    idList = idList + item.id[type + "Id"] + ",";
  });

  // Get the extended data
  let extendedData = getExtendedList(type + "s", idList);

  // Convert the items
  extendedData.items.forEach((item: any) => {
    // Only the kind that we want
    if (item.kind == "youtube#" + type) {
      switch (type) {
        case "video":
          returnItems.push(toSbTrack(item));
          break;

        case "playlist":
          returnItems.push(toSbPlaylist(item));
          break;
      }
    }
  });

  // Return the items back to SoundByte
  return new soundbyte.SourceResponse(returnItems, data.nextPageToken);
}

/**
 *
 * @param type The type of resource. video, playlist, user
 * @param idList Comma seperated list of ids
 */
function getExtendedList(type: string, idList: string): any {
  const uri = `https://www.googleapis.com/youtube/v3/${type}?part=snippet,contentDetails&key=${clientKey}&id=${idList}`;
  return JSON.parse(soundbyte.network.performRequest(uri));
}
