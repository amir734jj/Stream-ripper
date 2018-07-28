using System.Collections.ObjectModel;

namespace StreamRipper.Constants
{
    public class MetadataPatterns
    {
        private static readonly ReadOnlyCollection<string> MetadataSongPatterns = new ReadOnlyCollection<string>(new[]
        {
            @"StreamTitle='(?<title>[^~]+?) - (?<artist>[^~]+?)'",
            @"StreamTitle='(?<title>.+?)~(?<artist>.+?)~"
        });
    }
}