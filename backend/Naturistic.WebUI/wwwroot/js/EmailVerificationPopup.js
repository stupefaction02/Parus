export class VerificationPopup {
    constructor(popupId) {
        this.popup = document.getElementById(popupId);
        
        var send_code_again = document.getElementById("send_code_again");

        send_code_again.onclick = function (e) {
            this.send_code_again_onclick(e);
        };

        var inputs = [];

        var divNode = document.getElementById("verification_numbers");
        var inputNodes = divNode.getElementsByTagName('INPUT');
        for (var i = 0; i < inputNodes.length; ++i) {
            var inputNode = inputNodes[i];
            if (inputNode.type == 'text') {
                inputs.push(inputNode);

                inputNode.oninput = function (e) {
                    updateValue(e);
                };
            }
        }
    }

    SetEmail (email) {
        this.email = email;
    } 

    send_code_again_onclick (e) { 
        request_verificaion_code(this.email);
    }

    request_verificaion_code (email) {
        var url = "https://localhost:5001/api/account/createverificationcode?email=" + email;//
        console.log(url);
        sendPost(url, null);
    }

    updateValue(e) {
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

            send_code(code, function (e) { send_code_handler(e); });
        }
    }

    send_code (code, onsuccess) {
        var url = "https://localhost:5001/api/account/verifyaccount?code=" + code + "&email=" + this.email;

        this.sendPost(url, onsuccess);
    }

    send_code_handler(e) {


        this.popup.style.display = "none";
        console.log(window.location.protocol + "://" + window.location.host);
        window.location.href = window.location.protocol + "://" + window.location.host;
    }

    reg_email_input_oninput (e) {
        this.email = e.originalTarget.value;
    }

    ShowPopup() {
        popup.style.display = "block";
    }

    HidePopup() {
        popup.style.display = "none";
    }

    sendPost (url, onsuccess) {
        $.ajax({
            url: url,
            method: 'post',
            success: onsuccess
        });
    }
}