using System.Diagnostics;
using System.Runtime.CompilerServices;

public static class Utils
{
    public static ulong GetTimestamp()
    {
        return (ulong)new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
    }
}