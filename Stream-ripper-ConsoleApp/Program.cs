using System;
using System.IO;
using System.Threading.Tasks;
using StreamRipper.Builders;
using StreamRipper.Extensions;

namespace StreamRipper.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var streamRipper = StreamRipper.New(new Uri("https://rj1.rjstream.com/"));

            streamRipper.SongChangedEventHandlers += async (_, arg) =>
            {
                var filename = $"{arg.SongInfo.SongMetadata.Artist}-{arg.SongInfo.SongMetadata.Title}";
                Console.WriteLine(filename);
                await arg.SongInfo.Stream.ToFileStream($@"C:\Users\Amir-Acer\Music\ripped\{filename}.mp3");
            };
            
            streamRipper.Start();

            Console.ReadKey();
        }
    }
}