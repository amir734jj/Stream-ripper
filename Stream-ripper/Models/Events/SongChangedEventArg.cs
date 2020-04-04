using StreamRipper.Interfaces;
using StreamRipper.Models.Song;

namespace StreamRipper.Models.Events
{
    public class SongChangedEventArg : IEvent
    {        
        public SongInfo SongInfo { get; set; }
    }
}