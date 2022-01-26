console.log('its a chat')

var signInBtn = document.getElementsByClassName('signin-btn')[0]
var signUpBtn = document.getElementsByClassName('signup-btn')[0]

var signinupPopup = document.getElementsByClassName('sign-up-in-popup')[0]

var signinupPopupBg = document.getElementsByClassName('sign-up-in-popup-bg')[0]

var registerBtn = document.getElementsByClassName('register-btn')[0]

var closePopup = function(e, args) {
    signinupPopup.setAttribute('style', 'display: none');
    signinupPopupBg.removeEventListener('click', closePopup)
}

var openPopup = function(e, args) {
    signinupPopup.removeAttribute('style');
    signinupPopupBg.addEventListener('click', closePopup)
}

closePopup();

signInBtn.addEventListener('click', openPopup)
signUpBtn.addEventListener('click', openPopup)

$(document).ready(function() { 
    var registerForm = $('#register-form');
    registerForm.on('click', function(a, b) { 
        debugger
        
        var data = registerForm.serializeArray();

        $.ajax({
            url: registerApiUrl,
            context: data
          }).done(function() {
              
          });
    }); 
});


