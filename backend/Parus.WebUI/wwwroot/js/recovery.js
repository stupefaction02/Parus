import { ValidateEmail, RedirectToIndex } from "./common.js";
import { CURRENT_API_PATH } from "./config.js";

document.addEventListener('DOMContentLoaded', function () {
	var phase1 = document.getElementById("recovery_container_phase1");
	var phase2 = document.getElementById("recovery_container_phase2");
	var phase3 = document.getElementById("recovery_container_phase3");

	var email_input = document.getElementById("recovery_container_input");
	var recovery_continue_btn = document.getElementById("recovery_continue_btn");

	var username_input = document.getElementById("username_input");
	var send_ref_btn = document.getElementById("send_ref_btn");

	var repeat_btn = document.getElementById("repeat_btn");
	var complete_btn = document.getElementById("complete_btn");

	var email;

	var sendPost = function (url, onsuccess) {
		$.ajax({
			url: url,
			method: 'post',
			//dataType: 'html',
			//data: { text: 'Текст' },     
			success: onsuccess
		});
	}

	function switchToPhase1() {
		phase1.style.display = "block";
		phase2.style.display = "none";
		phase3.style.display = "none";
	}

	function switchToPhase2() { 
		phase1.style.display = "none";
		phase2.style.display = "block";
		phase3.style.display = "none";
	}

	function switchToPhase3() {
		phase1.style.display = "none";
		phase2.style.display = "none";
		phase3.style.display = "block";

		var check_email_span = document.getElementById("check_email_span");
		
		check_email_span.textContent = email;
	}

	switchToPhase1();

	email_input.oninput = function (e) {
		var text = e.target.value;
		
		if (text === "") {
			recovery_continue_btn.disabled = true;
		}

		if (ValidateEmail(text)) {
			recovery_continue_btn.disabled = false;

			email = text;
		}
	}

	recovery_continue_btn.onclick = function (e) {
		switchToPhase2();
	}

	username_input.oninput = function (e) {
		if (e.target.value.length > 3) {
			send_ref_btn.disabled = false;
		}
		else {
			send_ref_btn.disabled = true;
		}
	}

	send_ref_btn.onclick = function (e) {
		
		var username = username_input.value;
		var email = email_input.value;
		var locale = "ru";

		var url = CURRENT_API_PATH + "/account/sendrecoveryemail?username=" + username + "&email=" + email + "&locale=" + locale;

		sendPost(url, function (e)
		{
			switchToPhase3();
		});
	}

	repeat_btn.onclick = function (e) {
		username_input.value = "";
		email_input.value = "";
		switchToPhase1();
	}

	complete_btn.onclick = function (e) {
		RedirectToIndex();
	}
});