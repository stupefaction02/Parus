var weak_password_error = document.getElementById("weak_password_error");
var nomatch_password_error = document.getElementById("nomatch_password_error");
var new_password_input = document.getElementById("new_password_input");
var new_password_input_repeat = document.getElementById("new_password_input_repeat");
var compete_btn = document.getElementById("compete_btn");

var canSend = false;

var np_show_hide_password = document.getElementById("np_show_hide_password");
var npr_show_hide_password = document.getElementById("npr_show_hide_password");

var passwordShown = false;

var switchPasswordIcon = function (input, icon) {
    if (passwordShown) {
        passwordShown = false;

		icon.setAttribute("src", location.origin + "/images/show_password.png");

		input.setAttribute("type", "password");
    }
    else {
        passwordShown = true;

		icon.setAttribute("src", location.origin + "/images/hide_password.png");

		input.setAttribute("type", "text");
    }
}

np_show_hide_password.setAttribute("src", location.origin + "/images/show_password.png");
npr_show_hide_password.setAttribute("src", location.origin + "/images/show_password.png");

var UpdateLabels = function (inputLength) {
	if (inputLength == 0) {
		canSend = false;
		weak_password_error.style.visibility = "hidden";
		return;
	}

	if (inputLength > 6) {
		weak_password_error.style.visibility = "hidden";
	}
	else {
		weak_password_error.style.visibility = "visible";
	}
}

UpdateLabels(new_password_input.value.length);

np_show_hide_password.onclick = function (e) {
	switchPasswordIcon(new_password_input, np_show_hide_password);
}

npr_show_hide_password.onclick = function (e) {
	switchPasswordIcon(new_password_input_repeat, npr_show_hide_password);
}

new_password_input.oninput = function (e) {
	UpdateLabels(new_password_input.value.length);
}

new_password_input_repeat.oninput = function (e) {
	var newpassword = new_password_input.value;
	var newpasswordrepeat = new_password_input_repeat.value;

	if (newpassword != newpasswordrepeat) {
		nomatch_password_error.style.visibility = "visible";
	}
	else {
		nomatch_password_error.style.visibility = "hidden";
	}
}

compete_btn.onclick = function (e) {

	if (new_password_input.length == 0) {
		weak_password_error.style.visibility = "visible";

		return;
	}

	if (new_password_input.value == new_password_input_repeat.value) {
		var url = location.origin + "/account/editpassword&newpassword=" + new_password_input.value;
		
		window.location.href = url;

		return;
	}

	nomatch_password_error.style.visibility = "visible";
}




