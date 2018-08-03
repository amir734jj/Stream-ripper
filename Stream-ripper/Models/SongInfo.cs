using System;
using System.IO;
using StreamRipper.Extensions;

namespace StreamRipper.Models
{
    public class SongInfo: IDisposable, ICloneable
    {
        public SongMetadata SongMetadata { get; set; }
        
        public MemoryStream Stream { get; set; }

        /// <summary>
        /// Dispose self
        /// </summary>
        public void Dispose()
        {
            Stream?.Dispose();
            Stream?.Clear();
        }

        /// <summary>
        /// Clone self
        /// </summary>
        /// <returns></returns>
        public object Clone() => new SongInfo
        {
            SongMetadata = (SongMetadata) SongMetadata?.Clone(),
            Stream = Stream?.Clone()
        };
    }
}