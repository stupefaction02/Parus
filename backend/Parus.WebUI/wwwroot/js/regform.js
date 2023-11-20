/*import { sendPost } from "./network";*/

import { GetCookie } from "./common.js";
import { CURRENT_API_PATH, JWT_ACCESS_TOKEN_NAME } from "./config.js";
import { VerificationPopup } from "./EmailVerificationPopup.js";
function sendPost (url, onsuccess) {
    $.ajax({
        url: url,
        method: 'post',
        //dataType: 'html',          
        //data: { text: 'Текст' },     
        success: onsuccess
    });
}

function sendGet (url, onsuccess) {
    $.ajax({
        url: url,
        method: 'get',
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

    var nickname_input_error = document.getElementById("nickname_input_error");
    var email_input_error = document.getElementById("email_input_error"); 

    var header_register_btn = document.getElementById("header_register_btn"); 
    var close_popup_bn = document.getElementById("registration_close_popup");


    var send_check_if_nickname_exists_handler = function (e) {
        //debugger

        if (e == "N") {
            nicknameFormHasError = false;
            nickname_input_error.style.display = "none";
        } else {
            nicknameFormHasError = true;
            nickname_input_error.style.display = "block";
        }
    }

    var send_check_if_nickname_exists = function (nickname) {
        //debugger
        var url = CURRENT_API_PATH + "/account/checkifnicknameexists?nickname=" + nickname;

        sendGet(url, send_check_if_nickname_exists_handler);
    }

    var nickname_input_oninput = function (e) {
        //debugger
        send_check_if_nickname_exists(e.originalTarget.value);
    }

    var nickname_input = document.getElementById("nickname_input");
    nickname_input.oninput = nickname_input_oninput;

    send_check_if_nickname_exists(nickname_input.value);


    var send_check_if_email_exists_handler = function (e) { //debugger
        if (e == "N") {
            emailFormHasError = false;
            email_input_error.style.display = "none";
        } else {
            emailFormHasError = true;
            email_input_error.style.display = "block";
        }
    }

    var send_check_if_email_exists = function (email) {
        var url = CURRENT_API_PATH + "/account/checkifemailexists?email=" + email;
        //debugger
        sendGet(url, send_check_if_email_exists_handler);
    }

    var reg_email_input_oninput = function (e) { //debugger
        send_check_if_email_exists(e.originalTarget.value);
    }

    var email_input = document.getElementById("reg_email_input");
    email_input.oninput = reg_email_input_oninput;

    send_check_if_email_exists(email_input.value);

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

    header_register_btn.onclick = function (e) {
        showPopup();
    }

    var request_verificaion_code = function (username, onsuccess) {
        var url = CURRENT_API_PATH + "/account/requestverificationcode?username=" + username;//
        /*console.log(url);*/
        sendPost(url, onsuccess);
    }

    var requestJwtToken = function (username) {
        var url = CURRENT_API_PATH + "/account/jwt/login?username=" + username;

        var onsuccess = function (data) { 
            sessionStorage.setItem(JWT_ACCESS_TOKEN_NAME, data.access_token);
            document.cookie = "JWT=" + data.access_token + "; path=/";
            console.log(sessionStorage.getItem(JWT_ACCESS_TOKEN_NAME));
        }

        sendPost(url, onsuccess);
    }

    

    var regform_submit_onsubmit = function (e) {
        /*debugger*/

        if (nicknameFormHasError || emailFormHasError) {
            return;
        }

        reg_loading_gif.style.display = "display";


        var firstname = document.getElementById("firstname").value;
        var lastname = document.getElementById("lastname").value;
        var username = document.getElementById("nickname_input").value;
        var email = document.getElementById("reg_email_input").value;
        var password = document.getElementById("password_input").value;
        //debugger
        var genders = document.querySelectorAll('input[type=radio]:checked');

        var onsuccess = function (e) {
            reg_loading_gif.style.display = "none";

            //requestJwtToken(nickname);
            debugger
            if (e.success.toString() == "true") {

                hidePopup();

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

        // for genders enum see documentation 
        var gender = 3;
        if (genders.length > 0) {
            gender = genders[0].value;
        }

        var url = CURRENT_API_PATH + "/account/register?firstname=" + firstname + "&lastname=" + lastname + "&username=" + username + "&email=" + email + "&password=" + password + "&gender=" + gender;
        console.log(url);
        sendPost(url, onsuccess);
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

    var sendPost1 = function (url, onsuccess) {
        var httpRequest = false;
        //debugger
        if (window.XMLHttpRequest) { // Mozilla, Safari, ...
            httpRequest = new XMLHttpRequest();
            if (httpRequest.overrideMimeType) {
                httpRequest.overrideMimeType('text/xml');
                // Читайте ниже об этой строке
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
            console.log('Не вышло :( Невозможно создать экземпляр класса XMLHTTP ');
            return false;
        }

        httpRequest.onreadystatechange = onsuccess;
        httpRequest.open('POST', url, true);
        httpRequest.send(null);
    }

})(jQuery);
