"use strict";

class Network {
    // async
    static GetFileAsString(onsuccess) {
        const Http = new XMLHttpRequest();
        const url = 'https://jsonplaceholder.typicode.com/posts';
        Http.open("GET", url);
        Http.send();
        Http.onreadystatechange = () => {
            debugger;
        };
    }
}
class Manifest {
}
class ManifestController {
    constructor() {
        this.url = "";
    }
    UpdateManifest() {
        let manifestString;
        let callback = (type) => { debugger; };
        //Network.GetFileAsString(callback);
        Network.GetFileAsString((x) => this.OnStringLoaded(x));
    }
    OnStringLoaded(url) {
        debugger;
    }
}

export class HLSPlayer {
    constructor(media, manifestUrl) {
        this.media = media;
        this.manifestController = new ManifestController();
        this.manifestController.url = manifestUrl;
        this.manifestController.UpdateManifest();
    }
}
function create_HLSPlayer(media, manifestUrl) {
    return new HLSPlayer(media, manifestUrl);
}

