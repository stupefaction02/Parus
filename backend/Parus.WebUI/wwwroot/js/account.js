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

sendPost(url, onsuccess) {
    $.ajax({
        url: url,
        method: 'post',
        success: onsuccess
    });
}

sendGet(url, onsuccess) {
    $.ajax({
        url: url,
        method: 'get',
        success: onsuccess
    });
}

function InitSecurityOption() {
    var change_password_btn = document.getElementById("change_password_btn");

    var checkPassword = function (password) {
        var url = "https://localhost:5001/api/account/checkPassword?username=" + username + "&password=" + password;
        var ret = false;
        sendGet(url, function (e) {
            debugger

        });
    }

    change_password_btn.onclick = function () { debugger
        var change_password_popup = document.getElementById("change_password_popup");

        change_password_popup.style.display = "block";

        var old_password = document.getElementById("first_password");
        var new_password = document.getElementById("first_password");

        var rightPassword = checkPassword( old_password.value );
        if (rightPassword) {

        }
        else {

        }
    }
}