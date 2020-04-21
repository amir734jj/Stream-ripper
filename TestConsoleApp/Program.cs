using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StreamRipper;
using StreamRipper.Interfaces;
using StreamRipper.Models;

namespace TestConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging(cfg => cfg.AddConsole())
                .Configure<LoggerFilterOptions>(cfg => cfg.MinLevel = LogLevel.Trace)
                .BuildServiceProvider();

            var stream = new StreamRipperImpl(new StreamRipperOptions
            {
                Url = new Uri("http://stream.radiojavan.com/radiojavan"),
                Logger = serviceProvider.GetService<ILogger<IStreamRipper>>(),
                MaxBufferSize = 10 * 1000000    // stop when buffer size passes 10 megabytes
            });

            stream.SongChangedEventHandlers += (_, arg) =>
            {
                Console.WriteLine(arg.SongInfo);

                File.WriteAllBytes($"{arg.SongInfo.SongMetadata}.mp3", arg.SongInfo.Stream.ToArray());
            };
            
            stream.Start();

            Console.ReadKey();
        }
    }
}