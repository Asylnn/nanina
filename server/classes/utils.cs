using System.Diagnostics;
using System.Runtime.CompilerServices;

public static class Utils
{
    public static string GetTimestamp()
    {
        return (string) new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds().ToString();
    }
}