import GetCookie from "./common.js"

const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:5001/chat")
    .build();

// specific to razor pages
let nickname = GetCookie("username");

var sendBtn = document.getElementById("sendBtn");

if (sendBtn != null) {
    sendBtn.addEventListener("click", function () { //debugger
        let message = document.getElementById("message").value;

        hubConnection.invoke("Send", message, nickname)
            .catch(function (err) {
                return console.error(err.toString());
            });
    });

    hubConnection.on("Receive", function (message, nickname, color) {

        var messageElement = CreateMessage(message, nickname, color);
        console.log(messageElement);
        document.getElementById("chat_messages").appendChild(messageElement);
    });
    debugger
    hubConnection.start()
        .then(function () {
            
        })
        .catch(function (err) {
            return console.error(err.toString());
        });
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
}