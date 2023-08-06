/*import { sendPost } from "./network";*/

import { CURRENT_API_PATH, JWT_ACCESS_TOKEN_NAME } from "./config.js";

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

async function sleep(msec) {
    return new Promise(resolve => setTimeout(resolve, msec));
}

(function ($) {
    'use strict';
    
    //hidePopup();

    var nicknameFormHasError;
    var emailFormHasError;

    var regform_submit = document.getElementById("regform_submit"); 

    var nickname_input_error = document.getElementById("nickname_input_error");
    var email_input_error = document.getElementById("email_input_error"); 

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

    var popup = document.getElementById("popup");

    var showPopup = function () {
        popup.style.display = "block";
    }

    var hidePopup = function () {
        popup.style.display = "none";
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
        var nickname = document.getElementById("nickname_input").value;
        var email = document.getElementById("reg_email_input").value;
        var password = document.getElementById("password_input").value;
        //debugger
        var genders = document.querySelectorAll('input[type=radio]:checked');

        var onsuccess = function (e) {
            reg_loading_gif.style.display = "none";

            //requestJwtToken(nickname);

            if (e.success == "Y") {
                debugger
                document.cookie = "JWT=" + e.payload.access_token + "; path=/";

                showPopup();
                request_verificaion_code(nickname, function (e) {
                    //debugger
                    //showPopup();
                });
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

        var url = CURRENT_API_PATH + "/account/register?firstname=" + firstname + "&lastname=" + lastname + "&username=" + nickname + "&email=" + email + "&password=" + password + "&gender=" + gender;
        console.log(url);
        sendPost(url, onsuccess);
    }

    regform_submit.onclick = regform_submit_onsubmit;

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
