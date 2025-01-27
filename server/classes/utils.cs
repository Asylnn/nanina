using System.Diagnostics;
using System.Runtime.CompilerServices;

public static class Utils
{
    public static long GetTimestamp()
    {
        return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
    }
}