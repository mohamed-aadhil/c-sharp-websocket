using ForeignTimeWebSocket.Services;
using Microsoft.AspNetCore.Mvc;

namespace ForeignTimeWebSocket.Controllers;

[ApiController]
public class WebSocketController : ControllerBase
{
    private readonly TimeService _timeService;

    public WebSocketController(TimeService timeService)
    {
        _timeService = timeService;
    }

    [Route("/ws/time")]
    public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            var buffer = new byte[1024 * 4];

            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var timeZoneId = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);

            Console.WriteLine($"[WebSocket] Time zone selected: {timeZoneId}");

            while (webSocket.State == System.Net.WebSockets.WebSocketState.Open)
            {
                var time = _timeService.GetCurrentTimeForZone(timeZoneId);
                var json = System.Text.Json.JsonSerializer.Serialize(time);
                var bytes = System.Text.Encoding.UTF8.GetBytes(json);
                await webSocket.SendAsync(new ArraySegment<byte>(bytes), System.Net.WebSockets.WebSocketMessageType.Text, true, CancellationToken.None);

                await Task.Delay(1000);
            }

            await webSocket.CloseAsync(System.Net.WebSockets.WebSocketCloseStatus.NormalClosure, "Done", CancellationToken.None);
        }
        else
        {
            HttpContext.Response.StatusCode = 400;
        }
    }
}
