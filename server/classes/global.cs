using System.Net.Http;
using RestSharp;
public static class Global {
    public static RestClient client = new RestClient("");

    public static async Task RunInBackground(TimeSpan timeSpan, Action action)
    {
        var periodicTimer = new PeriodicTimer(timeSpan);
        while (await periodicTimer.WaitForNextTickAsync())
        {
            action();
        }
    }
}