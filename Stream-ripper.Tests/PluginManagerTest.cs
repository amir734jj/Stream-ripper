using System;
using System.Threading.Tasks;
using AutoFixture;
using StreamRipper.Builders;
using StreamRipper.Models;
using StreamRipper.Models.Events;
using StreamRipper.Plugins;
using Xunit;

namespace StreamRipper.Tests
{
    public class PlugginManagerTest
    {
        private readonly Fixture _fixture;

        public PlugginManagerTest()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public async Task Test__Flow()
        {
            // Arrange
            var called = false;
            var action = new Action<SongChangedEventArg>(_ =>
            {
                called = true;
            });

            var plugginManager = PlugginManagerBuilder.New()
                .SetOnSongChanged(action)
                .Build();
            
            // Act
            plugginManager.OnMetadataChanged(new MetadataChangedEventArg
            {
                SongMetadata = _fixture.Create<SongMetadata>()
            });
            
            plugginManager.OnMetadataChanged(new MetadataChangedEventArg
            {
                SongMetadata = _fixture.Create<SongMetadata>()
            });

            await Task.Delay(15000).ContinueWith(_ =>
            {
                Assert.True(called);
            });
        }
    }
}