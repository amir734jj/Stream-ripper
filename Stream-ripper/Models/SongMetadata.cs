using System;

namespace StreamRipper.Models
{
    /// <summary>
    /// Song metadata model
    /// </summary>
    public class SongMetadata: ICloneable
    {
        public string Artist { get; set; }
        
        public string Title { get; set; }
        
        public string Raw { get; set; }

        /// <summary>
        /// Clone self
        /// </summary>
        /// <returns></returns>
        public object Clone() => new SongMetadata
        {
            Artist = (string) Artist.Clone(),
            Title = (string) Title.Clone()
        };
    }
}