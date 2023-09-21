using Castle.DynamicProxy;
using CommonCore.Aspects.Autofac.Exception;
using CommonCore.CrossCuttingConcerns.Logging.Log4Net.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommonCore.Utilities.Interceptors
{
    public class AspectInterceptorSelector:IInterceptorSelector
    {
        public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
        {
            var classAttributes = type.GetCustomAttributes<MethodInterceptionBaseAttribute>
                (true).ToList();

            var methodParameters = method.GetParameters();
            var parameterTypes = new List<Type>();
            foreach (var parameter in methodParameters)
                parameterTypes.Add(parameter.ParameterType);

            var methodAttributes = type.GetMethod(method.Name, parameterTypes.ToArray())
                .GetCustomAttributes<MethodInterceptionBaseAttribute>(true);
            classAttributes.AddRange(methodAttributes);
            //classAttributes.Add(new ExceptionLogAspect(typeof(FileLogger)));

            return classAttributes.OrderBy(x => x.Priority).ToArray();
        }
    }
}
