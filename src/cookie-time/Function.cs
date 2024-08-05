using Amazon.Lambda.Core;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace cookie_time;

public static class Function
{
    public static async Task FunctionHandler(ILambdaContext context)
    {
        context.Logger.LogInformation("Cookie Time Executing");
        var utcNow = DateTime.UtcNow;
        var tz = TimeZoneInfo.FindSystemTimeZoneById("Europe/London");
        var now = TimeZoneInfo.ConvertTime(utcNow, tz);
        context.Logger.LogInformation(now.ToString());
        var mod = GetMod(now);
        var day = (int)now.DayOfWeek;
        var hour = now.Hour;
        try
        {
            var n = GetN(day, hour);
            if (mod == n)
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://ntfy.sh/cookie_time");
                var content = new StringContent("🚨🍪🚨🍪🚨🍪🚨🍪🚨🍪🚨🍪🚨", null, "text/plain");
                request.Content = content;
                await client.SendAsync(request);
            }
        }
        catch (ScheduleException ex)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://ntfy.sh/cookie_time_errors");
            var content = new StringContent(ex.Message, null, "text/plain");
            request.Content = content;
            await client.SendAsync(request);
        }
        catch (Exception ex)
        {
            context.Logger.LogError(ex.Message);
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://ntfy.sh/cookie_time_errors");
            var content = new StringContent("Check logs for errors", null, "text/plain");
            request.Content = content;
            await client.SendAsync(request);
        }
        context.Logger.LogInformation("Cookie Time Finished");
    }

    private static int GetMod(DateTime now)
    {
        var diff = (7 + (now.DayOfWeek - DayOfWeek.Monday)) % 7;
        var startOfWeek = now.AddDays(-1 * diff).Date.Ticks;
        var rand = new Random((int)startOfWeek);
        var test = rand.NextInt64();
        var mod = (int)(test % 13);
        return mod;
    }

    private static int GetN(int day, int hour)
    {
        return hour switch
        {
            9 => day switch
            {
                5 => 12,
                _ => throw new ScheduleException(day, hour)
            },
            11 => day switch
            {
                0 => 0,
                2 => 2,
                3 => 4,
                4 => 6,
                5 => 8,
                6 => 10,
                _ => throw new ScheduleException(day, hour)
            },
            14 => day switch
            {
                0 => 1,
                2 => 3,
                3 => 5,
                4 => 7,
                5 => 9,
                6 => 11,
                _ => throw new ScheduleException(day, hour)
            },
            _ => throw new ScheduleException(day, hour)
        };
    }
}
