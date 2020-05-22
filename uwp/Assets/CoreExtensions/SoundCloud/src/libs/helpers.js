const clientId = "gU5Rw9VDiPPA4OcDlC8VVcb19sHDZFTT";
const playbackIds = ["gU5Rw9VDiPPA4OcDlC8VVcb19sHDZFTT", "ytXCP8DpxZPd96FN12KsjT1P2mSHglXH", "59f81c512bd8eda616a21851093b2f16", "8547e755a4a625d4be8f243c1c7756a9", "0452ba585c12c2a37a143aca3b426b19"];

/**
 * Convert a SoundCloud track to a SoundByte 
 * compatible track.
 */
function toSbTrack(item) {
    try {
        // Convert the user class
        var user = toSbUser(item.user);

        // Calculate the required image sizes
        var artworkUrl = item.artwork_url || user.artworkUrl;
        var thumbnailUrl = item.thumbnailUrl || user.thumbnailUrl;

        // If the artwork url contains large, replace with the correct size.
        if (artworkUrl.indexOf("large") != -1) {
            artworkUrl = artworkUrl.replace("large", "t500x500");
        }

        // If the thumbnail url contains large, replace with the correct size.
        if (thumbnailUrl.indexOf("large") != -1) {
            thumbnailUrl = thumbnailUrl.replace("large", "t300x300");
        }

        // Build the track object
        var track = new Track();
        track.serviceType = 1; // TEMP
        track.trackId = item.id;
        track.link = item.permalink_url;
        track.artworkUrl = artworkUrl;
        track.thumbnailUrl = thumbnailUrl;
        track.created = item.created_at;
        // SoundByte expects a TimeSpan, so convert the double to a timespan using utils
        track.duration = sb.utils.timeFromMilliseconds(item.duration);
        track.description = item.description;
        track.title = item.title;
        track.user = user;
        track.likeCount = item.likes_count;
        track.viewCount = item.playback_count;
        track.commentCount = item.comment_count;
        track.genre = item.genre;

        return track;
    } catch (e) {
        sb.log("ERROR: " + e);
        return null;
    }
}

/**
 * Convert a SoundCloud user to a SoundByte 
 * compatible user.
 */
function toSbUser(item) {
    var defaultUser = "http://a1.sndcdn.com/images/default_avatar_large.png";

    var user = new User();
    user.serviceType = 1; // TEMP
    user.userId = item.id;
    user.username = item.username;
    user.country = item.country_code;
    user.artworkUrl = item.avatar_url || defaultUser;
    user.thumbnailUrl = item.avatar_url || defaultUser;

    return user;
}

/**
 * Convert a SoundCloud playlist to a SoundByte 
 * compatible playlist.
 */
function toSbPlaylist(item) {
    var playlist = new Playlist();
    playlist.serviceType = 1; // TEMP

    return playlist;
}