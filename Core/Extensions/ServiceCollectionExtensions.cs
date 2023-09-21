using CommonCore.Utilities.IoC;
using Microsoft.Extensions.DependencyInjection;

namespace CommonCore.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDependencyResolvers(this IServiceCollection services,
            ICoreModule[] modules)
        {
            foreach (var module in modules)
            {
               module.Load(services); 
            }

            return ServiceTool.Create(services);
        }
    }
}
