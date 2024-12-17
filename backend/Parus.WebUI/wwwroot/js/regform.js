/*import { sendPost } from "./network";*/

import { GetCookie, IsStringEmpty } from "./common.js";
import { CURRENT_API_PATH, JWT_ACCESS_TOKEN_NAME } from "./config.js";
import { VerificationPopup } from "./EmailVerificationPopup.js";
import { ShowPopupError } from "./site.js";


function sendPost(url, onsuccess, onerror) {
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
        },
    });
}

function sendGet(url, onsuccess, error) {
    console.log("debug: sending API request. Url: " + url);

    $.ajax({
        url: url,
        method: 'get',
        error: error,
        //dataType: 'html',          
        //data: { text: 'Текст' },     
        success: onsuccess
    });
}

(function ($) {
    'use strict';
  
    var authCookie = GetCookie("JWT");
    var authenticated = authCookie !== undefined;
    if (authenticated) { return; }

    var nicknameFormHasError;
    var emailFormHasError;

    var regform_submit = document.getElementById("regform_submit"); 

    //var nickname_input_error = document.getElementById("nickname_input_error");
    //var email_input_error = document.getElementById("email_input_error"); 

    var header_register_btn = document.getElementById("header_register_btn"); 
    var close_popup_bn = document.getElementById("registration_close_popup");

    var firstNameInput = document.getElementById("firstname");
    var firstNameInputError = document.getElementById("firstname_input_error");

    var lastnameInput = document.getElementById("lastname");
    var lastnameInputError = document.getElementById("lastname_input_error");

    var usernameInput = document.getElementById("nickname_input");
    var nickname_input_error = document.getElementById("nickname_input_error");

    var emailInput = document.getElementById("reg_email_input");
    var email_input_error = document.getElementById("email_input_error");

    var passwordInput = document.getElementById("password_input");
    var passwordInputError = document.getElementById("password_input_error");

    // pull from localization file
    var nickname_input_error_text = "Username is already taken";
    var email_input_error_text = "Email is already taken";
    var notEmptyText = "Fill it up!";

    function show_nickname_error(message) {
        nickname_input_error.style.display = "block";
        nickname_input_error.innerText = message;
    }

    function hide_username_error() { nickname_input_error.style.display = "none"; }
    function hide_email_error() { email_input_error.style.display = "none"; }

    function show_email_error(message) {
        email_input_error.style.display = "block";
        email_input_error.innerText = message;
    }

    // nickname = username -_-
    var send_check_if_nickname_exists = function (nickname) {
        //debugger
        var url = CURRENT_API_PATH + "/account/isusernametaken?username=" + nickname;

        var send_check_if_nickname_exists_handler = function (response, nickname) {
            //debugger

            // TODO: proper error handling
            if (response.taken == "false") {
                nicknameFormHasError = false;
                hide_username_error();
            } else {
                nicknameFormHasError = true;
                console.log("debug: nickname " + " " + nickname + " is already taken!");
                show_nickname_error(nickname_input_error_text);
            }
        }

        sendGet(url,
            (e) => send_check_if_nickname_exists_handler(e, nickname),
            () => {
                //ShowPopupError("Server Error 500!");
            }
        );
    }

    usernameInput.oninput = function (e) {
        var input = e.originalTarget.value;

        if (!IsStringEmpty(input)) {
            send_check_if_nickname_exists(input);
        }
    };

    if (!IsStringEmpty(usernameInput.value)) {
        send_check_if_nickname_exists(usernameInput.value);
    }

    var send_check_if_email_exists = function (email) {
        var url = CURRENT_API_PATH + "/account/isemailtaken?email=" + email;

        var send_check_if_email_exists_handler = function (response, email) { //debugger
            if (response.taken == "false") {
                emailFormHasError = false;
                hide_email_error();
            } else {
                emailFormHasError = true;
                show_email_error(email_input_error_text);
                console.log("debug: email " + " " + email + " is already taken!");
            }
        }

        sendGet(url,
            (e) => send_check_if_email_exists_handler(e, email),
            () => {
                //ShowPopupError("Server Error 500!");
            });
    }

    emailInput.oninput = function (e) { 
        var input = e.originalTarget.value;

        if (!IsStringEmpty(input)) {
            send_check_if_email_exists(input);
        }
    };

    if (!IsStringEmpty(emailInput.value)) {
        send_check_if_email_exists(emailInput.value);
    }



    var popup = document.getElementById("registration_popup");

    var showPopup = function () {
        popup.style.display = "block";
    }

    var hidePopup = function () {
        popup.style.display = "none";
    }

    close_popup_bn.onclick = function (e) {
        popup.style.display = "none";
    }

    if (header_register_btn !== null) {
        header_register_btn.onclick = function (e) {
            showPopup();
        }
    }

    var regform_submit_onsubmit = function (e) {
        /*debugger*/

        if (nicknameFormHasError || emailFormHasError) {
            return;
        }

        reg_loading_gif.style.display = "display";


        var firstname = firstNameInput.value;
        var lastname = lastnameInput.value;
        var username = usernameInput.value;
        var email = emailInput.value;
        var password = passwordInput.value;
       // debugger
        var genders = document.querySelectorAll('input[type=radio]:checked');

        var validateSuccess = true;

        // TODO: Change it to a loop

        if (IsStringEmpty(firstname)) {
            firstNameInputError.innerText = notEmptyText;
            firstNameInputError.style.display = "block";
            validateSuccess = false;
        }

        if (IsStringEmpty(lastname)) {
            lastnameInputError.innerText = notEmptyText;
            lastnameInputError.style.display = "block";
            validateSuccess = false;
        }

        if (IsStringEmpty(username)) {
            nickname_input_error.innerText = notEmptyText;
            nickname_input_error.style.display = "block";
            validateSuccess = false;
        }

        if (IsStringEmpty(email)) {
            email_input_error.innerText = notEmptyText;
            email_input_error.style.display = "block";
            validateSuccess = false;
        }

        if (IsStringEmpty(password)) {
            passwordInputError.innerText = notEmptyText;
            passwordInputError.style.display = "block";
            validateSuccess = false;
        }

        if (!validateSuccess) {
            setTimeout(cleanInputErrors, 2000);
            return;
        } 

        var onsuccess = function (e) {
            reg_loading_gif.style.display = "none";

            //requestJwtToken(nickname);
            //debugger
            if (e.success.toString() == "true") {

                hidePopup();
                //debugger
                SetCookie("JWT", e.access_token.jwt, e.access_token.expires);
                SetCookie("refreshToken", e.refresh_token.token, e.refresh_token.expires);

                var popup = new VerificationPopup("site_popup");
               
                if (username !== undefined && popup.username === undefined) {
                    popup.SetUsername(username);
                }

                popup.RequestCode();
            }
            else {
                // TODO: Display error
            }
        }

        var gender = 3;
        if (genders.length > 0) {
            gender = genders[0].value;
        }

        var url = CURRENT_API_PATH + "/account/register?firstname=" + firstname + "&lastname=" + lastname + "&username=" + username + "&email=" + email + "&password=" + password + "&gender=" + gender;

        sendPost(url, onsuccess, () => ShowPopupError("Server Error 500!"));
    }

    function cleanInputErrors() {
        firstNameInputError.style.display = "none";
        lastnameInputError.style.display = "none";
        nickname_input_error.style.display = "none";
        email_input_error.style.display = "none";
        passwordInputError.style.display = "none";
    }

    function SetCookie(cookie, cookieValue, expireUnix) {
        let date = new Date(Date.now() + expireUnix);
        var exp = date.toUTCString();
        document.cookie = cookie + "=" + cookieValue + "; path=/; expires=" + exp + " ;";
    }

    regform_submit.onclick = regform_submit_onsubmit;
    
    var show_hide_btn = document.getElementById("registration_show_hide_password");

    var passwordShown = false;

    var passwordInput = document.getElementById("password_input");

    var switchPasswordIcon = function () {
        if (passwordShown) {
            passwordShown = false;

            show_hide_btn.setAttribute("src", location.origin + "/images/show_password.png");

            passwordInput.setAttribute("type", "password");
        }
        else {
            passwordShown = true;

            show_hide_btn.setAttribute("src", location.origin + "/images/hide_password.png");

            passwordInput.setAttribute("type", "text");
        }
    }

    show_hide_btn.setAttribute("src", location.origin + "/images/show_password.png");

    passwordInput.setAttribute("type", "password");

    show_hide_btn.onclick = function (e) {
        switchPasswordIcon();
    }
})(jQuery);
