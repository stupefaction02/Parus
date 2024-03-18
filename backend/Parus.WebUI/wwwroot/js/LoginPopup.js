import { CURRENT_API_PATH, JWT_ACCESS_TOKEN_NAME } from "./config.js";

export class LoginPopup {
    constructor(popupId) { //debugger
        var popup = document.getElementById(popupId);

        this.popup = popup;

        var self = this;

        var inputs = [];
       
        var usernameInput = document.getElementById("login_usernameInput");
        var passwordInput = document.getElementById("password_Input");
        var sendbuton = document.getElementById("login_sendbutton");
        var login_error_label = document.getElementById("login_error_label");
        var close_popup_bn = document.getElementById("close_popup");
        var show_hide_btn = document.getElementById("login_show_hide_password");

        var passwordShown = false;

        var switchPasswordIcon = function () {
            if (passwordShown) {
                passwordShown = false;

                show_hide_btn.setAttribute("src", location.origin + "/images/show_password.png");

                passwordInput.setAttribute("type", "password");
            } else {
                passwordShown = true;

                show_hide_btn.setAttribute("src", location.origin + "/images/hide_password.png");

                passwordInput.setAttribute("type", "text");
            }
        }

        show_hide_btn.setAttribute("src", location.origin + "/images/show_password.png");

        passwordInput.setAttribute("type", "password");

        show_hide_btn.onclick = function (e) {
            switchPasswordIcon();
        }

        close_popup_bn.onclick = function (e) {
            popup.style.display = "none";
        }

        sendbuton.onclick = function (e) {
            self.HandleButtonClick(usernameInput.value, passwordInput.value);
        };

        this.inputs = inputs;
    }

    HandleButtonClick(username, password) {
        //debugger

        var self = this;

        if (username !== null && username !== "" && password !== null && password !== "") {
            var url = CURRENT_API_PATH + "/account/login?username=" + username + "&password=" + password;

            $.ajax({
                url: url,
                method: 'post',
                success: (e, a, b) => {
                    if (b.status == 200) {
                        if (e.success == "true") {
                            if (e.twoFactoryEnabled) {
                                self.ShowTwoFactoryForm();
                            } else {
                                document.cookie = "JWT=" + e.payload + "; path=/";
                                document.location.reload();
                            }
                        }
                    }
                },
                error: (e) => {
                    if (e.status == 401) {
                        var json = e.responseJSON;

                        if (json.errorCode == "LOGIN_WRONG_PSWD") {
                            login_error_label.style.display = "block";

                            var password_recovery_label =
                                document.getElementById("password_recovery_label");

                            password_recovery_label.style.display = "block";
                        }
                    }
                }
            });
        }
    }

    ShowTwoFactoryForm() {
        var loginForm = document.getElementById("login_body");
        var two_factory_body = document.getElementById("two_factory_body");

        loginForm.style.setProperty("display", "none");

        two_factory_body.style.setProperty("display", "block");

        this.InitTwoFactoryForm();
    }

    InitTwoFactoryForm() {

        var btn = document.getElementById("2fa_send_btn");
        var code_error = document.getElementById("code_error");
        var code_success = document.getElementById("code_success");

        btn.onclick = function () {
            var code = code_input.value;

            var url = CURRENT_API_PATH + "/account/2FA/verify2FACode?code="
                + code + "&" + "customerKey=" + self.TwoFASecretKey;

            var onfail = function (e) {
                if (e.status == 401) {
                    var json = e.responseJSON;

                    if (json.errorCode == "2FA_WRONG_QR_CODE") {
                        code_error.style.setProperty("display", "block");
                    }
                }
            }

            var onsuccess = function (e) {
                if (e.success == "Y") {

                    document.cookie = "JWT=" + e.payload + "; path=/";
                    document.location.reload();

                    code_success.style.setProperty("display", "block");
                }
            };

            self.sendPost(url, onsuccess, onfail);
        }
    }

    onclick(e) {

        
    }

    ShowPopup () {
        this.popup.style.display = "block";
    }

    HidePopup () {
        this.popup.style.display = "none";
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