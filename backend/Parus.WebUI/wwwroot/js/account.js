import { GetCookie } from "./common.js";
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

for (var i = 0; i < account_sidebar.children.length; i++) { debugger
    account_sidebar.children[i].onclick = onitemclick;
}

var currentOptionElem = new HTMLElement();
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
    currentOptionElem.style = "none";
    currentOptionElem = selectedElem;
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

    var checkPassword = function (password) {
        var url = "https://localhost:5001/api/account/checkPassword?password=" + password;
        var ret = false;
        sendGet(url, function (e) {
            debugger

        });
    }

    //change_password_btn.removeEventListener("click");
    change_password_btn.onclick = function () { 
        var change_password_popup = document.getElementById("change_password_popup");
        var change_password_close_popup = document.getElementById("change_password_close_popup");

        var change_password_close_popup_click = function () {
            change_password_popup.style.display = "block";
        };
        change_password_close_popup.removeEventListener("click", change_password_close_popup_click);
        change_password_close_popup.addEventListener("click", change_password_close_popup_click);

        change_password_popup.style.display = "block";

        var changePasswordButton = document.getElementById("change_pswd_button");

        var changePassword = function () {
            var old_password = document.getElementById("first_password");

            var rightPassword = checkPassword(old_password.value);
            if (rightPassword) {
                var new_password = document.getElementById("new_password");

                var url = "https://localhost:5001/api/account/updatePassword?password=" + password;
                sendPost(url, function () {
                    window.location.reload();
                });
            }
            else {
                var first_password_input_error = document.getElementById("first_password_input_error");
                first_password_input_error.style.display = "block";
            }
        }

        changePasswordButton.removeEventListener("click", changePassword);
        changePasswordButton.addEventListener("click", changePassword);
    }
}