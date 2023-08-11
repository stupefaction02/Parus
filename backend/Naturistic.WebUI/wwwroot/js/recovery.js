import { ValidateEmail } from "./common.js";

document.addEventListener('DOMContentLoaded', function () {
	var phase1 = document.getElementById("recovery_container_phase1");
	var phase2 = document.getElementById("recovery_container_phase2");
	var phase3 = document.getElementById("recovery_container_phase3");

	var input = document.getElementById("recovery_container_input");
	var recovery_continue_btn = document.getElementById("recovery_continue_btn");

	var username_input = document.getElementById("username_input");
	var send_ref_btn = document.getElementById("send_ref_btn");

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
	}

	switchToPhase1();

	input.oninput = function (e) {
		var text = e.target.value;
		
		if (text === "") {
			recovery_continue_btn.disabled = true;
		}

		if (ValidateEmail(e.target.value)) {
			recovery_continue_btn.disabled = false;
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

	}
});