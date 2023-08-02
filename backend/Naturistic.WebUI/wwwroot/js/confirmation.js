/*import { sendGet } from "./network";*/
/*import { showPopup } from "./regform";*/

(function ($) {
    'use strict';

    var sendPost = function (url, onsuccess) {
        $.ajax({
            url: url,
            method: 'post',
            //dataType: 'html',
            //data: { text: 'Текст' },     
            success: onsuccess
        });
    }

    var request_verificaion_code = function (username) {
        var url = "https://localhost:5001/api/account/requestverificationcode?username=" + username;//
        console.log(url);
        sendPost(url, null);
    }

    var send_code = function (code, onsuccess) {
        //debugger
        var username = document.getElementById("nickname_input").value;
        var url = "https://localhost:5001/api/account/verifyaccount?code=" + code + "&username=" + username;

        console.log(url);

        //sendGet(url);

        $.ajax({
            url: url,
            method: 'post',
            //dataType: 'html',
            //data: { text: 'Текст' },     
            success: onsuccess
        });
    }

    var send_code_again_onclick = function (e) { //debugger
        var username = document.getElementById("nickname_input").value;
        request_verificaion_code(username);
    }

    var send_code_again = document.getElementById("send_code_again");

    send_code_again.onclick = send_code_again_onclick;

    var email;

    var inputs = [];

    var divNode = document.getElementById("verification_numbers");
    var inputNodes = divNode.getElementsByTagName('INPUT');
    for (var i = 0; i < inputNodes.length; ++i) {
        var inputNode = inputNodes[i];
        if (inputNode.type == 'text') {
            inputs.push(inputNode);

            //inputNode.addEventListener("input", updateValue);
            inputNode.oninput = updateValue;
        }
    }

    var send_code_handler = function (e) {
        //debugger

        var popup = document.getElementById("popup");
        popup.style.display = "none";
        console.log(window.location.protocol + "://" + window.location.host);
        window.location.href = window.location.protocol + "://" + window.location.host;
    }

    function updateValue(e) {
        //debugger

        var target = e.originalTarget;

        var allAreFilled = true;
        for (var i = 0; i < inputs.length; ++i) {
            var inputNode = inputs[i];
            /*console.log(inputNode.value);*/
            if (!Object.is(inputNode, target)) {
                if (inputNode.value == "") {
                    allAreFilled = false;
                }
            }
        }

        if (allAreFilled) {
            //debugger

            var code = "";
            for (var i = 0; i < inputs.length; ++i) {
                var inputNode = inputs[i];
                code += inputNode.value;    
            }

            console.log(code);

            send_code(code, send_code_handler);
        }
    }

    var reg_email_input = document.getElementById("reg_email_input");
    reg_email_input.oninput = reg_email_input_oninput;

    var reg_email_input_oninput = function (e) {
        email = e.originalTarget.value;

        console.log(email);
    }

})(jQuery);

