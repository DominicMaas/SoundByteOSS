// MEDIA PLAYBACK

/**
 * Gets the auto stream url for the music provider.
 * @param {*} trackId The track we need the music stream for.
 */
function getMediaStream(trackId) {
    // SoundCloud has a fixed rate on playbacks. This system 
    // chooses a key on random and plays from it.
    var id = playbackIds[Math.floor(Math.random() * playbackIds.length)];
    return "https://api.soundcloud.com/tracks/" + trackId + "/stream?client_id=" + id;
}


// CONTENT GROUPS

function getTopTracks(count, token, parameters) {
    return getExploreItems(count, token, parameters, "top");
}

function getTrendingTracks(count, token, parameters) {
    return getExploreItems(count, token, parameters, "trending");
}

function getExploreItems(count, token, parameters, kind) {
    var filter = parameters["filter"] || "all-music";
    var genre = "soundcloud%3Agenres%3A" + filter;

    // Temp array that will store the return tracks
    var returnTracks = new Array();

    // Construct the URL
    var uri = "https://api-v2.soundcloud.com/charts?kind=" + kind + "&genre=" + genre + "&limit=" + count + "&offset=" + token + "&linked_partitioning=1&client_id=" + clientId;

    // Get a response from the SoundCloud API, and parse
    // it into an object.
    // var data = sb.network.get(uri);
    var data = JSON.parse(sb.network.getString(uri));

    // Extract the next offset / token
    var nextUrl = data.next_href;
    var extractedToken = null;

    if (nextUrl != null) {
        var matches = nextUrl.match(/offset=([^&]*)/);
        extractedToken = matches[0].substring(7, matches[0].length);
    }

    // Handle when there are no items
    if (data.collection.length == 0) {
        return new SourceResponse(null, null, false, "No results found", "No items matching");
    }

    // Convert the SoundCloud objects int SoundByte objects.
    data.collection.forEach(function (item) {
        if (item.track != null) {
            var sbTrack = toSbTrack(item.track);
            if (sbTrack != null) {
                returnTracks.push(new GenericItem(sbTrack));
            }
        }
    });

    // Return the tracks back to SoundByte
    return new SourceResponse(returnTracks, extractedToken, true, "", "");
}

function navigateTopTracks(parent) {
    navigateToExploreView(parent, "Top 50 SoundCloud Tracks");
}

function navigateTrendingTracks(parent) {
    navigateToExploreView(parent, "New & Hot SoundCloud Tracks");
}

function navigateToExploreView(parent, title) {
    sb.navigation.navigateTo(PageName.FilteredListView, new FilteredListViewHolder(parent.collection, title, [
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