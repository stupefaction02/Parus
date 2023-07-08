export function sendGet(url, onsuccess) {
    var httpRequest = false;

    if (window.XMLHttpRequest) { // Mozilla, Safari, ...
        httpRequest = new XMLHttpRequest();
        if (httpRequest.overrideMimeType) {
            httpRequest.overrideMimeType('text/xml');
            // ������� ���� �� ���� ������
        }
    } else if (window.ActiveXObject) { // IE
        try {
            httpRequest = new ActiveXObject("Msxml2.XMLHTTP");
        } catch (e) {
            try {
                httpRequest = new ActiveXObject("Microsoft.XMLHTTP");
            } catch (e) { }
        }
    }

    if (!httpRequest) {
        console.log('�� ����� :( ���������� ������� ��������� ������ XMLHTTP ');
        return false;
    }

    httpRequest.onreadystatechange = onsuccess;
    httpRequest.open('GET', url, true);
    httpRequest.send(null);
}

export function sendPost(url, onsuccess) {
    var httpRequest = false;

    if (window.XMLHttpRequest) { // Mozilla, Safari, ...
        httpRequest = new XMLHttpRequest();
        if (httpRequest.overrideMimeType) {
            httpRequest.overrideMimeType('text/xml');
            // ������� ���� �� ���� ������
        }
    } else if (window.ActiveXObject) { // IE
        try {
            httpRequest = new ActiveXObject("Msxml2.XMLHTTP");
        } catch (e) {
            try {
                httpRequest = new ActiveXObject("Microsoft.XMLHTTP");
            } catch (e) { }
        }
    }

    if (!httpRequest) {
        console.log('�� ����� :( ���������� ������� ��������� ������ XMLHTTP ');
        return false;
    }

    httpRequest.onreadystatechange = onsuccess;
    httpRequest.open('POST', url, true);
    httpRequest.send(null);
}
