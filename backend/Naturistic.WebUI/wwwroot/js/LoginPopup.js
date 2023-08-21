export class LoginPopup {
    constructor(popupId) { //debugger
        this.popup = document.getElementById(popupId);

        var self = this;

        var inputs = [];
       
        var usernameInput = document.getElementById("login_usernameInput");
        var passwordInput = document.getElementById("password_Input");
        var sendbuton = document.getElementById("login_sendbutton");
        var login_error_label = document.getElementById("login_error_label");

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