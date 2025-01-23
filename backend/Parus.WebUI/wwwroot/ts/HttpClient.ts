export type BlobResponse = (blob: Blob) => void;
export type StringResponse = (str: string) => void;
export type ArrayBufferResponse = (ab: ArrayBuffer) => void;
export class HttpClient {

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

    static GetArrayBuffer(url: string, onsuccess: ArrayBufferResponse): void {
        const xhr = new XMLHttpRequest();
        const _url = url;
        xhr.open("GET", _url);

        xhr.responseType = "arraybuffer";
        xhr.onload = (e) => {
            onsuccess(xhr.response as ArrayBuffer);
        };

        xhr.send();
    }
}
