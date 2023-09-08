import sendPost from "../network.js";

var play_button = document.getElementById("play_button");
var plays = true;


function switch_play() {
    if (plays) {
        plays = false;

        pause();
    } else {
        plays = true;

        play();
    }
}

function play() {

}

function pause() {

}

play_button.onclick = function () {
    switch_play();
}