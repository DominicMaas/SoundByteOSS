function getMediaStream(trackId) {
    var id = playbackIds[Math.floor(Math.random() * playbackIds.length)];
    return "https://api.soundcloud.com/tracks/" + trackId + "/stream?client_id=" + id;
}
function getTopTracks(count, token, parameters) {
    return getExploreItems(count, token, parameters, "top");
}
function getTrendingTracks(count, token, parameters) {
    return getExploreItems(count, token, parameters, "trending");
}
function getExploreItems(count, token, parameters, kind) {
    var filter = parameters["filter"] || "all-music";
    var genre = "soundcloud%3Agenres%3A" + filter;
    var returnTracks = new Array();
    var uri = "https://api-v2.soundcloud.com/charts?kind=" + kind + "&genre=" + genre + "&limit=" + count + "&offset=" + token + "&linked_partitioning=1&client_id=" + "sCFubJLD5LtMoZI4xUu46btuIqerNC84";
    var data = JSON.parse(network.getString(uri));
    var nextUrl = data.next_href;
    var extractedToken = null;
    if (nextUrl != null) {
        var matches = nextUrl.match(/offset=([^&]*)/);
        extractedToken = matches[0].substring(7, matches[0].length);
    }
    if (data.collection.length == 0) {
        return new SourceResponse(null, null, false, "No results found", "No items matching");
    }
    data.collection.forEach(function (item) {
        if (item.track != null) {
            var sbTrack = toSbTrack(item.track);
            if (sbTrack != null) {
                returnTracks.push(fromTrack(sbTrack));
            }
        }
    });
    return new SourceResponse(returnTracks, extractedToken, true, "", "");
}
function navigateTopTracks(parent) {
    navigateToExploreView(parent, "Top 50 SoundCloud Tracks");
}
function navigateTrendingTracks(parent) {
    navigateToExploreView(parent, "New & Hot SoundCloud Tracks");
}
function navigateToExploreView(parent, title) {
    navigation.navigateTo(PageName.FilteredListView, new FilteredListViewHolder(parent.collection, title, [
        new FilterViewItem(true, "General"),
        new FilterViewItem("All Music Genres", "all-music"),
        new FilterViewItem("All Audio Genres", "all-audio"),
        new FilterViewItem(true, "Music"),
        new FilterViewItem("Alternative Rock", "alternativerock"),
        new FilterViewItem("Ambient", "ambient"),
        new FilterViewItem("Classical", "classical"),
        new FilterViewItem("Country", "country"),
        new FilterViewItem("Dance & EDM", "danceedm"),
        new FilterViewItem("Dancehall", "dancehall"),
        new FilterViewItem("Deep House", "deephouse"),
        new FilterViewItem("Disco", "disco"),
        new FilterViewItem("Drum & Bass", "drumbass"),
        new FilterViewItem("Dubstep", "dubstep"),
        new FilterViewItem("Electronic", "electronic"),
        new FilterViewItem("Folk & Singer-Songwriter", "folksingersongwriter"),
        new FilterViewItem("Hip Hop & Rap", "hiphoprap"),
        new FilterViewItem("House", "house"),
        new FilterViewItem("Indie", "indie"),
        new FilterViewItem("Jazz & Blues", "jazzblues"),
        new FilterViewItem("Latin", "latin"),
        new FilterViewItem("Metal", "metal"),
        new FilterViewItem("Piano", "piano"),
        new FilterViewItem("Pop", "pop"),
        new FilterViewItem("R&B & Soul", "rbsoul"),
        new FilterViewItem("Reggae", "reggae"),
        new FilterViewItem("Reggaeton", "reggaeton"),
        new FilterViewItem("Rock", "rock"),
        new FilterViewItem("Soundtrack", "soundtrack"),
        new FilterViewItem("Techno", "techno"),
        new FilterViewItem("Trance", "trance"),
        new FilterViewItem("Trap", "trap"),
        new FilterViewItem("Triphop", "triphop"),
        new FilterViewItem("World", "world"),
        new FilterViewItem(true, "Audio"),
        new FilterViewItem("Audiobooks", "audiobooks"),
        new FilterViewItem("Business", "business"),
        new FilterViewItem("Comedy", "comedy"),
        new FilterViewItem("Entertainment", "entertainment"),
        new FilterViewItem("Learning", "learning"),
        new FilterViewItem("News & Politics", "newspolitics"),
        new FilterViewItem("Religion & Spirituality", "religionspirituality"),
        new FilterViewItem("Science", "science"),
        new FilterViewItem("Sports", "sports"),
        new FilterViewItem("Storytelling", "storytelling"),
        new FilterViewItem("Technology", "technology")
    ]));
}
var clientId = "gU5Rw9VDiPPA4OcDlC8VVcb19sHDZFTT";
var playbackIds = ["gU5Rw9VDiPPA4OcDlC8VVcb19sHDZFTT", "ytXCP8DpxZPd96FN12KsjT1P2mSHglXH", "59f81c512bd8eda616a21851093b2f16", "8547e755a4a625d4be8f243c1c7756a9", "0452ba585c12c2a37a143aca3b426b19"];
function toSbTrack(item) {
    try {
        var user = toSbUser(item.user);
        var artworkUrl = item.artwork_url || user.artworkUrl;
        var thumbnailUrl = item.thumbnailUrl || user.thumbnailUrl;
        if (artworkUrl.indexOf("large") != -1) {
            artworkUrl = artworkUrl.replace("large", "t500x500");
        }
        if (thumbnailUrl.indexOf("large") != -1) {
            thumbnailUrl = thumbnailUrl.replace("large", "t300x300");
        }
        var track = new Track();
        track.serviceType = 1;
        track.trackId = item.id;
        track.link = item.permalink_url;
        track.artworkUrl = artworkUrl;
        track.thumbnailUrl = thumbnailUrl;
        track.created = item.created_at;
        track.duration = utils.timeFromMilliseconds(item.duration);
        track.description = item.description;
        track.title = item.title;
        track.user = user;
        track.likeCount = item.likes_count;
        track.viewCount = item.playback_count;
        track.commentCount = item.comment_count;
        track.genre = item.genre;
        return track;
    }
    catch (e) {
        log("ERROR: " + e);
        return null;
    }
}
function toSbUser(item) {
    var defaultUser = "http://a1.sndcdn.com/images/default_avatar_large.png";
    var user = new User();
    user.serviceType = 1;
    user.userId = item.id;
    user.username = item.username;
    user.country = item.country_code;
    user.artworkUrl = item.avatar_url || defaultUser;
    user.thumbnailUrl = item.avatar_url || defaultUser;
    return user;
}
function toSbPlaylist(item) {
    var playlist = new Playlist();
    playlist.serviceType = 1;
    return playlist;
}