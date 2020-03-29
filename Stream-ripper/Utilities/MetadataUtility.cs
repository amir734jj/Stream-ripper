using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using StreamRipper.Constants;
using StreamRipper.Models;

namespace StreamRipper.Utilities
{
    public static class MetadataUtility
    {
        public static SongMetadata ParseMetadata(string str)
        {
            var artist = new List<string>();
            var title = new List<string>();
            
            // Loop through patterns
            foreach (var metadataSongPattern in MetadataPatterns.MetadataSongPatterns)
            {
                var match = Regex.Match(str, metadataSongPattern);
                
                if (match.Success)
                {
                    artist.Add(match.Groups["artist"].Value.Trim());
                    title.Add(match.Groups["title"].Value.Trim());
                }
            }

            // Return the first not null or empty matches
            return new SongMetadata
            {
                Artist = artist.FirstOrDefault(x => !string.IsNullOrEmpty(x)),
                Title = title.FirstOrDefault(x => !string.IsNullOrEmpty(x)),
                Raw = str
            };
        }
    }
}