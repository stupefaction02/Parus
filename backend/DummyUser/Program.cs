using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

string userEntity = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoidGVzdF9pdmFuMTIyIiwibmJmIjoxNjk2OTU5MTM1LCJleHAiOjE2OTcyMTgzMzUsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjUwMDIiLCJhdWQiOiJodHRwczovL2xvY2FsaG9zdDo1MDAyIn0.V4Bja7XIzImd2EhF1cHiGf5cb_Pyr_UuoJLhjfdajdY";
string username = "test_ivan122";
string email = "testivan122@gcom";

Console.WriteLine($"Booting up Dummy user...");
Console.WriteLine($"User info. Username: {username}, Email: {email}");

string wsUri = "wss://localhost:5001/chat?id=";

var negotianteUri = @"https://localhost:5001/chat/negotiate?negotiateVersion=1";

Console.WriteLine($"Negotiating with server...");
string uid = Negotiate();

string[] phrases = new string[3]
{
    "Hello",
    "How are you?",
    "Nice view!"
};

int phraseIndex = 0;

if (!String.IsNullOrEmpty(uid))
{
    Console.WriteLine($"Negotiation is succesufull!");
    
    ClientWebSocket ws = await InitWebSocket(uid);

    if (ws.State == WebSocketState.Open)
    {
        Console.WriteLine($"Websocket was created");
        Console.WriteLine($"Starting loop...");
        await StartLoopAsync(ws, new CancellationToken());
    }
}
else
{
    Console.WriteLine($"Server Error!");
}

async Task StartLoopAsync(ClientWebSocket ws, CancellationToken ct)
{
    while (true)
    {
        Thread.Sleep(5000);

        if (ws.State == WebSocketState.Closed || ws.State == WebSocketState.Aborted)
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
        ArraySegment<byte> buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(phrase));

        await ws.SendAsync(buffer, WebSocketMessageType.Text, true, ct);
    }
}

async Task<ClientWebSocket> InitWebSocket(string id)
{
    var ws = new ClientWebSocket();

    Console.WriteLine($"Creating websocket with uri=" + wsUri + id);
    await ws.ConnectAsync(new Uri(wsUri + id), new CancellationToken());

    //await Task.Run(ReceivingTaskHandler);

    return ws;
}

void ReceivingTaskHandler()
{
    // If you need to get chat responses
}

string Negotiate()
{
    using HttpClient httpClient = new HttpClient();

    HttpRequestMessage negotianteUriRequest = new HttpRequestMessage
    {
        Method = HttpMethod.Post,
        RequestUri = new Uri(negotianteUri)
    };

    //negotianteUriRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain;charset=UTF-8");
    negotianteUriRequest.Headers.Add("Cookie", $"locale=ru; JWT={userEntity}; identity.username={username}; email={email}; username={username}");
    negotianteUriRequest.Headers.Add("X-Requested-With", "XMLHttpRequest");

    var negotianteResponse = httpClient.Send(negotianteUriRequest);

    if (negotianteResponse.IsSuccessStatusCode)
    {
        string jsonString;
        using (var inputStream = new StreamReader(negotianteResponse.Content.ReadAsStream()))
        {
            jsonString = inputStream.ReadToEnd();
        }

        var result = JsonSerializer.Deserialize<NegotiationResponse>(jsonString);

        return result.connectionToken;
    }

    return null;
}

public class NegotiationResponse
{
    public string connectionToken { get; set; }
}