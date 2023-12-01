class Helper {
    static TrimEndUntilChar_nochecks(str, stopChar) {
        let charsTotal = str.length;
        var sub = 0;
        var i = charsTotal;
        while (i != 0) {
            if (str[i - 1] === stopChar) {
                sub = charsTotal - (charsTotal - i);
                break;
            }
            i--;
        }
        return str.substring(0, sub);
    }
    static getMediaSource(preferManagedMediaSource = true) {
        if (typeof self === 'undefined')
            return undefined;
        const mms = (preferManagedMediaSource || !self.MediaSource) &&
            self.ManagedMediaSource;
        return (mms ||
            self.MediaSource ||
            self.WebKitMediaSource);
    }
}
class HttpClient {
    // async
    static GetFileAsString(url, onsuccess) {
        const xhr = new XMLHttpRequest();
        const _url = url;
        xhr.open("GET", _url);
        xhr.send();
        xhr.onreadystatechange = (e) => {
            if (xhr.readyState === XMLHttpRequest.DONE) {
                const status = xhr.status;
                if (status === 0 || (status >= 200 && status < 400)) {
                    onsuccess(xhr.responseText);
                }
                else {
                    //err = "error!";
                }
            }
        };
    }
    static GetBlob(url, onsuccess) {
        const xhr = new XMLHttpRequest();
        const _url = url;
        xhr.open("GET", _url);
        xhr.responseType = "blob";
        xhr.onload = (e) => {
            onsuccess(xhr.response);
        };
        xhr.send();
    }
}
class Manifest {
    constructor() {
        // 180
        // 360
        // 720
        this.playlists = ["", "", ""];
    }
}
class Playlist {
    constructor() {
        this.segments = [];
        this.url = "";
    }
}
class Segment {
    constructor(url) {
        this.filename = "";
        this.blob = null;
        this.blobUrl = "";
        this.filename = url;
    }
}
export class SegmentController {
    constructor() {
        //public egmentLoaded: SegmentLoadedCallback | undefined;
        this.segmentBank = [];
        //public NextSegments(count: number): Array<Segment> {
        //    var ret: Array<Segment> = [];
        //    var i: number = 0;
        //    while (count != 0) {
        //        var seg = this.segmentBank[i - 1];
        //        ret.push(seg);
        //    }
        //    return ret;
        //}
    }
    ApplyPlaylist(playlist) {
        var segments = playlist.segments;
        for (var i = 0; i < segments.length; i++) {
            var seg = segments[i];
            var segUrl = playlist.url + seg.filename;
            HttpClient.GetBlob(segUrl, (x) => this.OnSegmentLoaded(seg, x));
        }
    }
    OnSegmentLoaded(segment, videoFileBlob) {
        segment.blobUrl = URL.createObjectURL(videoFileBlob);
        this.segmentBank.push(segment);
    }
}
export class PlaylistController {
    constructor() {
        this.items = [];
    }
    Update(playlistUrl) {
        HttpClient.GetFileAsString(playlistUrl, (x) => this.OnStringLoaded(x, playlistUrl));
    }
    OnStringLoaded(playlistText, playlistUrl) {
        if (playlistText !== "" && playlistText !== null) {
            // clear
            this.items = [];
            let pl = this.ParsePlaylist(playlistText);
            pl.url = Helper.TrimEndUntilChar_nochecks(playlistUrl, "/");
            var handler = this.plalistLoaded;
            if (handler != undefined) {
                handler(pl);
            }
        }
    }
    ParsePlaylist(text) {
        let playlist = new Playlist();
        var charsTotal = text.length;
        var charCount = 0;
        var segmentLine = false;
        var str = "";
        while (charCount != charsTotal) {
            var char = text[charCount];
            if (char == "\n") {
                if (segmentLine) {
                    this.items.push(new Segment(str));
                    segmentLine = false;
                }
                else {
                    segmentLine = true;
                }
                str = "";
            }
            else {
                str += char;
            }
            charCount++;
        }
        playlist.segments = this.items;
        return playlist;
    }
}
export default class ManifestController {
    constructor() {
        this.url = "";
    }
    Update() {
        HttpClient.GetFileAsString(this.url, (x) => this.OnStringLoaded(x));
    }
    OnStringLoaded(manifestText) {
        if (manifestText !== "" && manifestText !== null) {
            let man = this.ParseManifest(manifestText);
            var handler = this.manifestLoaded;
            if (handler != undefined) {
                handler(man);
            }
        }
    }
    ParseManifest(manifestText) {
        let manifest = new Manifest();
        var charsTotal = manifestText.length;
        var charCount = 0;
        var levelCount = 0;
        var str = "";
        while (charCount != charsTotal) {
            var char = manifestText[charCount];
            if (char == "\n") {
                manifest.playlists[levelCount++] = str;
                str = "";
            }
            else {
                str += char;
            }
            charCount++;
        }
        return manifest;
    }
}
export class HLSPlayer {
    constructor(media, manifestUrl) {
        this.qualityOption = "720";
        this.segmentsUrl = "";
        this.media = media;
        this.manifestController = new ManifestController();
        this.manifestController.url = manifestUrl;
        this.manifestController.manifestLoaded = (e) => { this.OnManifestLoaded(e); };
        this.manifestController.Update();
        this.playlistController = new PlaylistController();
        this.playlistController.plalistLoaded = (e) => { this.OnPlaylistLoaded(e); };
        this.segmentController = new SegmentController();
    }
    OnPlaylistLoaded(playlist) {
        this.segmentController.ApplyPlaylist(playlist);
    }
    OnManifestLoaded(e) {
        this.playlistController.Update(e.playlists[0]);
    }
    Play() {
        var _a;
        var avaiableSegments = this.segmentController.segmentBank;
        debugger;
        if (avaiableSegments.length > 0) {
            for (var i = 0; i < 1; i++) {
                var url = (_a = avaiableSegments[i]) === null || _a === void 0 ? void 0 : _a.blobUrl;
                this.media.src = url;
            }
        }
        this.media.play();
    }
}
