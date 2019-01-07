using System;
using System.Collections.Immutable;
using StreamRipper.Interfaces;
using StreamRipper.Models;

namespace StreamRipper.Builders
{
    public class StreamRipperBuilder: IStreamRipperBuilderUrl
    {
        public static StreamRipperBuilder New()
        {
            return new StreamRipperBuilder();
        }
        
        public IStreamRipperBuilderFilters WithUrl(Uri uri)
        {
            return new StreamRipperBuilderFilters(uri);
        }
    }

    public class StreamRipperBuilderFilters : IStreamRipperBuilderFilters
    {
        private readonly Uri _uri;
        
        private ImmutableList<Func<SongMetadata, bool>> _filters;

        public StreamRipperBuilderFilters(Uri uri)
        {
            _uri = uri;
            _filters = ImmutableList<Func<SongMetadata, bool>>.Empty;
        }
        
        public IStreamRipperBuilderFilters WithFilter(Func<SongMetadata, bool> filter)
        {
            _filters = _filters.Add(filter);

            return this;
        }

        public IStreamRipperBuilderFinalize FinalizeFilters()
        {
            return new StreamRipperBuilderFinalize(_uri, _filters);
        }
    }

    public class StreamRipperBuilderFinalize : IStreamRipperBuilderFinalize
    {
        private readonly Uri _uri;
        
        private readonly ImmutableList<Func<SongMetadata, bool>> _filters;

        public StreamRipperBuilderFinalize(Uri uri, ImmutableList<Func<SongMetadata, bool>> filters)
        {
            _uri = uri;
            _filters = filters;
        }
        
        public IStreamRipper Build()
        {
            return new StreamRipper(_uri, _filters);
        }
    }
}