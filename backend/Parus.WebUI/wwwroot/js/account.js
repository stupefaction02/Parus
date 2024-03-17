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

for (var i = 0; i < account_sidebar.children.length; i++) { 
    account_sidebar.children[i].onclick = onitemclick;
}

var currentOption = "security";
function switchOption(option) {

    if (currentOption == option) return;

    var selectedElem;
    switch (option) {
        case "other":
            selectedElem = document.getElementById("setting_content_other");
            break;

        case "security":
            selectedElem = document.getElementById("setting_content_security");
            InitSecurityOption();
            break;
    }
    
    currentOption = option;

    selectedElem.style.setProperty("display", "block");
    console.log(selectedElem.id, selectedElem.style.getPropertyValue("display"));

    for (var i = 0; i < setting_content.children.length; i++) {
        var child = setting_content.children[i];

        if (child !== selectedElem) {
            child.style.setProperty("display", "none");
            console.log(child.id, child.style.getPropertyValue("display"));
        }
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

var securitySettingsInited = false;
function InitSecurityOption() {

    if (securitySettingsInited) {
        return;
    }

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

    var twoFAbuttons = document.getElementById("2fa_buttons");

    var btn = twoFAbuttons.children[0];

    if (btn.id === "enable_2ft_btn") {
        InitEnable2FAButton(btn, "enable_2tf_popup");
    } else if (btn.id === "disable_2ft_btn") {
        InitDisable2FAButton(btn, "disbale_2tf_popup");
    }

    securitySettingsInited = true;
}

function InitDisable2FAButton(btn, id) {
    var popup = new TwoFAdisablePopup(id, function (btn) {
        InitEnable2FAButton(btn, "enable_2tf_popup");
    });

    btn.onclick = function () {
        popup.Show();
    }
}

function InitEnable2FAButton(btn, id) {
    var popup = new TwoTFpopup(id, function (btn) {
        InitDisable2FAButton(btn, "disable_2tf_popup");
    });

    btn.onclick = function () {
        popup.Show();
    }
}