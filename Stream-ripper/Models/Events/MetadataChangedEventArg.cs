using StreamRipper.Interfaces;
using StreamRipper.Models.Song;

namespace StreamRipper.Models.Events
{
    public class MetadataChangedEventArg : IEvent
    {
        public SongMetadata SongMetadata { get; set; }
    }
}