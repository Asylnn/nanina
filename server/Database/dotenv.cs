namespace Nanina.Database
{
    public static class DotEnv
    {
        public static void Load(string filePath)
        {
            var exit = true;
            if (!File.Exists(filePath))
                Console.WriteLine("No .env file found!");
                
            foreach (var line in File.ReadAllLines(filePath))
            {
                var parts = line.Split('=',StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                    continue;
                
                exit = false;
                Environment.SetEnvironmentVariable(parts[0], parts[1]);
            }
            if(exit)
                System.Environment.Exit(1);
        }
    }
}

