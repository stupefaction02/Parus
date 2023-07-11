/*import { sendGet } from "./network";*/
import { showPopup } from "./regform";

(function ($) {
    'use strict';
    /*==================================================================*/

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

    function updateValue(e) {
        //debugger

        var allAreFilled = true;
        for (var i = 0; i < inputs.length; ++i) {
            var inputNode = inputs[i];
            /*console.log(inputNode.value);*/
            if (!Object.is(inputNode, e.originalTarget)) {
                if (inputNode.value == "") {
                    allAreFilled = false;
                }
            }
        }

        if (allAreFilled) {
            //debugger

            var number = 0;
            for (var i = 0; i < inputs.length; ++i) {
                var inputNode = inputs[i];
                number += inputNode.value;    
            }

            console.log(number);

            var email = "";
            var url = "https://localhost:5001/api/account/confirmdigits?number=" + number + "&email=" + email;

            console.log(url);

            //sendGet(url);

            $.ajax({
                url: url,
                method: 'post',
                dataType: 'html',
                //data: { text: 'Текст' },     
                success: onsuccess
            });
        }
    }

    var reg_email_input = document.getElementById("reg_email_input");
    reg_email_input.oninput = reg_email_input_oninput;

    var reg_email_input_oninput = function (e) {
        email = e.originalTarget.value;

        console.log(email);
    }

})(jQuery);

