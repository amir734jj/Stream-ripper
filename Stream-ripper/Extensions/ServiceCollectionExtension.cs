using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StreamRipper.Interfaces;

namespace StreamRipper.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddStreamRipper(this IServiceCollection collection)
        {
            collection.AddScoped<IStreamRipperFactory>(ctx => new StreamRipperFactory(ctx.GetService<ILogger<IStreamRipper>>()));
            
            return collection;
        }
    }
}