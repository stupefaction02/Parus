import { VerificationPopup } from "./EmailVerificationPopup.js";
import { DeleteCookie, GetCookie, IsStringEmpty } from "./common.js";
import { sendGetAjax } from "./network.js";
import { CURRENT_API_PATH, JWT_ACCESS_TOKEN_NAME } from "./config.js";
import { LoginPopup } from "./LoginPopup.js";

// Network
// todo: move it to network.js

export function AjaxPost(url, onsuccess, onerror) {
    console.log("debug: sending API request. Url: " + url);

    $.ajax({
        url: url,
        method: 'post',
        //error: error,
        //dataType: 'html',          
        //data: { text: 'Текст' },     
        success: onsuccess,

        error: function (jqXHR, textStatus, errorThrown) {
            var status = jqXHR.status;
            //debugger

            if (status == 500) {
                console.log("debug: " + CURRENT_API_PATH + " is down!");
                ShowErrorPopup(CURRENT_API_HOST + " is down! Error 500");
            } else if (status == 0) {
                console.log("debug: " + "CORS Error. Status Code: " + status);
                ShowErrorPopup("CORS Error. Status Code: " + status);
            } else if (status != 200) {
                // todo: proper debug log
                console.log("debug: " + " Error. Status code: " + status);
            }

            //onerror(jf);
        },
    });
}

document.addEventListener('DOMContentLoaded', function () {
    var header_user_settings = document.getElementById("header_user_settings");
    var confirm_account_link = document.getElementById("confirm_account_link");
    var header_signout_button = document.getElementById("header_signout_button");
    var header_login_btn = document.getElementById("header_login_btn");
    var header_register_btn = document.getElementById("header_register_btn");
    var header_search_icon = document.getElementById("header_search_icon");
    var header_search_input = document.getElementById("header_search_input");

    header_search_input.addEventListener("keydown", function (e) {
        if (e.key == "Enter") {
            var q = header_search_input.value;
            window.location.href = window.location.origin + "/search?q=" + q;
        }
    });

    header_search_input.addEventListener("focus", function (e) {
        document.getElementsByClassName("header_search_icon_highlight")[0].style.display = "block";
    });

    header_search_input.addEventListener("focusout", function (e) {
        document.getElementsByClassName("header_search_icon_highlight")[0].style.display = "none";
    });

    header_search_icon.addEventListener("mousedown", function () {
        var q = header_search_input.value;
        window.location.href = window.location.origin + "/search?q=" + q;
    });

    var header_user_settings_popup_is_open = false;
    if (header_user_settings !== null) {

        header_user_settings.onclick = function (e) {
            var popup = header_user_settings.getElementsByClassName("header_user_settings_popup")[0];
            
            if (popup !== null) {
                if (header_user_settings_popup_is_open) {
                    popup.style.display = "none";
                    header_user_settings_popup_is_open = false;
                }
                else {
                    popup.style.display = "block";
                    header_user_settings_popup_is_open = true;
                }
            }
        }
    }

    if (header_signout_button !== null) {
        header_signout_button.onclick = function (e) {  
            DeleteCookie("JWT");
            DeleteCookie("identity.username");
            DeleteCookie("username");
            
            console.log(document.cookie);
            window.location.reload();
        }
    }
   
    var popup = new VerificationPopup("site_popup");
    
    if (confirm_account_link !== null) {
        confirm_account_link.onclick = function (e) {
            
            var username = GetCookie("identity.username");
            if (username !== undefined && popup.username === undefined) {
                popup.SetUsername(username);
            }

            popup.RequestCode();

            popup.ShowPopup();

            document.body.onclick = function (e) {
                //debugger
                //popup.HidePopup();
            }
        }
    }

    var loginPopup = new LoginPopup("login_popup");

    if (header_login_btn != null) {
        header_login_btn.onclick = function () {
            loginPopup.ShowPopup();
        }
    }

    sideErrorPopup = document.getElementById("side_popup");
    sideErrorPopupText = document.querySelector(".side_popup .side_popup_content span");

    sideDebugPopup = document.getElementById("debug_popup");
    sideDebugPopupText = document.querySelector("#debug_popup .side_popup_content span");

    /*ShowDebugPopup("Alooo");*/

}, false);


var sideErrorPopup;
var sideErrorPopupText;

var sideDebugPopup;
var sideDebugPopupText;

var hidePopup = function (popup) {
    popup.style.display = "none";
}

// TODO: there is need to be a collections of popups, when a one popup is created by the template and disposed right after being shown
export function ShowErrorPopup(message, type) {
    //debugger
    //var leftTopPopup = document.getElementById("error_popup");

    if (sideErrorPopup !== null) {
        sideErrorPopup.style.display = "block";

        sideErrorPopupText.innerText = message;

        setTimeout(() => hidePopup(sideErrorPopup), 5000);
    }
}

export function ShowDebugPopup(message) {
    //debugger
    //var leftTopPopup = document.getElementById("error_popup");

    if (sideDebugPopup !== null) {
        sideDebugPopup.style.display = "block";

        sideDebugPopupText.innerText = message;

        setTimeout(() => hidePopup(sideDebugPopup), 1000000);
    }
}

// dummy raw implementation
export function ValidatePassword(password) { debugger
    var isValid = true;
    var errorMessages = [];

    if (password.length < 4) {
        errorMessages.push("Password must be more than 4 characters!");
        isValid = false;
    }

    return {
        isValid: isValid,
        errorMessages: errorMessages,
    }

}

export function ApiPostRequest(path, handlers) {
    //debugger
    var url = CURRENT_API_PATH + path;

    console.log("debug: sending API request. Url: " + url);

    var ajax = {
        url: url,
        method: 'post',
        success: (e, a, b) => {
            if (b.status >= 200 && b.status < 300) {
                if (e.success == "true") {
                    handlers.success(e, a, b);
                }
            }
        },
        error: (jqXHR, textStatus, errorThrown) => {
            if (jqXHR.status == 401) {
                handlers.status401(jqXHR, textStatus, errorThrown);
            } else if (jqXHR.status == 500 || jqXHR.status == 0) {
                handlers.status500(jqXHR, textStatus, errorThrown);
            }
        }
    };
    
    var jwt = GetCookie("JWT");
    if (!IsStringEmpty(jwt)) {
        ajax.headers = {
            "Authorization": "Bearer " + jwt
        };
    }

    $.ajax(ajax);
}


