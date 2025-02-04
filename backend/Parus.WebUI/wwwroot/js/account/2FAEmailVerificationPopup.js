import { CURRENT_API_PATH } from "../config.js";
import { GetCookie } from "../common.js";
import { ApiPostRequest } from "../site.js";

export class TwoFAEmailVerificationPopup {
    constructor() { //debugger
        this.send_code_again_timer = document.getElementById("2fa_send_code_again_timer");

        var self = this;

        // TODO: do ifs
        var emailVerificated;

        var send_code_again = document.getElementById("2fa_send_code_again");

        this.send_code_again = send_code_again;

        send_code_again.onclick = function (e) {
            self.send_code_again_onclick(e);
        };

        var inputs = [];

        var divNode = document.getElementById("2fa_verification_numbers");
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

    UpdatePlaceholder() {
        var spanPlaceholder = document.querySelector("#2fa_verification_info .placeholder");

        spanPlaceholder.textContent = GetCookie("identity.username");
    }

    SetUsername (username) {
        this.username = username;
    } 

    send_code_again_onclick(e) { 
        debugger
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
            if (e.success == "Y") {
                //self.ShowPopup();
                debugger
                var spanPlaceholder = document.querySelector("#2fa_verification_info .placeholder");

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

            self.sendCode(code);
        }
    }

    sendCode(code) {
        var url = "/account/2FA/verify?code=" + code;

        var self = this;

        ApiPostRequest(url, {
            success: function (e) {
                // calling callback
                self.OnSuccessCallback(e);
            },
            status400: function (xhr) { debugger
                if (xhr.responseJSON.success = "false") {
                    var errorCode = xhr.responseJSON.errorCode;

                    // TODO: Pull MAIL_VERIF_WRONG_CODE from serverResponseCodes.js
                    if (errorCode === "MAIL_VERIF_WRONG_CODE") {
                        self.showError(errorCode);
                    }
                }
            }
        });
    }

    showError(errorCode) {
        var errorLabel = document.getElementById("2fa_verification_error");

        // TODO: Pull MAIL_VERIF_WRONG_CODE text from locale_ru.js
        errorLabel.textContent = "������������ ���";
        errorLabel.style.setProperty("display", "block");
    }

    send_code (code, onsuccess, onfail) {
        var url = CURRENT_API_PATH + "/account/2FA/verify?code=" + code;

        this.sendPost(url, onsuccess, onfail);
    }

    send_code_handler(e) {
        //this.popup.style.display = "none";
        //console.log(window.location.protocol + "://" + window.location.host);
        //window.location.href = window.location.protocol + "://" + window.location.host;

        this.onsuccess();
    }

    reg_email_input_oninput (e) {
        this.username = e.originalTarget.value;
    }

    RequestCode() {
        this.request_verificaion_code(this.username);
        
        this.canSend = false;
        
        this.send_code_again.classList.add("disabled_text");

        this.timeSeconds = 120;

        setInterval(() => this.updateTimer(this), 900);
    }

    sendPost(url, onsuccess, onfail) {
        $.ajax({
            url: url,
            method: 'post',
            success: onsuccess,
            error: onfail,
            xhrFields: { withCredentials: true }
        });
    }
}