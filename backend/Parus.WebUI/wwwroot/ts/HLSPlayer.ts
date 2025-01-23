import { HttpClient } from "./HttpClient.js";

class Helper {
    static TrimEndUntilChar_nochecks(str: string, stopChar: string): string {
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

    static getMediaSource(
        preferManagedMediaSource = true,
    ): typeof MediaSource {
        
        const mms =
            (preferManagedMediaSource || !self.MediaSource) &&
            ((self as any).ManagedMediaSource as undefined | typeof MediaSource);
        return (
            mms ||
            self.MediaSource ||
            ((self as any).WebKitMediaSource as typeof MediaSource)
        );
    }
}

class Manifest
{
    // 180
    // 360
    // 720
    public playlists: Array<string> = ["", "", ""];
}

class Playlist {
    public segments: Array<Segment> = [];
    public url: string = "";
}

type PlaylistLoadedCallback = (manifest: Playlist) => void;
type Nullable<T> = T | null;

class Segment {
    public filename: string = "";

    public blob: Blob;
    public buffer: ArrayBuffer;

    public blobUrl: string = "";

    constructor(url: string) {
        this.filename = url;
    }
}

type SegmentLoadedCallback = (manifest: Segment) => void;
export class SegmentController {

    //public egmentLoaded: SegmentLoadedCallback | undefined;
    public segmentBank: Array<Segment> = [];

    ApplyPlaylist(playlist: Playlist): void {
        var segments = playlist.segments;
        for (var i = 0; i < segments.length; i++) {
            var seg = segments[i];
            var segUrl = playlist.url + seg.filename;

            //HttpClient.GetBlob(segUrl, (x) => this.OnSegmentLoaded(seg, x));
            HttpClient.GetArrayBuffer(segUrl, (x) => this.OnSegmentLoaded(seg, x));
        }
    }

    OnSegmentLoaded1(segment: Segment, videoFileBlob: Blob): void { 
        segment.blobUrl = URL.createObjectURL(videoFileBlob);
        segment.blob = videoFileBlob;
        console.log("Segment loaded.");
        this.segmentBank.push(segment);
    }

    OnSegmentLoaded(segment: Segment, buf: ArrayBuffer): void {
        segment.buffer = buf;
        console.log("Segment loaded." + " buf:" + buf);
        this.segmentBank.push(segment);


    }
}

export class PlaylistController {

    public plalistLoaded: PlaylistLoadedCallback | undefined;

    Update(playlistUrl: string): void {
        HttpClient.GetFileAsString(playlistUrl, (x) => this.OnStringLoaded(x, playlistUrl));
    }

    private OnStringLoaded(playlistText: string, playlistUrl: string): void {
        if (playlistText !== "" && playlistText !== null) {

            // clear
            this.items = [];

            let pl: Playlist = this.ParsePlaylist(playlistText);

            pl.url = Helper.TrimEndUntilChar_nochecks(playlistUrl, "/");

            var handler = this.plalistLoaded;
            if (handler != undefined) {
                handler(pl);
            }
        }
    }

    private items: Array<Segment> = [];

    private ParsePlaylist(text: string): Playlist {
        let playlist: Playlist = new Playlist();
        var charsTotal = text.length;
        var charCount = 0;
        var segmentLine = false;

        var str: string = "";
        while (charCount != charsTotal) {
            var char: string = text[charCount];

            if (char == "\n") {
                if (segmentLine) {

                    this.items.push(new Segment(str));

                    segmentLine = false;
                } else {
                    segmentLine = true;
                }

                str = "";
            } else {
                str += char;
            }

            charCount++;
        }

        playlist.segments = this.items;

        return playlist;
    }
}

type ManifestLoadedCallback = (manifest: Manifest) => void;

export default class ManifestController {

    public manifestLoaded: ManifestLoadedCallback | undefined;

    public url: string = "";

    Update(): void {
        HttpClient.GetFileAsString(this.url, (x) => this.OnStringLoaded(x));
    }

    private OnStringLoaded(manifestText: string): void {
        if (manifestText !== "" && manifestText !== null) {
            let man: Manifest = this.ParseManifest(manifestText);
            
            var handler = this.manifestLoaded;
            if (handler != undefined) {
                handler(man);
            }
        }
    }

    private ParseManifest(manifestText: string): Manifest {
        let manifest: Manifest = new Manifest();
        var charsTotal = manifestText.length;
        var charCount = 0;
        var levelCount = 0;
        
        var str: string = "";
        while (charCount != charsTotal) {
            var char: string = manifestText[charCount];

            if (char == "\n") {
                manifest.playlists[levelCount++] = str;
                str = "";
            } else {
                str += char;
            }

            charCount++;
        }

        return manifest;
    }
}

export class HLSPlayer {
    public qualityOption: string = "720";

    public segmentsUrl: string = "";

    media: HTMLMediaElement;
    manifestController: ManifestController;
    playlistController: PlaylistController;
    segmentController: SegmentController;
    private _objectUrl: string = "";

    constructor(media: HTMLMediaElement, manifestUrl: string) {
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

    private sourceBuffer: SourceBuffer;
    private mediaSource: MediaSource;

    AttachMedia(media: HTMLMediaElement) {

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
            self.sourceBuffer.addEventListener("updateend",
                () => self.appendToSourceBuffer());

            self.sourceBuffer.addEventListener("error", (a) => { debugger });
        });

        //mediaSource.err
    }

    async appendToSourceBuffer() {
        if (
            this.mediaSource.readyState === "open" &&
            this.sourceBuffer &&
            this.sourceBuffer.updating === false
        ) {
            if (this.segmentController.segmentBank.length > 0) {
                var seg = this.segmentController.segmentBank.shift();
                var buffer = seg.buffer;
                this.sourceBuffer.appendBuffer( new Uint8Array(buffer) );
                
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
    }

    OnPlaylistLoaded(playlist: Playlist) {
        this.segmentController.ApplyPlaylist(playlist);

        
    }

    OnManifestLoaded(e: Manifest) {
        this.playlistController.Update(e.playlists[0]);
    }

    Play(): void {
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
