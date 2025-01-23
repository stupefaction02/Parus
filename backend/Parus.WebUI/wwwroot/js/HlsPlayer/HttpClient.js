export class HttpClient {
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
    static GetArrayBuffer(url, onsuccess) {
        const xhr = new XMLHttpRequest();
        const _url = url;
        xhr.open("GET", _url);
        xhr.responseType = "arraybuffer";
        xhr.onload = (e) => {
            onsuccess(xhr.response);
        };
        xhr.send();
    }
}
