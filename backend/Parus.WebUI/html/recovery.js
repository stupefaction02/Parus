import { VerifyEmail } from "./common.js";

document.addEventListener('DOMContentLoaded', function () {
	var phase1 = document.getElementById("recovery_container_phase1");
    var phase2 = document.getElementById("recovery_container_phase2");
    var input = document.getElementById("recovery_container_input");
	
	function switchToPhase1 () {
		phase1.display = "block";
		phase2.display = "none";
	}
	
	function switchToPhase2 () {
		phase1.display = "none";
		phase2.display = "block";
	}
	
	input.oninput = function (e) {
		debugger
	}
}