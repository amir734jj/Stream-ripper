using Microsoft.Extensions.Logging;
using StreamRipper.Interfaces;
using StreamRipper.Models;

namespace StreamRipper
{
    internal class StreamRipperFactory : IStreamRipperFactory
    {
        private readonly ILogger _logger;

        public StreamRipperFactory(ILogger logger)
        {
            _logger = logger;
        }
        
        /// <summary>
        /// Static constructor
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public IStreamRipper New(StreamRipperOptions options)
        {
            return new StreamRipperImpl(options, _logger);
        }
    }
}