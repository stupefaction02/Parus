/*import { sendPost } from "./network";*/

export function showPopup() {
    $("#popup1").show();
}

export function PopUpHide() {
    $("#popup1").hide();
}

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
    
    PopUpHide();

    var regform_submit = document.getElementById("regform_submit"); 

    var nickname_input_error = document.getElementById("nickname_input_error");
    var email_input_error = document.getElementById("email_input_error"); 

    var send_check_if_nickname_exists_handler = function (e) {
        //debugger

        if (e == "N") {
            nickname_input_error.style.display = "none";
        } else {
            nickname_input_error.style.display = "block";
        }
    }

    var send_check_if_nickname_exists = function (nickname) {
        //debugger
        var url = "https://localhost:5001/api/account/checkifnicknameexists?nickname=" + nickname;

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
            email_input_error.style.display = "none";
        } else {
            email_input_error.style.display = "block";
        }
    }

    var send_check_if_email_exists = function (email) {
        var url = "https://localhost:5001/api/account/checkifemailexists?email=" + email;
        //debugger
        sendGet(url, send_check_if_email_exists_handler);
    }

    var reg_email_input_oninput = function (e) { //debugger
        send_check_if_email_exists(e.originalTarget.value);
    }

    var email_input = document.getElementById("reg_email_input");
    email_input.oninput = reg_email_input_oninput;

    send_check_if_email_exists(email_input.value);

    var regform_submit_onsubmit = function (e) {
        //debugger
        var onsuccess = function (e) {
            //debugger

            showPopup();
        }

        var firstname = document.getElementById("firstname").value;
        var lastname = document.getElementById("lastname").value;
        var nickname = document.getElementById("nickname_input").value;
        var email = document.getElementById("reg_email_input").value;
        var password = document.getElementById("password_input").value;
        var url = "https://localhost:5001/api/account/register?firstname=" + firstname + "&lastname=" + lastname + "&nickname=" + nickname + "&email=" + email + "&password=" + password;
        console.log(url);
        sendPost(url, onsuccess);
    }

    regform_submit.onclick = regform_submit_onsubmit;

    var sendPost1 = function (url, onsuccess) {
        var httpRequest = false;
        debugger
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
