var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import { HttpClient } from "./HttpClient.js";
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
        const mms = (preferManagedMediaSource || !self.MediaSource) &&
            self.ManagedMediaSource;
        return (mms ||
            self.MediaSource ||
            self.WebKitMediaSource);
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
        this.blobUrl = "";
        this.filename = url;
    }
}
export class SegmentController {
    constructor() {
        //public egmentLoaded: SegmentLoadedCallback | undefined;
        this.segmentBank = [];
    }
    ApplyPlaylist(playlist) {
        var segments = playlist.segments;
        for (var i = 0; i < segments.length; i++) {
            var seg = segments[i];
            var segUrl = playlist.url + seg.filename;
            //HttpClient.GetBlob(segUrl, (x) => this.OnSegmentLoaded(seg, x));
            HttpClient.GetArrayBuffer(segUrl, (x) => this.OnSegmentLoaded(seg, x));
        }
    }
    OnSegmentLoaded1(segment, videoFileBlob) {
        segment.blobUrl = URL.createObjectURL(videoFileBlob);
        segment.blob = videoFileBlob;
        console.log("Segment loaded.");
        this.segmentBank.push(segment);
    }
    OnSegmentLoaded(segment, buf) {
        segment.buffer = buf;
        console.log("Segment loaded." + " buf:" + buf);
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
        this._objectUrl = "";
        this.media = media;
        this.manifestController = new ManifestController();
        this.manifestController.url = manifestUrl;
        this.manifestController.manifestLoaded = (e) => { this.OnManifestLoaded(e); };
        this.manifestController.Update();
        this.playlistController = new PlaylistController();
        this.playlistController.plalistLoaded = (e) => { this.OnPlaylistLoaded(e); };
        this.segmentController = new SegmentController();
        this.AttachMedia(media);
    }
    AttachMedia(media) {
        var self = this;
        var mediaSourceType = Helper.getMediaSource(true);
        var mediaSource = new MediaSource();
        this.mediaSource = mediaSource;
        //this.mediaSource.
        var avaibleSegments = this.segmentController.segmentBank;
        const objectUrl = (this._objectUrl = URL.createObjectURL(mediaSource));
        media.src = objectUrl;
        mediaSource.addEventListener("sourceopen", function () {
            // NOTE: Browsers are VERY picky about the codec being EXACTLY
            // right here. Make sure you know which codecs you're using!
            self.sourceBuffer = mediaSource.addSourceBuffer("video/mp4;codecs=avc1.64001f");
            //debugger
            self.sourceBuffer.mode = "segments";
            // If we requested any video data prior to setting up the SourceBuffer,
            // we want to make sure we only append one blob at a time
            self.sourceBuffer.addEventListener("updateend", () => self.appendToSourceBuffer());
            self.sourceBuffer.addEventListener("error", (a) => { debugger; });
        });
        //mediaSource.err
    }
    appendToSourceBuffer() {
        return __awaiter(this, void 0, void 0, function* () {
            if (this.mediaSource.readyState === "open" &&
                this.sourceBuffer &&
                this.sourceBuffer.updating === false) {
                if (this.segmentController.segmentBank.length > 0) {
                    var seg = this.segmentController.segmentBank.shift();
                    var buffer = seg.buffer;
                    this.sourceBuffer.appendBuffer(new Uint8Array(buffer));
                    console.log("blob: " + seg.blobUrl + " append to buffer.");
                }
            }
            // Limit the total buffer size to 20 minutes
            // This way we don't run out of RAM
            //if (
            //    this.media.buffered.length &&
            //    this.media.buffered.end(0) - this.media.buffered.start(0) > 1200
            //) {
            //    this.sourceBuffer.remove(0, this.media.buffered.end(0) - 1200)
            //}
        });
    }
    OnPlaylistLoaded(playlist) {
        this.segmentController.ApplyPlaylist(playlist);
    }
    OnManifestLoaded(e) {
        this.playlistController.Update(e.playlists[0]);
    }
    Play() {
        var avaiableSegments = this.segmentController.segmentBank;
        var self = this;
        //if (avaiableSegments.length > 0) {
        //    for (var i = 0; i < 1; i++) {
        //        var url = avaiableSegments[i]?.blobUrl;
        //        this.media.src = url;
        //    }
        //}
        self.appendToSourceBuffer();
        setInterval(function () {
            // NEW: Try to flush our queue of video data to the video element
            self.appendToSourceBuffer();
            var e = self.mediaSource;
            var e1 = self.media;
            //debugger
        }, 1000);
        console.log("play");
        //this.media.play();
    }
}
