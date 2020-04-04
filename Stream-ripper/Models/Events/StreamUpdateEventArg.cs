using StreamRipper.Interfaces;

namespace StreamRipper.Models.Events
{
    public class StreamUpdateEventArg : IEvent
    {
        public byte[] SongRawPartial { get; set; }
    }
}