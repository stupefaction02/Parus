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
            break;
    }
    //debugger
    currentOption = option;
    selectedElem.style = "display";
    currentOptionElem.style = "none";
    currentOptionElem = selectedElem;
}