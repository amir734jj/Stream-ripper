using StreamRipper.Interfaces;
using StreamRipper.Models.Song;

namespace StreamRipper.Models.Events
{
    public class StreamStartedEventArg : IEvent
    {
        public SongInfo SongInfo { get; set; }
    }
}