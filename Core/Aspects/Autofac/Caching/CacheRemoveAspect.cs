using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;

using CommonCore.CrossCuttingConcerns.Caching;
using CommonCore.Utilities.Interceptors;
using CommonCore.Utilities.IoC;

namespace CommonCore.Aspects.Autofac.Caching
{
    public class CacheRemoveAspect : MethodInterception
    {
        private string _key;
        private ICacheManager _cacheManager;

        public CacheRemoveAspect(string key)
        {
            _key = key;
            _cacheManager = ServiceTool.ServiceProvider.GetService<ICacheManager>();
        }

        protected override void OnSuccess(IInvocation invocation)
        {
            _key = string.Format($"{invocation.Method.ReflectedType.FullName}.{_key}");
            _cacheManager.TerminateList(_key);
        }
    }
}
