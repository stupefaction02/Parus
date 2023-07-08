export function PopUpShow() {
    $("#popup1").show();
}

export function PopUpHide() {
    $("#popup1").hide();
}

(function ($) {
    'use strict';
    debugger
    PopUpHide();

    var regform_submit = document.getElementById("regform_submit"); 

    var regform_submit_onsubmit = function (e) {
        PopUpShow();
    }

    regform_submit.onclick = regform_submit_onsubmit;
    
})(jQuery);
