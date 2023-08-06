const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:5001/chat")
    .build();

function getCookie(name) {
    let matches = document.cookie.match(new RegExp(
        "(?:^|; )" + name.replace(/([\.$?*|{}\(\)\[\]\\\/\+^])/g, '\\$1') + "=([^;]*)"
    ));
    return matches ? decodeURIComponent(matches[1]) : undefined;
}

// specific to razor pages
let nickname = getCookie("username");

var sendBtn = document.getElementById("sendBtn");

if (sendBtn != null) {
    sendBtn.addEventListener("click", function () { //debugger
        let message = document.getElementById("message").value;

        hubConnection.invoke("Send", message, nickname)
            .catch(function (err) {
                return console.error(err.toString());
            });
    });

    hubConnection.on("Receive", function (message, nickname) {

        let messageElement = document.createElement("p");
        messageElement.textContent = nickname + ": " + message;
        console.log(messageElement.textContent);
        document.getElementById("chatroom").appendChild(messageElement);
    });

    hubConnection.start()
        .then(function () {
            document.getElementById("sendBtn").disabled = false;
        })
        .catch(function (err) {
            return console.error(err.toString());
        });
}