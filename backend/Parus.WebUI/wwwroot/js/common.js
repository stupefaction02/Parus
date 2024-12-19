export function GetCookie(name) {
    let matches = document.cookie.match(new RegExp(
        "(?:^|; )" + name.replace(/([\.$?*|{}\(\)\[\]\\\/\+^])/g, '\\$1') + "=([^;]*)"
    ));
    return matches ? decodeURIComponent(matches[1]) : undefined;
}
function setCookie(key, value, expireDays, expireHours, expireMinutes, expireSeconds) {
    var expireDate = new Date();
    if (expireDays) {
        expireDate.setDate(expireDate.getDate() + expireDays);
    }
    if (expireHours) {
        expireDate.setHours(expireDate.getHours() + expireHours);
    }
    if (expireMinutes) {
        expireDate.setMinutes(expireDate.getMinutes() + expireMinutes);
    }
    if (expireSeconds) {
        expireDate.setSeconds(expireDate.getSeconds() + expireSeconds);
    }
    document.cookie = key + "=" + escape(value) +
        ";domain=" + window.location.hostname +
        ";path=/" +
        ";expires=" + expireDate.toUTCString();
}

export function IsStringEmpty(str) {
    return (!str || str.length === 0);
}

export function DeleteCookie1(name) {
    setCookie(name, "", null, null, null, 1);
}

const emailValidationPattern = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|.(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
export function ValidateEmail(email) {
    var matches = String(email).toLowerCase().match(emailValidationPattern);
    return {
        isValid: matches !== null
    };
}

export function RedirectToIndex() {
    document.location.href = "/";
}

export function DeleteCookie(name) {
    document.cookie = name + "=" + ";path=/;max-age=-1";
    console.log(name + "=" + ";path=/;expires = Thu, 01 Jan 1970 00: 00: 00 GMT");
}

export function DeleteCookie2(name, path) {
    document.cookie = name + "=" + " ; path=" + path + "; expires = Thu, 01 Jan 1970 00: 00: 00 GMT";
    console.log(name + "=" + " ; path=" + path + "; expires = Thu, 01 Jan 1970 00: 00: 00 GMT");
}