using StreamRipper.Models;

namespace StreamRipper.Interfaces
{
    public interface IStreamRipperFactory
    {
        IStreamRipper New(StreamRipperOptions options);
    }
}