using System;
using Microsoft.Extensions.Logging;
using StreamRipper;
using StreamRipper.Interfaces;

namespace TestConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            IStreamRipper stream = new StreamRipperImpl(new Uri("http://stream.radiojavan.com/radiojavan"),
                LoggerFactory.Create(o => o.AddConsole()).CreateLogger<Program>());

            stream.SongChangedEventHandlers += (_, arg) =>
            {
                Console.WriteLine(arg);
            };
            
            stream.Start();

            Console.ReadKey();
        }
    }
}