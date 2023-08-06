export class VerificationPopup {
    constructor(popupId) { //debugger
        this.popup = document.getElementById(popupId);
        
        var send_code_again = document.getElementById("send_code_again");

        var self = this;

        send_code_again.onclick = function (e) {
            self.send_code_again_onclick(e);
        };

        var inputs = [];

        var divNode = document.getElementById("verification_numbers");
        var inputNodes = divNode.getElementsByTagName('INPUT');
        for (var i = 0; i < inputNodes.length; ++i) {
            var inputNode = inputNodes[i];
            if (inputNode.type == 'text') {
                inputs.push(inputNode);

                inputNode.oninput = function (e) {
                    self.updateValue(e);
                };
            }
        }

        this.inputs = inputs;
    }

    SetUsername (username) {
        this.username = username;
    } 

    send_code_again_onclick (e) { 
        this.request_verificaion_code(this.username);
    }

    request_verificaion_code (username) { 
        var url = "https://localhost:5001/api/account/requestverificationcode?username=" + username;
        console.log(url);

        var self = this;

        this.sendPost(url, function (e) {
            if (e.success == "Y") {
                self.ShowPopup();
            }
        });
    }

    updateValue(e) {
        var target = e.originalTarget;

        var allAreFilled = true;
        for (var i = 0; i < this.inputs.length; ++i) {
            var inputNode = this.inputs[i];
            /*console.log(inputNode.value);*/
            if (!Object.is(inputNode, target)) {
                if (inputNode.value == "") {
                    allAreFilled = false;
                }
            }
        }

        var self = this;

        if (allAreFilled) {
            //debugger

            var code = "";
            for (var i = 0; i < this.inputs.length; ++i) {
                var inputNode = this.inputs[i];
                code += inputNode.value;
            }

            self.send_code(code, function (e) { self.send_code_handler(e); });
        }
    }

    send_code (code, onsuccess) {
        var url = "https://localhost:5001/api/account/verifyaccount?code=" + code + "&username=" + this.username;

        this.sendPost(url, onsuccess);
    }

    send_code_handler(e) {
        this.popup.style.display = "none";
        console.log(window.location.protocol + "://" + window.location.host);
        window.location.href = window.location.protocol + "://" + window.location.host;
    }

    reg_email_input_oninput (e) {
        this.username = e.originalTarget.value;
    }

    RequestCode() {
        this.request_verificaion_code(this.username);
    }

    ShowPopup() {
        this.popup.style.display = "block";
    }

    HidePopup() {
        this.popup.style.display = "none";
    }

    sendPost (url, onsuccess) {
        $.ajax({
            url: url,
            method: 'post',
            success: onsuccess
        });
    }
}