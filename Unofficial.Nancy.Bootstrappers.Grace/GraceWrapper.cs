using Grace.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Unofficial.Nancy.Bootstrappers.Grace
{
    public class GraceWrapper : IGraceWrapper
    {
        private IExportLocatorScope _scope;

        public IGraceWrapper Parent { get; private set; }

        public GraceWrapper(IExportLocatorScope scope)
        {
            _scope = scope;
        }

        public void Configure(Action<IExportRegistrationBlock> registrationAction)
        {
            if (_scope is IInjectionScope injectionScope)
            {
                injectionScope.Configure(registrationAction);
            }
        }

        public T Locate<T>()
        {
            return _scope.Locate<T>();
        }

        public object Locate(Type type)
        {
            return _scope.Locate(type);
        }

        public List<T> LocateAll<T>()
        {
            // it's faster because of caching and other things.
            return _scope.LocateAll<T>();
        }

        public IGraceWrapper CreateScope()
        {
            return new GraceWrapper(_scope.BeginLifetimeScope()) { Parent = this };
        }

        public void Dispose()
        {
            // _scope?.Dispose();
        }

    }
}
