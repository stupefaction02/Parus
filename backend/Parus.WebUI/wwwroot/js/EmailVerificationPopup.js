import { GetCookie } from "./common.js";
import { CURRENT_API_PATH } from "./config.js";

export class VerificationPopup {
    constructor(popupId) { //debugger
        this.popup = document.getElementById(popupId);

        var close_popup_bn = document.getElementById("verification_close_popup");
        this.send_code_again_timer = document.getElementById("send_code_again_timer");

        var self = this;

        close_popup_bn.onclick = function (e) {
            self.popup.style.display = "none";
        }

        // TODO: do ifs
        var emailVerificated;

        var send_code_again = document.getElementById("send_code_again");

        this.send_code_again = send_code_again;

        var spanPlaceholder = document.querySelector("#verification_info .placeholder");
        
        spanPlaceholder.textContent = GetCookie("identity.username");

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

    send_code_again_onclick(e) { 
        //debugger
        if (this.canSend) {
            this.request_verificaion_code(this.username, true);
        }
    }

    updateTimer(self) {
        //debugger

        if (self.timeSeconds == 0) {
            self.canSend = true;

            this.send_code_again.classList.remove("disabled_text");

            // two minutes
            self.timeSeconds = 120;
        }

        self.timeSeconds--;

        self.send_code_again_timer.innerText = self.timeSeconds;
    }

    request_verificaion_code (username, forceCreate) { 
        var url = CURRENT_API_PATH + "/account/requestverificationcode?username=" + username + "&forceCreate=" + forceCreate;
        console.log(url);

        var self = this;
        debugger
        this.sendPost(url, function (e) {
            if (e.success == "true") {
                self.ShowPopup();
                //debugger
                var spanPlaceholder = document.querySelector("#verification_info .placeholder");

                spanPlaceholder.textContent = self.username;
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
        var url = CURRENT_API_PATH + "/account/verifyaccount?code=" + code + "&username=" + this.username;

        this.sendPost(url, onsuccess);
    }

    send_code_handler(e) {
        //this.popup.style.display = "none";
        //console.log(window.location.protocol + "://" + window.location.host);
        //window.location.href = window.location.protocol + "://" + window.location.host;

        window.location.reload();
    }

    reg_email_input_oninput (e) {
        this.username = e.originalTarget.value;
    }

    RequestCode() {
        this.request_verificaion_code(this.username, false);
        
        this.canSend = false;
        
        this.send_code_again.classList.add("disabled_text");

        this.timeSeconds = 120;

        setInterval(() => this.updateTimer(this), 900);
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
            success: onsuccess, 
            headers: {
                "Content-Type": "application/json",
                "Accept": "application/json"
            }
        });
    }
}