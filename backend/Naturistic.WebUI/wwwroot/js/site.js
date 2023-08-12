import { VerificationPopup } from "./EmailVerificationPopup.js";
import { DeleteCookie, GetCookie } from "./common.js";
import { sendGetAjax } from "./network.js";
import { CURRENT_API_PATH, JWT_ACCESS_TOKEN_NAME } from "./config.js";

// TODO
function setLocale() {
    document.cookie = "locale=ru";
}

document.addEventListener('DOMContentLoaded', function () {

    setLocale();


    var confirm_account_link = document.getElementById("confirm_account_link");
    var header_signout_button = document.getElementById("header_signout_button");

    if (header_signout_button !== null) {
        header_signout_button.onclick = function (e) {
            DeleteCookie("JWT");
            DeleteCookie("identity.username");
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

            document.body.onclick = function (e) {
                //debugger
                //popup.HidePopup();
            }
        }
    }

}, false);