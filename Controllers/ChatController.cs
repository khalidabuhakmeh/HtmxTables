using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;
using HtmxTables.Services;
using Microsoft.AspNetCore.Mvc;

namespace HtmxTables.Controllers
{
    public class ChatController : ControllerBase
    {
        private readonly IViewRenderService _renderer;

        public static List<Message> Messages { get; }
            = new();

        public ChatController(IViewRenderService renderer)
        {
            _renderer = renderer;
        }

        public static List<WebSocket> Clients = new();

        // GET
        [Route("/chatroom"), HttpGet]
        public async Task Index()
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return;
            }

            using var ws = await HttpContext.WebSockets.AcceptWebSocketAsync();
            Clients.Add(ws);
            await Chatting(ws);
        }

        /// <summary>
        /// Please don't use this in production
        /// </summary>
        /// <param name="webSocket"></param>
        private async Task Chatting(WebSocket webSocket)
        {
            WebSocketReceiveResult result;
            do
            {
                var buffer = new byte[1024 * 4];
                do
                {
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                } while (!result.EndOfMessage);

                try
                {
                    var bytes = buffer.Where(b => b != 0).ToArray();
                    var message = JsonSerializer.Deserialize<Message>(Encoding.UTF8.GetString(bytes));
                    Messages.Add(message);

                    // Rerender the current chat messages
                    // and send back to client

                    var view = await _renderer.RenderToStringAsync("ChatRoom", Messages);
                    var response = Encoding.UTF8.GetBytes(view);

                    foreach (var client in Clients.AsParallel())
                    {
                        await client.SendAsync(response,
                            WebSocketMessageType.Text,
                            true,
                            CancellationToken.None);
                    }
                }
                catch (Exception)
                {
                    break;
                }

            } while (!result.CloseStatus.HasValue);

            // I'm out!
            await webSocket.CloseAsync(
                result.CloseStatus.GetValueOrDefault(),
                result.CloseStatusDescription,
                CancellationToken.None
            );
            Clients.Remove(webSocket);
        }

        public record Message(string Text, string Username)
        {
            public DateTime At { get; } = DateTime.Now;
        };
    }
}