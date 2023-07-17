const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/chat")
    .build();

document.getElementById("sendBtn").addEventListener("click", function () { debugger
    let message = document.getElementById("message").value;
    hubConnection.invoke("Send", message)
        .catch(function (err) {
            return console.error(err.toString());
        });
});

hubConnection.on("Receive", function (message) {
    debugger
    let messageElement = document.createElement("p");
    messageElement.textContent = message;
    document.getElementById("chatroom").appendChild(messageElement);
});

hubConnection.start()
    .then(function () {
        document.getElementById("sendBtn").disabled = false;
    })
    .catch(function (err) {
        return console.error(err.toString());
    });