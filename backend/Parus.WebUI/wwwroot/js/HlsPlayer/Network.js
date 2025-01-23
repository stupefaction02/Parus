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
