﻿using System.Linq.Expressions;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;

public class Program
{
    private static void Main(string[] args)
    {
        Console.Title = "DummyUser";

        StartBroadcast(args).GetAwaiter().GetResult();

        CleanUp().GetAwaiter().GetResult();

        Console.ReadKey();

        return;
        //if (args.Length > 0)
        //{
        //    string command = args[0];
        //    Console.WriteLine(command);
        //    switch (command)
        //    {
        //        case "flood":
        //            Task.Run(async () => { await FloodChat(args); });
        //            break;
        //        default:
        //        case "start_broadcast":
        //            Task.Run(async () => { await StartBroadcast(args); });
        //            break;
        //    }
        //}

        
    }

    private static async Task FloodChat(string[] args)
    {
        string userEntity = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoidGVzdF9pdmFuMTIyIiwibmJmIjoxNjk2OTU5MTM1LCJleHAiOjE2OTcyMTgzMzUsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjUwMDIiLCJhdWQiOiJodHRwczovL2xvY2FsaG9zdDo1MDAyIn0.V4Bja7XIzImd2EhF1cHiGf5cb_Pyr_UuoJLhjfdajdY";
        string username = "test_ivan122";
        string email = "testivan122@gcom";

        string chatName = "ivan21";

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
            Cookie cookie5 = new Cookie("chatName", chatName, "/", "localhost");
            options.Cookies.Add(cookie1);
            options.Cookies.Add(cookie2);
            options.Cookies.Add(cookie3);
            options.Cookies.Add(cookie4);
            options.Cookies.Add(cookie5);
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

            return colors[rng.Next(0, colors.Length)];
        }
    }

    private static IEnumerable<User> ConfirmedUsers;

    static string AdminJWT => "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW4iLCJuYmYiOjE2OTc4MDQ2MTEsImV4cCI6MTc1NDA1MTAxMSwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NTAwMiIsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0OjUwMDIifQ.wOOfJlyvSCf6VqPsOxx8HsONDbRu5TqKEOrzyAZibkU";

    private static async Task StartBroadcast(string[] args)
    {
        //await CleanUp();

        var users = ConfirmedUsers = (await GetUsers()).Take(3);

        Console.WriteLine("Stopping all broadcasts...");
        await CleanUp();

        Console.WriteLine("");

        Console.WriteLine("Starting all broadcasts...");
        await StartBroadcastCore(users);

        try
        {
            List<Task> tasks = new List<Task>();
            foreach (User confirmedUser in users)
            {
                tasks.Add(RunOBSAsync(confirmedUser));
            }

            await Task.WhenAll(tasks.ToArray());
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
            Console.WriteLine($"Error occured on thread {Thread.CurrentThread.ManagedThreadId}");
            Console.ForegroundColor = ConsoleColor.White;
        }
        finally
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("It is happening again.");
            Console.WriteLine($"Error occured on thread {Thread.CurrentThread.ManagedThreadId}");
            Console.Out.Flush();
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    private static List<User> broadcastingUsers = new List<User>();

    private static async Task StartBroadcastCore(IEnumerable<User> users)
    {
        foreach (User usr in users)
        {
            string uri = CreateBroadcastCreationUri();
            
            broadcastingUsers.Add(usr);

            Console.WriteLine($"{usr.username} has started broadcast. uri={uri}");

            CreatedApiResponse res = await GetJSON<CreatedApiResponse>(uri, usr.jwt, HttpMethod.Post);

            if (res != null)
            {
                usr.broadcastKey = res.broadcastKey;    
            }
        }
    }

    private class CreatedApiResponse
    {
        public string broadcastKey;
    }

    private static async Task StopBroadcast(IEnumerable<User> users)
    {
        foreach (User usr in users)
        {
            string uri = $"{api}/broadcasts/stop?broadcastKey=amogus";

            Console.WriteLine($"{usr.username} is stoping broadcast... uri={uri}");

            var y = await Request(uri, usr.jwt, HttpMethod.Delete);
        }
    }

    static int broadcastIndex = 0;

    static string api = "https://localhost:5001/api";

    private static async Task<string> GetStringAsync(string uri)
    {
        string ret;

        HttpResponseMessage resp = await Request(uri, "", HttpMethod.Get);

        using (StreamReader ms = new StreamReader(resp.Content.ReadAsStream()))
        {
            ret = ms.ReadToEnd();
        }

        return ret;
    }

    private static string CreateBroadcastCreationUri()
    {
        broadcastIndex++;

        string title = $"Test broadcast %231{broadcastIndex}";

        return $"{api}/broadcasts/start?title={title}&catId=1&tags[]=1,2";
    }

    private static Task RunOBSAsync(User user)
    {
        DeshOBS obs = new DeshOBS(user);

        // fall a settings of broadcast
        return Task.Run(async () => await obs.RunAsync());
    }

    private async static Task<List<User>> GetUsers()
    {
        string uri = "https://localhost:5001/api/test/userslimited";

        List<User> lst = new List<User>();
        foreach (User user in await GetJSON<List<User>>(uri, AdminJWT, HttpMethod.Get))
        {
            lst.Add(user);
        }

        return lst;
    }

    private static async Task<T> GetJSON<T>(string uri, string jwt, HttpMethod method)
    {
        HttpResponseMessage response = await Request(uri, jwt, method);

        return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
    }

    private static async Task<bool> CleanUp()
    {
        foreach (string file in Directory.GetFiles("manifest"))
        {
            File.Delete(file);
        }

        foreach (string dir in Directory.GetDirectories("segments"))
        {
            Directory.Delete(dir, true);
        }

        string uri = "https://localhost:5001/api/test/purgeallbroadcasts";
        
        return (await Request(uri, AdminJWT, HttpMethod.Delete)).IsSuccessStatusCode;
    }

    private async static Task<HttpResponseMessage> Request(string uri, string jwt, HttpMethod method)
    {
        using HttpClient httpClient = new HttpClient();

        HttpRequestMessage request = new HttpRequestMessage
        {
            Method = method,
            RequestUri = new Uri(uri)
        };

        request.Headers.Add("Cookie", $"locale=ru; JWT={jwt};");
        request.Headers.Add("X-Requested-With", "XMLHttpRequest");
        //request.Headers.Add("Content-Type", "text/plain");

        return await httpClient.SendAsync(request);
    }

    public class User
    {
        public string username;
        public string email;
        public string id;
        public string emailConfirmed;
        public string jwt;
        internal string broadcastKey;
    }
}