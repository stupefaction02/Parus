console.log('its a chat')

var signInBtn = document.getElementsByClassName('signin-btn')[0]
var signUpBtn = document.getElementsByClassName('signup-btn')[0]

var closePopup = function(e, args) {
    //debugger
    $('.sign-up-in-popup').css('visibility', 'collapse');
    $('.sign-up-popup-content').css('visibility', 'collapse');
    $('.sign-in-popup-content').css('visibility', 'collapse');
    $('.sign-up-in-popup-bg').remove('click', closePopup);
}

var openSignUpPopup = function(e, args) {
    //debugger
    $('.sign-up-in-popup').css('visibility', 'visible');
    $('.sign-up-popup-content').css('visibility', 'visible');

    $('.sign-up-in-popup-bg').on('click', closePopup);
}

var openSignInPopup = function(e, args) {
    $('.sign-up-in-popup').css('visibility', 'visible');
    $('.sign-in-popup-content').css('visibility', 'visible');

    $('.sign-up-in-popup-bg').on('click', closePopup);
}

signInBtn.addEventListener('click', openSignInPopup)
signUpBtn.addEventListener('click', openSignUpPopup)

$(document).ready(function() { 
    var registerForm = $('#register-form');
    registerForm.on('submit', function(a, b) { 
        var data = registerForm.serializeArray();
        debugger
        $.ajax({
            type: 'POST',
            url: `https://127.0.0.1:5001/api/user/register?nickname=${data[0].value}&email=${data[1].value}&password=${data[2].value}&passwordRepeat=${data[3].value}`,
            data: data,
            dataType: 'json',
            success: function(a) {
              debugger
            }
          }).done(function() {
              debugger
          });
          
          a.preventDefault();
    }); 

    var loginForm = $('#login-form');
    loginForm.on('click', function(a, b) { 
        var data = loginForm.serializeArray();

        $.ajax({
            url: "https://127.0.0.1:5001/api/user/login",
            data: data
          }).done(function() {
            debugger
          }).fail(function() {
            debugger
          });
    });

    var printUsers = function () {
      $.ajax({
        headers:
        { 
          'Accept': '*/*', 
          'Accept-Encoding': 'gzip, deflate, br',
        },
        type: 'GET',
        url: 'https://localhost:5001/api/users',
        success: function (a, b) {
          debugger
        }
      }).done(function() {
        debugger
      }).fail(function(a, b) {
      });
    };

    printUsers();
});


