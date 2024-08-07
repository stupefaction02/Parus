import { GetCookie } from "./common.js";
import { URL } from "./config.js";
import { HLSPlayer } from "./HlsPlayer/HLSPlayer.js";

var play_button = document.getElementById("play_button");
var plays = true;

var hlsServiceUrl = "https://localhost:2020";

var video = document.getElementById('video');

var manifestUrl = `${hlsServiceUrl}/live/123456/master_playlist.m3u8`;

var url = "https://localhost:2020/live/desh02/1_dash.mpd";
var player = dashjs.MediaPlayer().create();

//player.updateSettings({
//    'debug': {
//        'logLevel': dashjs.Debug.LOG_LEVEL_INFO
//    }
//});

player.initialize(video, url, true);

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

var maxMessages = 50;
var messageCount = 0;
var isScrolling = false;

var hubUrl = URL + '/chat';

if (signalR !== null) {

    const hubConnection = new signalR.HubConnectionBuilder()
        .withUrl(URL + '/chat')
        .build();

    var sendBtn = document.getElementById("sendBtn");

    var messages = document.getElementById("messages");
    var chat_input = document.getElementById("chat_input");

    var sys_con_txt = document.getElementById("sys_con_txt").innerText;
    var sys_discon_txt = document.getElementById("sys_discon_txt").innerText;
    var color = document.getElementById("user_color").innerText;

    messages.addEventListener("mousedown", function () { isScrolling = true; });
    messages.addEventListener("mouseup", function () { isScrolling = false; });

    var chatName = "athleanxdotcommer14";// location.pathname.replace('/', '');
    document.cookie = "chatName=" + chatName + "; path=/";

    if (sendBtn != null) {
        sendBtn.addEventListener("click", function () { //debugger
            let message = chat_input.value;

            hubConnection.invoke("Send", message, color, chatName)
                .catch(function (err) {
                    return console.error(err.toString());
                });

            chat_input.value = "";
        });

        var messagesList = [];

        hubConnection.on("Receive", function (message, nickname, color) {
            if (messageCount <= maxMessages) {
                var messageElement = CreateMessage(message, nickname, color);

                messages.append(messageElement);

                messagesList.push(messageElement);

                //for (var i = 1; i < messages.childNodes.length; i++) {
                //    messages.insertBefore(messages.childNodes[i], messages.firstChild);
                //}
            }

            messageCount++;  
        });

        hubConnection.start()
            .then(function () {

                console.log("Connected to SignalR hub. Url: " + hubUrl);

                //CreateSystemMessage

                hubConnection.invoke("JoinChat", chatName).catch(function (err) { debugger
                    return console.error(err.toString());
                });
            })
            .catch(function (err) {
                //return console.error(err.toString());
            });
    }

    function handleScrollbarChange() {
    
        //for (var i in messagesList) {
        //    var element = 
        //}

    }

    function CreateMessage(message, nickname, color) {
        var chat_message = document.createElement("div");
        chat_message.classList.add("chat_message");

        var message_sender = document.createElement("span");
        message_sender.classList.add("message_sender");
        message_sender.style = "color: " + color;
        message_sender.innerText = nickname;

        var message_body = document.createElement("p");
        message_body.classList.add("message_body");
        message_body.innerText = ": " + message;

        chat_message.appendChild(message_sender);
        chat_message.appendChild(message_body);

        return chat_message;
    }

    function CreateSystemMessage(message) {
        var chat_message = document.createElement("div");
        chat_message.classList.add("chat_message");
        chat_message.classList.add("chat_system_message");

        var message_body = document.createElement("p");
        message_body.classList.add("message_body");
        message_body.innerText = message;

        chat_message.appendChild(message_body);

        return chat_message;
    }
}