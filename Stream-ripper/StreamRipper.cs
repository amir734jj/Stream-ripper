using StreamRipper.Interfaces;
using StreamRipper.Models;

namespace StreamRipper
{
    public static class StreamRipperFactory
    {
        /// <summary>
        /// Static constructor
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IStreamRipper New(StreamRipperOptions options)
        {
            return new StreamRipperImpl(options);
        }
    }
}