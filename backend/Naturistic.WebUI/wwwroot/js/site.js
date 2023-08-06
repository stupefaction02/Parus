import { VerificationPopup } from "./EmailVerificationPopup.js";
import { DeleteCookie, GetCookie } from "./common.js";
import { sendGetAjax } from "./network.js";
import { CURRENT_API_PATH, JWT_ACCESS_TOKEN_NAME } from "./config.js";

document.addEventListener('DOMContentLoaded', function () {
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

    //sendGetAjax(CURRENT_API_PATH + "/test", function (e) { debugger }, {
    //    "Accept": "application/json",
    //    'Authorization': 'Bearer ' + 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYnJvYWRjYXN0ZXIyNSIsIm5iZiI6MTY5MDI3Mzc3NSwiZXhwIjoxNjkwNTMyOTc1LCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo1MDAwIiwiYXVkIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NTAwMiJ9.OELfoBcMINvF-qCgS9nst26jYJbU7aOdqsdRAdw6W2A'//sessionStorage.getItem(JWT_ACCESS_TOKEN_NAME)
    //});

}, false);