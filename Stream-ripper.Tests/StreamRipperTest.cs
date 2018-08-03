using System;
using System.IO;
using System.Linq;
using Xunit;
using StreamRipper.Builders;
using StreamRipper.Extensions;
using StreamRipper.Models;
using StreamRipper.Models.Events;

namespace StreamRipper.Tests
{
    public class StreamRipperTest
    {
        [Fact]
        public void Test__BasicRipper()
        {
            // ReSharper disable once NotAccessedVariable
            SongInfo songInfo = null;
            
            // Arrange
            StreamRipper streamRipper = null;
            
            streamRipper = new StreamRipper(new Uri("https://rj1.rjstream.com/"), 
                PlugginManagerBuilder
                    .New()
                    .SetOnSongChanged(x =>
                    {
                        songInfo = x.SongInfo;
                        Console.WriteLine(x.SongInfo.SongMetadata.Artist);
                        // ReSharper disable once AccessToModifiedClosure
                        // ReSharper disable once PossibleNullReferenceException
                        // streamRipper.Dispose();
                    }, x =>
                    {
                        // Do not invoke new when it is an advertisement
                        // ReSharper disable once ConvertToLambdaExpression
                        return !(x.SongInfo.SongMetadata.Artist + x.SongInfo.SongMetadata.Title)
                            .ToLower()
                            .ContainsAny("advertisement", "radio");
                    })
                    .Build());
            
            // Start streaming in sync fashion
            streamRipper.Start();
            
            // Act, Assert
            Assert.NotEmpty(songInfo.Stream.ToArray());
        }
    }
}