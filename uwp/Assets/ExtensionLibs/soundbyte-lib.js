// ----- METHODS ----- //

// use invokeNativePlatformAsync when not expecting a result
// use invokeNativePlatform when expecting a result

var navigation = {
  /**
   *
   * @param {*} pageName The page name to navigate to. See PageName
   * @param {*} args Arguments for the page
   */
  navigateTo: function(pageName, args) {
    platform.invokeNativePlatformAsync("navigateTo", pageName, args);
  }
};

var network = {
  /**
   * Fetch a url and return it as a string
   * @param {*} uri The url to fetch
   */
  getString: function(uri) {
    return platform.invokeNativePlatform("httpGetString", uri);
  }
};

var utils = {
  timeFromMilliseconds: function(duration) {
    return platform.invokeNativePlatform("timeFromMilliseconds", duration);
  }
};

/**
 * Logs a message to the SoundByte Console
 * @param {*} message The message to log
 */
function log(message) {
  platform.invokeNativePlatformAsync("log", message);
}

function fromTrack(track) {
  var item = new GenericItem();
  item.track = track;
  item.type = 1;
  return item;
}

function fromPlaylist(playlist) {
  var item = new GenericItem();
  item.playlist = playlist;
  item.type = 3;
  return item;
}

function fromUser(user) {
  var item = new GenericItem();
  item.user = user;
  item.type = 2;
  return item;
}

// ----- OBJECTS ----- //
