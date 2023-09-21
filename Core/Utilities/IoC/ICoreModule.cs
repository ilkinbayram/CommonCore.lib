using Microsoft.Extensions.DependencyInjection;

namespace CommonCore.Utilities.IoC
{
    public interface ICoreModule
    {
        void Load(IServiceCollection collection);
    }
}
