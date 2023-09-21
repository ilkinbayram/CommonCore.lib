using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;

using CommonCore.CrossCuttingConcerns.Caching;
using CommonCore.Utilities.Interceptors;
using CommonCore.Utilities.IoC;

namespace CommonCore.Aspects.Autofac.Caching
{
    public class CacheRemoveAspect : MethodInterception
    {
        private string _pattern;
        private ICacheManager _cacheManager;

        public CacheRemoveAspect(string pattern)
        {
            _pattern = pattern;
            _cacheManager = ServiceTool.ServiceProvider.GetService<ICacheManager>();
        }

        protected override void OnSuccess(IInvocation invocation)
        {
            _cacheManager.RemoveByPattern(_pattern);
        }
    }
}
