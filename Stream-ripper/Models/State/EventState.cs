using Microsoft.Extensions.Logging;
using StreamRipper.Models.Song;

namespace StreamRipper.Models.State
{
    public class EventState
    {
        public EventState(string url, ILogger logger)
        {
            Url = url;
            Logger = logger;
            SongInfo = new SongInfo();
        }

        public string Url { get; }
                
        public ILogger Logger { get; }

        public SongInfo SongInfo { get; set; }
        
        public int Count { get; set; }

        public EventHandlers EventHandlers { get; set; }

        public SongMetadata PrevSongMetadata { get; set; }
    }
}