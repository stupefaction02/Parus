type StringResponse = (str: string) => void;
class Network {

    // async
    static GetFileAsString(onsuccess: StringResponse): void {
        const Http = new XMLHttpRequest();
        const url = 'https://jsonplaceholder.typicode.com/posts';
        Http.open("GET", url);
        Http.send();

        Http.onreadystatechange = () => {
            debugger
        };
    }

}

class Manifest { }

export default class ManifestController {

    public url: string = "";

    UpdateManifest(): void {
        let manifestString: string;

        let callback: StringResponse = (type: string) => { debugger };
        //Network.GetFileAsString(callback);

        Network.GetFileAsString((x) => this.OnStringLoaded(x));
    }

    private OnStringLoaded(url: string): void {
        debugger
    }

}

export class HLSPlayer {
    media: HTMLMediaElement;
    manifestController: ManifestController;
    constructor(media: HTMLMediaElement, manifestUrl: string) {
        this.media = media;

        this.manifestController = new ManifestController();

        this.manifestController.url = manifestUrl;

        this.manifestController.UpdateManifest();
    }
}

function create_HLSPlayer(media: HTMLMediaElement, manifestUrl: string) {
    return new HLSPlayer(media, manifestUrl);
}
