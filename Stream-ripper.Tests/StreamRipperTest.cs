using System.IO;
using System.Linq;
using Xunit;
using StreamRipper.Builders;
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
            
            streamRipper = new StreamRipper("https://rj1.rjstream.com/",
                PlugginManagerBuilder
                    .New()
                    .SetOnSongChanged(x =>
                    {
                        songInfo = x.SongInfo;
                        // ReSharper disable once AccessToModifiedClosure
                        // ReSharper disable once PossibleNullReferenceException
                        streamRipper.Dispose();
                    })
                    .Build());
            
            streamRipper.Start();
            
            // Act, Assert
            Assert.NotEmpty(songInfo.Bytes);
        }
    }
}