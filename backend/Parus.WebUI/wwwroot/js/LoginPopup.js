import { IsStringEmpty } from "./common.js";
import { CURRENT_API_PATH, JWT_ACCESS_TOKEN_NAME } from "./config.js";
import { ShowErrorPopup, ApiPostRequest } from "./site.js"

export class LoginPopup {
    constructor(popupId) { //debugger
        var popup = document.getElementById(popupId);

        this.popup = popup;

        var self = this;

        var inputs = [];

        this.nickname_input_empty_error_text = "Fill it up!";
        this.password_input_empty_error_text = "Fill it up!";
        
        this.nickname_input_error = document.getElementById("username_error");
        this.password_input_error = document.getElementById("password_error");

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

    cleanInputErrors() {
        this.nickname_input_error.style.display = "none";
        this.password_input_error.style.display = "none";
    }

    showUsernameError(text) {
        this.nickname_input_error.innerText = text;
        this.nickname_input_error.style.display = "block";
    }

    HandleButtonClick(username, password) {
        //debugger

        var anyErrors = false;

        if (IsStringEmpty(username)) {
            this.showUsernameError(this.nickname_input_empty_error_text);
            anyErrors = true;
        }

        if (IsStringEmpty(password)) {
            this.password_input_error.innerText = this.password_input_empty_error_text;
            this.password_input_error.style.display = "block";
            anyErrors = true;
        }

        var self = this;

        if (anyErrors) {
            setTimeout(() => self.cleanInputErrors(), 4000);
        } else {
            var url = "/account/login?username=" + username + "&password=" + password;

            ApiPostRequest(url, {
                success: (e, a, b) => {
                    if (e.twoFactoryEnabled) {
                        self.ShowTwoFactoryForm();

                        this.InitTwoFactoryForm(e, username);
                    } else {
                        document.cookie = "JWT=" + e.payload + "; path=/";
                        document.location.reload();
                    }
                },
                status401: (jqXHR, a, b) => {
                    //debugger
                    var errorCode = jqXHR.responseJSON.errorCode;

                    if (errorCode == "Login.WrongPassword") {
                        login_error_label.style.display = "block";

                        var password_recovery_label =
                            document.getElementById("password_recovery_label");

                        password_recovery_label.style.display = "block";
                    } else if (errorCode == "JWT_TOKEN_EXPIRED") {
                        //var url = 
                    }
                },
                status500: (e, a, b) => { //debugger
                    ShowErrorPopup("Server is down! Status Code 500");
                }
            });
        }
    }

    

    ShowTwoFactoryForm() {
        var loginForm = document.getElementById("login_body");
        var two_factory_body = document.getElementById("two_factory_body");

        loginForm.style.setProperty("display", "none");

        two_factory_body.style.setProperty("display", "block");
    }

    InitTwoFactoryForm(response, username) {
        var self = this;

        var btn = document.getElementById("2fa_send_btn");
        var code_error = document.getElementById("code_error");
        var code_success = document.getElementById("code_success");

        btn.onclick = function () {
            var code = code_input.value;

            var url = CURRENT_API_PATH + "/account/2FA/verify2FACode/login?code="
                + code + "&" + "customerKey=" + response.twoFactoryCustomKey + "&username=" + username;
            console.log(url);
            var onfail = function (e) {
                if (e.status == 401) {
                    var json = e.responseJSON;

                    if (json.errorCode == "2FA_WRONG_QR_CODE") {
                        code_error.style.setProperty("display", "block");
                    }
                }
            }

            var onsuccess = function (e) {
                if (e.success == "true") {
                    document.cookie = "JWT=" + e.payload + "; path=/";

                    code_success.style.setProperty("display", "block");

                    document.location.reload();
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