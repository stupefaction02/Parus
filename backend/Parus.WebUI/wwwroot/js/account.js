import { GetCookie } from "./common.js";
import { TwoTFpopup, TwoFAdisablePopup } from "./account/2FTpopup.js";
import { CURRENT_API_PATH, JWT_ACCESS_TOKEN_NAME } from "./config.js";

var account_sidebar = document.getElementById("account_sidebar");
var setting_content = document.getElementById("setting_content");

var currentOptionElem;
var onitemclick = function (e) {
    var option = e.currentTarget.getAttribute("data");
    //debugger

    switchOption(option);
}

switchOption("security");

for (var i = 0; i < account_sidebar.children.length; i++) { //debugger
    account_sidebar.children[i].onclick = onitemclick;
}

var currentOptionElem = undefined;
var currentOption = "security";
function switchOption(option) {

    if (currentOption == option) return;

    var selectedElem;
    switch (option) {
        case "other":
            selectedElem = document.getElementById("setting_content_security");
            break;

        case "security":
            selectedElem = document.getElementById("setting_content_security");
            InitSecurityOption();
            break;
    }
    //debugger
    currentOption = option;
    selectedElem.style = "display";

    if (currentOptionElem !== undefined) {
        currentOptionElem.style = "none";
        currentOptionElem = selectedElem;
    }
}

function sendPost(url, onsuccess) {
    $.ajax({
        url: url,
        method: 'post',
        success: onsuccess,
        xhrFields: {
            withCredentials: true
        }
    });
};

function sendGet(url, onsuccess) {
    $.ajax({
        url: url,
        method: 'get',
        success: onsuccess,
        headers: {
            "Authorization": "Bearer " + GetCookie("JWT"),
        },
    });
};

function InitSecurityOption() {
    var change_password_btn = document.getElementById("change_password_btn");

    var checkPassword = function (password, success) {
        var url = CURRENT_API_PATH + "/account/checkPassword?password=" + password;
        var ret = false;
        sendGet(url, success);
    }

    //change_password_btn.removeEventListener("click");
    change_password_btn.onclick = function () { 
        var change_password_popup = document.getElementById("change_password_popup");
        var change_password_close_popup = document.getElementById("change_password_close_popup");

        var change_password_close_popup_click = function () {
            change_password_popup.style.display = "none";
        };
        change_password_close_popup.removeEventListener("click", change_password_close_popup_click);
        change_password_close_popup.addEventListener("click", change_password_close_popup_click);

        change_password_popup.style.display = "block";

        var changePasswordButton = document.getElementById("change_pswd_button");

        var changePassword = function () {
            var old_password = document.getElementById("first_password");
            var new_password = document.getElementById("new_password");
            var password = new_password.value;

            if (password !== "" && password !== null) {
                checkPassword(old_password.value, function (e) {

                    if (e.message == "Valid") {

                        if (old_password.textContent === password) {
                            var new_password_input_error = document.getElementById("new_password_input_error");
                            new_password_input_error.style.display = "block";
                            new_password_input_error.textContent = "Новый пароль не должен совпадать с предыдущим";
                        }

                        var url = CURRENT_API_PATH + "/account/editPassword?newPassword=" + password;
                        sendPost(url, function (e) {
                            let date = new Date(Date.now() + (60 * 60 * 1));
                            var exp = date.toUTCString();
                            document.cookie = "password_changed=1;expires=" + exp + ";path=/account;";
                            window.location.reload();
                        });
                    } else {
                        var first_password_input_error = document.getElementById("first_password_input_error");
                        first_password_input_error.style.display = "block";
                        first_password_input_error.textContent = "Пароли не совпадают";
                    }
                });
            } else {
                var new_password_input_error = document.getElementById("new_password_input_error");
                new_password_input_error.style.display = "block";
                new_password_input_error.textContent = "Введите новый пароль";
            }
        }

        changePasswordButton.removeEventListener("click", changePassword);
        changePasswordButton.addEventListener("click", changePassword);
    }

    var enable_2ft_btn = document.getElementById("enable_2ft_btn");
    //debugger
    var enable_2tf_popup_created = false;
    var popup;

    if (enable_2ft_btn != null) {
        enable_2ft_btn.onclick = function () {
            if (!enable_2tf_popup_created) { //debugger
                popup = new TwoTFpopup("enable_2tf_popup", function (btn) {
                    debugger
                });

                enable_2tf_popup_created = true;
            }

            popup.Show();
        }
    } else {
        disable_2ft_btn.onclick = function () {
            var d_popup = new TwoFAdisablePopup("disbale_2tf_popup");

            d_popup.Show();
        }
    }
}