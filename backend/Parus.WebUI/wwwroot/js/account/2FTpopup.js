import { CURRENT_API_PATH } from "../config.js";
import { ValidateEmail } from "../common.js";
import { TwoFAEmailVerificationPopup } from "./2FAEmailVerificationPopup.js";
import { ShowErrorPopup, ApiPostRequest, ApiPutRequest } from "../site.js"

export class TwoTFpopup {
    constructor(popopElemId, onsuccess) {
        this.popup = document.getElementById(popopElemId);

        this.closeBtn = document.getElementById("enable_2tf_close_popup");

        var self = this;
        this.closeBtn.onclick = function () { self.Hide(); };

        this.init_phase_1();

        this.phase2_inited = false;
        this.phase3_inited = false;

        this.onsuccess = onsuccess;
    }

    switchToPhase2() {
        this.phase1panel.style.setProperty("display", "none");
        if (this.phase3panel !== undefined) {
            this.phase3panel.style.setProperty("display", "none");
        }

        if (!this.phase2_inited) {
            this.init_phase_2();
        } else {
            this.phase2panel.style.setProperty("display", "block");
        }
    }

    switchToPhase3() {
        this.phase2panel.style.setProperty("display", "none");

        this.init_phase_3();
    }

    init_phase_3() {
        this.phase3panel = document.getElementById("qr_code_phase");

        this.phase3panel.style.setProperty("display", "block");

        var two_fa_enable_btn = document.getElementById("2fa_enable_btn");

        two_fa_enable_btn.disabled = true;

        var code_input = document.getElementById("code_input");

        var self = this;
        two_fa_enable_btn.onclick = function () {
            var code = code_input.value;

            var url = "/account/2FA/verify2FACode?code="
                + code + "&" + "customerKey=" + self.TwoFASecretKey;

            ApiPostRequest(url, {
                success: (e, a, b) => {
                    var done_phase = document.getElementById("done_phase");

                    self.phase3panel.style.setProperty("display", "none");

                    done_phase.style.setProperty("display", "block");

                    if (self.onsuccess !== undefined) {
                        self.onsuccess(two_fa_enable_btn);
                    }
                },
                status400: () => {
                    var code_error = document.getElementById("code_error");

                    code_error.style.setProperty("display", "block");       
                },
                status401: (jqXHR, a, b) => {
                    var json = jqXHR.responseJSON;

                    if (json.errorCode == "2FA.WrongCode") {
                        self.showWrongQrCodeError(json.errorCode);
                    }
                },
                status500: (e, a, b) => { //debugger
                    ShowErrorPopup("Server is down! Status Code 500");
                }
            });
        }

        code_input.oninput = function () {
            if (code_input.value.length == 6) {
                two_fa_enable_btn.disabled = false;
            } else {
                two_fa_enable_btn.disabled = true;
            }
        }

        console.log("qr code panel is shown.");
    }

    showWrongQrCodeError() {
        var code_error = document.getElementById("code_error");

        code_error.textContent = "Неправильный код";

        code_error.style.setProperty("display", "block");
    }

    init_phase_2() {
        this.phase2panel = document.getElementById("enter_code_phase");

        this.phase2panel.style.setProperty("display", "block");

        var image = document.getElementById("qrcode");

        var this1 = this;
        this.phase2panel.onload = function () {
            this1.verificationCodePopup.UpdatePlaceholder();
        }

        this.verificationCodePopup = new TwoFAEmailVerificationPopup();

        this.verificationCodePopup.OnSuccessCallback = function (e) {
            this1.switchToPhase3();

            image.setAttribute("src", e.qr_image);

            // browser stores it until page is reloaded
            // much security!
            this1.TwoFASecretKey = e.customer_key; 
        };

        this.phase2_inited = true;
    }

