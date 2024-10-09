
using System;
using System.IO;

public static class DotEnv
{
    public static bool Load(string filePath)
    {
        var status = false;
        if (!File.Exists(filePath))
        {
            Console.WriteLine("No .env file found!");
            return status;

        }
            
        foreach (var line in File.ReadAllLines(filePath))
        {
            var parts = line.Split('=',StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
                continue;
            
            status = true;
            Environment.SetEnvironmentVariable(parts[0], parts[1]);
        }
        return status;
    }
}
