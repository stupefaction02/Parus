import { VerificationPopup } from "./EmailVerificationPopup.js";

document.addEventListener('DOMContentLoaded', function () {
    var headerSignOutForm = document.getElementById("header-signout-form");

    if (headerSignOutForm != null) {
        var back_path = window.location.pathname;
        headerSignOutForm.value = back_path;
        //headerSignOutForm.setAttribute("name", "back_url");
        headerSignOutForm.setAttribute("href", back_path);

        console.log(headerSignOutForm);
        console.log(back_path);
    }

    var confirm_account_link = document.getElementById("confirm_account_link");
    var popup = new VerificationPopup("site_popup");

    //debugger
    var email = document.cookie["email"];
    if (email !== undefined) {
        popup.SetEmail(email);
    }

    if (confirm_account_link !== null) {
        confirm_account_link.onclick = function (e) {
            debugger
            popup.ShowPopup();

            document.body.onclick = function (e) {
                debugger
                popup.HidePopup();
            }
        }
    }


}, false);