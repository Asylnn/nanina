using System;
using System.Buffers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Configuration;
using System.Threading;
using LiteDB;
using System.Runtime.Intrinsics.X86;
using WebSocketSharp.Server;
using WebSocketSharp;
using Internal;


namespace Nanina
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Server Launched!");
            LoadServer.Load();
            
            Thread.Sleep(100);
            
            Console.WriteLine ("Press any key to shut down the server...");
            Console.ReadKey();
        }
    }
}