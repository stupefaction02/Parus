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

                show_hide_btn.setAttribute("src", location.origin +  + "/images/show_password.png");

                passwordInput.setAttribute("type", "password");
            }
            else {
                passwordShown = true;

                show_hide_btn.setAttribute("src", location.origin +  + "/images/hide_password.png");

                passwordInput.setAttribute("type", "text");
            }
        }

        show_hide_btn.setAttribute("src", location.origin +  + "/images/show_password.png");

        passwordInput.setAttribute("type", "password");

        show_hide_btn.onclick = function (e) {
            switchPasswordIcon();
        }

        close_popup_bn.onclick = function (e) {
            popup.style.display = "none";
        }

        sendbuton.onclick = function (e) {
            //debugger
            var username = usernameInput.value;
            var password = passwordInput.value;

            if (username !== null && username !== "" && password !== null && password !== "") {
                var url = "https://localhost:5001/api/account/login?username=" + username + "&password=" + password;

                $.ajax({
                    url: url,
                    method: 'post',
                    success: (e) => {
                        //debugger

                        if (e.success == "Y") {
                            document.cookie = "JWT=" + e.payload;

                            document.location.reload();
                        }
                        else {
                            login_error_label.style.display = "block";
                        }
                    },
                    error: (e) => { debugger
                        //login_error_label.style.display = "block";
                    }
                });
            }
        };

        this.inputs = inputs;
    }

    onclick(e) {

        
    }

    ShowPopup () {
        this.popup.style.display = "block";
    }

    HidePopup () {
        this.popup.style.display = "none";
    }

    sendPost (url, onsuccess) {
        
    }
}