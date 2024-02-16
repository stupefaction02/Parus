import { ValidateEmail } from "../common.js";
import { TwoFAEmailVerificationPopup } from "./2FAEmailVerificationPopup.js";

export class TwoTFpopup {
    constructor(popopElemId) {
        this.popup = document.getElementById(popopElemId);

        this.closeBtn = document.getElementById("enable_2tf_close_popup");

        this.closeBtn.onclick = function () {
            Hide();
        }

        this.init_phase_1();

        this.phase2_inited = false;
        this.phase3_inited = false;
    }

    switchToPhase2() {
        if (!this.phase2_inited) {
            this.init_phase_2();
        }
    }

    init_phase_2() {
        this.verificationCodePopup = new TwoFAEmailVerificationPopup();


    }

    init_phase_1() {
        var input = document.getElementById("ac_ep_email_input");
        this.ac_ep_continue_btn = document.getElementById("ac_ep_continue_btn");

        var this1 = this;
        this.ac_ep_continue_btn.onclick = function () {
            var url = "https://localhost:5001/api/account/2FA/request2FAVerificationEmailCode";

            this1.sendPost(url, function () {
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

                email = text;
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