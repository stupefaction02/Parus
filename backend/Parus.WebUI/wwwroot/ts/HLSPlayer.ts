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

type BlobResponse = (blob: Blob) => void;
type StringResponse = (str: string) => void;
class HttpClient {

    // async
    static GetFileAsString(url: string, onsuccess: StringResponse): void {
        const xhr = new XMLHttpRequest();
        const _url = url;
        xhr.open("GET", _url);
        xhr.send();

        xhr.onreadystatechange = (e) => {
            if (xhr.readyState === XMLHttpRequest.DONE) {
                const status = xhr.status;
                if (status === 0 || (status >= 200 && status < 400)) {
                    onsuccess(xhr.responseText);
                } else {
                    //err = "error!";
                }
            }
        };
    }

    static GetBlob(url: string, onsuccess: BlobResponse): void {
        const xhr = new XMLHttpRequest();
        const _url = url;
        xhr.open("GET", _url);

        xhr.responseType = "blob";
        xhr.onload = (e) => {
            onsuccess(xhr.response as Blob);
        };

        xhr.send();
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

    public blob: Nullable<Blob> = null;

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

            HttpClient.GetBlob(segUrl, (x) => this.OnSegmentLoaded(seg, x));
        }
    }

    OnSegmentLoaded(segment: Segment, videoFileBlob: Blob): void { 
        segment.blobUrl = URL.createObjectURL(videoFileBlob);

        this.segmentBank.push(segment);
    }

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

    AttachMedia(media: HTMLMediaElement) {
        debugger
        
        var mediaSourceType = Helper.getMediaSource(true);

        var ms = new mediaSourceType();

        const objectUrl = (this._objectUrl = self.URL.createObjectURL(ms));

        media.src = objectUrl;
    }

    OnPlaylistLoaded(playlist: Playlist) {
        this.segmentController.ApplyPlaylist(playlist);
    }

    OnManifestLoaded(e: Manifest) {
        this.playlistController.Update(e.playlists[0]);
    }

    Play(): void {
        var avaiableSegments = this.segmentController.segmentBank;
        



        //if (avaiableSegments.length > 0) {
        //    for (var i = 0; i < 1; i++) {
        //        var url = avaiableSegments[i]?.blobUrl;
        //        this.media.src = url;
        //    }
        //}

        //this.media.play();
    }
}