    init_phase_1() {
        this.phase1panel = document.getElementById("enter_email_phase");

        var input = document.getElementById("ac_ep_email_input");
        this.ac_ep_continue_btn = document.getElementById("ac_ep_continue_btn");

        ac_ep_continue_btn.disabled = true;

        var this1 = this;
        this.ac_ep_continue_btn.onclick = function () {
            //var url = CURRENT_API_PATH + "/account/2FA/request2FAVerificationEmailCode";
            var url = "/account/2FA/request2FAVerificationEmailCode";

            //this1.sendPost(url, function (e) {
            //    console.log(e.payload);
            //    this1.switchToPhase2();
            //});

            ApiPostRequest(url, { success: function () { this1.switchToPhase2(); } });
        }

        input.oninput = function (e) {
            var text = e.target.value;

            if (text === "") {
                this1.ac_ep_continue_btn.disabled = true;
            }

            if (ValidateEmail(text)) {
                this1.ac_ep_continue_btn.disabled = false;

                this.email = text;
            }
        }
    }

    Show() {
        this.popup.style.setProperty("display", "block");
    }

    Hide() {
        this.popup.style.setProperty("display", "none");
    }

    sendPost(url, onsuccess, onfail) {
        $.ajax({
            url: url,
            method: 'post',
            success: onsuccess,
            error: onfail,
            xhrFields: {
                withCredentials: true
            },
            
        });
    }
}

export class TwoFAdisablePopup {
    constructor(popopElemId, onsuccess) {
        this.popup = document.getElementById(popopElemId);

        this.closeBtn = document.getElementById("disable_2tf_close_popup");

        var self = this;
        this.closeBtn.onclick = function () { self.Hide(); };

        this.InitPhase1();

        this.phase2_inited = false;
        this.phase3_inited = false;

        this.onsuccess = onsuccess;
    }

    Show() {
        this.popup.style.setProperty("display", "block");
    }

    Hide() {
        this.popup.style.setProperty("display", "none");
    }

    InitPhase1() {
        this.phase1panel = document.getElementById("2fa_disable_phase1");

        var input = document.getElementById("2fa_disable_code_input");
        var btn = document.getElementById("2fa_disable_phase1_btn");

        btn.disabled = true;

        var self = this;

        input.oninput = function () {
            if (input.value.length == 6) {
                btn.disabled = false;
            } else {
                btn.disabled = true;
            }
        }

        btn.onclick = function () {
            var url = "/account/2FA/disable?code=" + input.value;

            self.sendPut(url, self.OnVerifyCodeSuccess, onfail);

            ApiPutRequest(url, {
                success: (e, a, b) => {
                    if (e.success) {
                        self.OnVerifyCodeSuccess
                    } else {
                        document.cookie = "JWT=" + e.payload + "; path=/";
                        document.location.reload();
                    }
                },
                status401: (jqXHR, a, b) => {
                    var json = e.responseJSON;

                    if (json.errorCode == "2FA.WrongCode") {
                        self.showWrongQrCodeError(json.errorCode);
                    }
                },
                status500: (e, a, b) => { //debugger
                    ShowErrorPopup("Server is down! Status Code 500");
                }
            });
        }
    }

    showWrongQrCodeError() {
        var code_error = document.getElementById("2fa_disable_code_error");

        code_error.textContent = "Неправильный код";

        code_error.style.setProperty("display", "block");
    }

    OnVerifyCodeSuccess() {
        var donePhase = document.getElementById("disable_2tf_done_phase");

        donePhase.style.setProperty("display", "block");

        if (self.onsuccess !== undefined) {
            self.onsuccess();
        }
    }

    SwitchToPhase1() {
        this.phase1panel.style.setProperty("display", "none");

        if (this.phase3panel !== undefined) {
            this.phase3panel.style.setProperty("display", "none");
        }

        if (!this.phase2_inited) {
            this.init_phase_2();
        } else {
            this.phase2panel.style.setProperty("display", "block");
        }
    }

    sendPut(url, onsuccess, onfail) {
        $.ajax({
            url: url,
            method: 'put',
            error: onfail,
            success: onsuccess,
            xhrFields: { withCredentials: true }
        });
    }
}