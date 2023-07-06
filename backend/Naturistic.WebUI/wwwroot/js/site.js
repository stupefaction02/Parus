// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.

document.addEventListener('DOMContentLoaded', function () {
    var headerSignOutForm = document.getElementById("header-signout-form");

    var back_path = window.location.pathname;
    headerSignOutForm.value = back_path;
    //headerSignOutForm.setAttribute("name", "back_url");
    headerSignOutForm.setAttribute("href", back_path);

    console.log(headerSignOutForm);
    console.log(back_path);
}, false);