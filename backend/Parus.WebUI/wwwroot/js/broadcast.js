import { GetCookie, IsStringEmpty } from "./common.js";
import { CHAT_API_PATH, VIDEO_EDGE_PATH } from "./config.js";
import { HLSPlayer } from "./HlsPlayer/HLSPlayer.js";
import { ApiPostRequest } from "./site.js";

var play_button = document.getElementById("play_button");
var plays = true;

var hlsServiceUrl = VIDEO_EDGE_PATH;

var video = document.getElementById('video');

var manifestUrl = `${hlsServiceUrl}/live/123456/master_playlist.m3u8`;

var url = VIDEO_EDGE_PATH + "/live/desh02/1_dash.mpd";
var player = dashjs.MediaPlayer().create();

class SignalRChat {
    constructor(signalR) {
        //debugger
        this.messages = document.getElementById("messages");
        this.chatInput = document.getElementById("chat_input");

        this.sys_con_txt = document.getElementById("sys_con_txt").innerText;
        this.sys_discon_txt = document.getElementById("sys_discon_txt").innerText;

        // TODO: this can be fetch from server, in OnStartSucceded there could be a needed value in response
        this.usernameColor = document.getElementById("user_color").innerText;

        this.messages.addEventListener("mousedown", function () { isScrolling = true; });
        this.messages.addEventListener("mouseup", function () { isScrolling = false; });

        this.chatId = GetCookie("chatid");
        this.authenticationValue = GetCookie("JWT");
        this.messagesList = [];
        this.maxMessages = 50;
        this.messageCount = 0;
        this.isScrolling = false;

        console.log("debug: chatid=" + this.chatId);

        this.hubConnection = new signalR.HubConnectionBuilder()
            .withUrl(CHAT_API_PATH, {
                headers: { "Authentication": "Bearer " + this.authenticationValue },
                accessTokenFactory: () => {
                    return "Bearer " + this.authenticationValue;
                }
            })
            .build();

        document.getElementById("sendBtn").addEventListener("click", () => this.OnSendingMessage(this.chatInput));

        this.hubConnection.on("Receive", (message, nickname, color) => this.OnReceive(message, nickname, color));
        this.hubConnection.on("HandleErrors", (response) => this.HandleErrors(response));

        this.hubConnection.start()
            .then((response) => this.OnStartSucceded(response))
            .catch((response) => this.OnStartFailed(response));
    }

    OnStartSucceded(response) {
        console.log("Connected to SignalR hub. Url: " + CHAT_API_PATH);
        
        this.hubConnection.invoke("JoinChat", this.chatId).catch(this.OnError);
    }

    OnError(error) {
        console.log(error);
    }

    OnStartFailed(error) {
        console.log(error);
    }

    OnReceive(message, nickname, color) {

        if (this.messageCount <= this.maxMessages) {
            var messageElement = this.CreateMessage(message, nickname, color);

            this.messages.append(messageElement);

            this.messagesList.push(messageElement);

            //for (var i = 1; i < messages.childNodes.length; i++) {
            //    messages.insertBefore(messages.childNodes[i], messages.firstChild);
            //}
        }

        this.messageCount++;
    }

    OnSendingMessage(chatInput) {
        var message = chatInput.value;

        if (IsStringEmpty(message)) return;

        this.hubConnection.invoke("Send", message, "red", this.chatId).catch(this.OnSendingError);

        this.chatInput.value = "";
    }

    OnSendingError(response) {
        console.error(err.toString());
    }

    CreateMessage(message, nickname, color) {
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

    CreateSystemMessage(message) {
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

if (signalR !== null) {
    var chat = new SignalRChat(signalR);
}