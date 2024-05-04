// const socket = new WebSocket("ws://127.0.0.1:3050/");socket.addEventListener('open', (event) => { socket.send("connect"); });

using System.Net.Sockets;
using System.Net;
using System;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Text.Json;
using System.Text.Json.Serialization;
using static Server;
class Server
{
    private static WebSocketServer server;
    public class Command
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "connect";

        [JsonPropertyName("data")]
        public JsonElement Data { get; set; }
    }
    public class Message
    {
        [JsonPropertyName("char")]
        public string Char { get; set; } = "";

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("msg")]
        public string Msg { get; set; } = "";
    }
    public class Socket : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            try
            {
                Command? cmd = JsonSerializer.Deserialize<Command>(e.Data);
                switch (cmd.Type)
                {
                    case "message":
                        server.WebSocketServices["/"].Sessions.Broadcast(cmd.Data.ToString());
                        break;
                    case "connect":
                        Console.WriteLine("User Connected.");
                        Send("Connected.");
                        break;
                    default:
                        Send("Unknown Command.");
                        break;
                }
            } catch (Exception ex)
            {
                Send("ERROR - " + ex.ToString());
                Console.WriteLine("ERROR - " + ex.ToString());
            }
        }
    }
    public static void Main()
    {
        server = new WebSocketServer("ws://127.0.0.1:3050");
        server.AddWebSocketService<Socket>("/");
        server.Start();
        Console.WriteLine("Server started", Environment.NewLine);
        Console.ReadLine();
    }
}