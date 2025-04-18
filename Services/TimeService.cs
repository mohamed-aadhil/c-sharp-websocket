using ForeignTimeWebSocket.Models;

namespace ForeignTimeWebSocket.Services;

public class TimeService
{
    public TimeResponse GetCurrentTimeForZone(string timeZoneId)
    {
        try
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var localTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, timeZoneInfo);
            return new TimeResponse
            {
                Time = localTime.ToString("HH:mm:ss"),
                TimeZone = timeZoneId
            };
        }
        catch
        {
            return new TimeResponse
            {
                Time = "Invalid Time Zone",
                TimeZone = timeZoneId
            };
        }
    }
}
