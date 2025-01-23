var showBroadcastsBtn = document.getElementById("show_broadcasts_btn");
var showCategoriessBtn = document.getElementById("show_categories_btn");
var showUsersBtn = document.getElementById("show_users_btn");



// TODO: security issue, fix it!!!!!
var params = new URLSearchParams(document.location.search);
var q = params.get("q");
var s = params.get("sector");

if (q !== null && q !== "") {
    var si = document.getElementsByClassName("header_search_input")[0];

    if (si !== null) {
        si.value = q;
    }
}

if (s === null) {
    var f = function (sector) {
        document.location.href = document.location.pathname + "?q=" + q + "&sector=" + sector;
    }

    if (showBroadcastsBtn != null) {
        showBroadcastsBtn.onclick = function () { f("broadcasts"); };
    }

    if (showCategoriessBtn != null) {
        showCategoriessBtn.onclick = function () { f("categories"); };
    }

    if (showUsersBtn != null) {
        showUsersBtn.onclick = function () { f("users"); };
    }
}