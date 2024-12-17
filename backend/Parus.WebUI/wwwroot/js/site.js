import { VerificationPopup } from "./EmailVerificationPopup.js";
import { DeleteCookie, GetCookie } from "./common.js";
import { sendGetAjax } from "./network.js";
import { CURRENT_API_PATH, JWT_ACCESS_TOKEN_NAME } from "./config.js";
import { LoginPopup } from "./LoginPopup.js";

// Network
// todo: move it to network.js

export  function AjaxPost(url, onsuccess, onerror) {
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
                ShowPopupError(CURRENT_API_HOST + " is down! Error 500");
            } else if (status == 0) {
                console.log("debug: " + "CORS Error. Status Code: " + status);
                ShowPopupError("CORS Error. Status Code: " + status);
            } else if (status != 200) {
                // todo: proper debug log
                console.log("debug: " + " Error. Status code: " + status);
            }

            //onerror(jf);
        },
    });
}

var leftTopPopup;
var leftTopPopupText;

var hidePopupError = function () {
    leftTopPopup.style.display = "none";
}

export function ShowPopupError(message) {
    //debugger
    //var leftTopPopup = document.getElementById("error_popup");

    if (leftTopPopup !== null) {
        leftTopPopup.style.display = "block";

        leftTopPopupText.innerText = message;

        setTimeout(hidePopupError, 2000);
    }
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

    leftTopPopup = document.getElementById("error_popup");
    leftTopPopupText = document.querySelector(".error_tl_popup .error_tl_popup_content span");

}, false);
