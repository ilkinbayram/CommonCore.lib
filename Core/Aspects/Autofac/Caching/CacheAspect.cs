using Castle.DynamicProxy;
using CommonCore.CrossCuttingConcerns.Caching;
using Microsoft.Extensions.DependencyInjection;
using CommonCore.Utilities.Interceptors;
using CommonCore.Utilities.IoC;


namespace CommonCore.Aspects.Autofac.Caching
{
    public class CacheAspect : MethodInterception
    {
        private int _duration;
        private ICacheManager _cacheManager;
        private readonly bool _forceToCache;
        private string _cacheKey;

        public CacheAspect(string cacheKey, int duration = 60, bool forceToCache = false)
        {
            _duration = duration;
            _cacheManager = ServiceTool.ServiceProvider.GetService<ICacheManager>();
            _forceToCache = forceToCache;
            _cacheKey = cacheKey;
        }

        public override void Intercept(IInvocation invocation)
        {
            _cacheKey = string.Format($"{invocation.Method.ReflectedType.FullName}.{_cacheKey}");
            var arguments = invocation.Arguments.ToList();
            if (_cacheManager.KeyExist(_cacheKey))
            {
                invocation.ReturnValue = _cacheManager.Get(_cacheKey);
                return;
            }
            invocation.Proceed();
            _cacheManager.Add(_cacheKey, invocation.ReturnValue, _duration, _forceToCache);
        }
    }
}
