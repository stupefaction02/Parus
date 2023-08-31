import { VerificationPopup } from "./EmailVerificationPopup.js";
import { DeleteCookie, GetCookie } from "./common.js";
import { sendGetAjax } from "./network.js";
import { CURRENT_API_PATH, JWT_ACCESS_TOKEN_NAME } from "./config.js";
import { LoginPopup } from "./LoginPopup.js";

document.addEventListener('DOMContentLoaded', function () {
    var confirm_account_link = document.getElementById("confirm_account_link");
    var header_signout_button = document.getElementById("header_signout_button");
    var header_login_btn = document.getElementById("header_login_btn");
    var header_register_btn = document.getElementById("header_register_btn");

    //header_login_btn.onmouseover = e => { debugger }

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

}, false);