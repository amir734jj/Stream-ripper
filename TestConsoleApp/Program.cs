using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StreamRipper.Extensions;
using StreamRipper.Interfaces;
using StreamRipper.Models;

namespace TestConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging(cfg => cfg.AddConsole())
                .Configure<LoggerFilterOptions>(cfg => cfg.MinLevel = LogLevel.Trace)
                .AddStreamRipper()
                .BuildServiceProvider();

            var streamRipperFactory = serviceProvider.GetService<IStreamRipperFactory>();

            var stream = streamRipperFactory.New(new StreamRipperOptions
            {
                Url = new Uri("http://stream.radiojavan.com/radiojavan"),
                MaxBufferSize = 10 * 1000000    // stop when buffer size passes 10 megabytes
            });

            if (!await stream.CheckUrlValidAsync())
            {
                Console.WriteLine("Stream URL is not valid or not reachable.");
                return;
            }

            stream.SongChangedEventHandlers += (_, arg) =>
            {
                Console.WriteLine(arg.SongInfo);

                File.WriteAllBytes($"{arg.SongInfo.SongMetadata}.mp3", arg.SongInfo.Stream.ToArray());

                arg.SongInfo.Dispose();
            };
            
            stream.Start();

            Console.ReadKey();

            stream.Dispose();
        }
    }
}