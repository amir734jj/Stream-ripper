using System;
using StreamRipper.Models;

namespace StreamRipper.Interfaces
{
    public interface IStreamRipperBuilderUrl
    {
        IStreamRipperBuilderFilters WithUrl(Uri uri);
    }
    
    public interface IStreamRipperBuilderFilters
    {
        IStreamRipperBuilderFilters WithFilter(Func<SongMetadata, bool> filter);
        
        IStreamRipperBuilderFinalize FinalizeFilters();
    }
    
    public interface IStreamRipperBuilderFinalize
    {
        IStreamRipper Build();
    }
}