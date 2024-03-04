import { CURRENT_API_PATH } from "../config.js";
import { ValidateEmail } from "../common.js";
import { TwoFAEmailVerificationPopup } from "./2FAEmailVerificationPopup.js";

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

        two_fa_enable_btn.disabled = false;

        var code_input = document.getElementById("code_input");
        var code_error = document.getElementById("code_error");

        var self = this;
        two_fa_enable_btn.onclick = function () {
            //debugger

            var code = code_input.value;

            var url = CURRENT_API_PATH + "/account/2FA/verify2FACode?code="
                + code + "&" + "customerKey=" + self.TwoFASecretKey;

            self.sendPost(url, function (e) {
                //debugger

                if (e.success == "Y") {
                    var done_phase = document.getElementById("done_phase");

                    self.phase3panel.style.setProperty("display", "none");

                    done_phase.style.setProperty("display", "block");

                    if (self.onsuccess !== undefined) {
                        self.onsuccess(two_fa_enable_btn);
                    }
                } else {
                    code_error.style.setProperty("display", "block");
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

    init_phase_2() {
        this.phase2panel = document.getElementById("enter_code_phase");

        this.phase2panel.style.setProperty("display", "block");

        var image = document.getElementById("qrcode");

        var this1 = this;
        this.phase2panel.onload = function () {
            this1.verificationCodePopup.UpdatePlaceholder();
        }

        this.verificationCodePopup = new TwoFAEmailVerificationPopup();

        this.verificationCodePopup.onsuccess = function (e) {
            this1.switchToPhase3();

            if (e.success == "Y") {
                image.setAttribute("src", e.qr_image);

                // browser stores it until page is reloaded
                // much security!
                this1.TwoFASecretKey = e.customer_key;            
            }
        };

        this.phase2_inited = true;
    }

    init_phase_1() {
        this.phase1panel = document.getElementById("enter_email_phase");

        var input = document.getElementById("ac_ep_email_input");
        this.ac_ep_continue_btn = document.getElementById("ac_ep_continue_btn");

        var this1 = this;
        this.ac_ep_continue_btn.onclick = function () {
            var url = CURRENT_API_PATH + "/account/2FA/request2FAVerificationEmailCode";

            this1.sendPost(url, function (e) {
                console.log(e.payload);
                this1.switchToPhase2();
            });
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

    sendPost(url, onsuccess) {
        $.ajax({
            url: url,
            method: 'post',
            success: onsuccess,
            xhrFields: {
                withCredentials: true
            },
            
        });
    }
}

export class TwoFAdisablePopup {
    constructor(popopElemId) {
        this.popup = document.getElementById(popopElemId);

        this.closeBtn = document.getElementById("disable_2tf_close_popup");

        var self = this;
        this.closeBtn.onclick = function () { self.Hide(); };

        this.InitPhase1();

        this.phase2_inited = false;
        this.phase3_inited = false;
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

        var self = this;

        input.oninput = function () {
            if (input.value.length == 6) {
                btn.disabled = false;
            } else {
                btn.disabled = true;
            }
        }

        btn.onclick = function () {
            var url = CURRENT_API_PATH + "/account/2FA/disable?code=" + input.value;

            self.sendPut(url, self.OnVerifyCodeSuccess);       
        }
    }

    OnVerifyCodeSuccess() {
        debugger
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

    sendPost(url, onsuccess) {
        $.ajax({
            url: url,
            method: 'post',
            success: onsuccess,
            xhrFields: { withCredentials: true }
        });
    }

    sendPut(url, onsuccess) {
        $.ajax({
            url: url,
            method: 'put',
            success: onsuccess,
            xhrFields: { withCredentials: true }
        });
    }
}