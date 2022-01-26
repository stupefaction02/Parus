import * as signalR from "@microsoft/signalr";

export default class SignalRClient {

     connect(url) {
          console.log("Client type: SignalR");
          console.log("Connecting...");

          this.hubConnection = new signalR.HubConnectionBuilder()
               .configureLogging(signalR.LogLevel.Error)
               .withUrl(url)
               .build();

          let self = this;

          let start = function(connection) {
               try {
                    connection.start().then(() => {
                         
                         var hubId = 'id';

                         console.log('Connection established. HubId=`hubId`');
                         console.log(self.hubConnection.state);
                    })
                    .catch(errinfo => { 
                         console.log('Connection Error: ' + errinfo);
                         
                         setTimeout(() => 
                         {
                              console.log('Trying ro reconnect...');
                              start(connection);
                         }, 3000);
                    });
               } catch (error) {
                    console.log('Connection Error: ' + error);
                    
                    setTimeout(() => 
                    {
                         console.log('Trying ro reconnect...');
                         start(connection);
                    }, 3000);
               }
          }

          this.hubConnection.on('ReceiveChatMessage', data => {
               debugger
          });

          start(self.hubConnection);

          console.log('SignalR Client connected state: ' + this.hubConnection.state);

          this.hubConnection.onclose(msg => {
               debugger

               console.log('Connection failed. Trying to reconnect...');

               start(self.hubConnection);
          });

          this.hubConnection.serverTimeoutInMilliseconds = 1000 * 120 * 10;

          //this.hubConnection.

          if (this.hubConnection.state == signalR.HubConnectionState.Connected)
          {
               debugger
          }
     }

     send(sendMessage) {
         this.hubConnection.invoke("Send", sendMessage);
     } 
}