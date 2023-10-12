using System.Linq.Expressions;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;

string userEntity = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoidGVzdF9pdmFuMTIyIiwibmJmIjoxNjk2OTU5MTM1LCJleHAiOjE2OTcyMTgzMzUsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjUwMDIiLCJhdWQiOiJodHRwczovL2xvY2FsaG9zdDo1MDAyIn0.V4Bja7XIzImd2EhF1cHiGf5cb_Pyr_UuoJLhjfdajdY";
string username = "test_ivan122";
string email = "testivan122@gcom";

int timeout = 5000;

Console.WriteLine($"Booting up Dummy user...");
Console.WriteLine($"User info. Username: {username}, Email: {email}");

Console.WriteLine($"Connecting...");
HubConnection connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:5001/chat", ConfigureConnection)
                .Build();

void ConfigureConnection(HttpConnectionOptions options)
{
    Cookie cookie1 = new Cookie("JWT", userEntity, "/", "localhost");
    Cookie cookie2 = new Cookie("email", email, "/", "localhost");
    Cookie cookie3 = new Cookie("username", username, "/", "localhost");
    Cookie cookie4 = new Cookie("identity.username", username, "/", "localhost");
    options.Cookies.Add(cookie1);
    options.Cookies.Add(cookie2);
    options.Cookies.Add(cookie3);
    options.Cookies.Add(cookie4);
}

string[] phrases = new string[3]
{
    "Hello",
    "How are you?",
    "Nice view!"
};

string[] colors = new string[3]
{
    "#333333",
    "#999999",
    "#000000"
};

string color = PickRandomColor();

int phraseIndex = 0;

L2:
try
{ 
    await connection.StartAsync();

    if (connection.State == HubConnectionState.Connected)
    {
        Console.WriteLine($"Connected!");

        Console.WriteLine($"Starting loop...");

        await StartLoopAsync(connection, new CancellationToken());
    }
}
catch (HttpRequestException e)
{
    Console.WriteLine($"Can't establishe connection. Trying to reconnect...");
    e = null;
    Thread.Sleep(5000);
    goto L2;
}

async Task StartLoopAsync(HubConnection ws, CancellationToken ct)
{
    L1:
    try
    {
        while (true)
        {
            if (ws.State == HubConnectionState.Disconnected)
            {
                Console.WriteLine($"Connection was closed or aborted! Shutting down...");
                break;
            }

            if (phraseIndex == 3)
            {
                phraseIndex = 0;
            }

            string phrase = phrases[phraseIndex];
            phraseIndex++;

            await ws.InvokeAsync("Send", phrase, color);

            Thread.Sleep(500);
        }
    }
    catch (Exception e)
    {
        Thread.Sleep(timeout);
        Console.WriteLine($"Trying to reconect...");
        e = null;
        goto L1;
    }
}

string PickRandomColor()
{
    var rng = new Random();

    return colors[(int)rng.Next(0, colors.Length)];
}