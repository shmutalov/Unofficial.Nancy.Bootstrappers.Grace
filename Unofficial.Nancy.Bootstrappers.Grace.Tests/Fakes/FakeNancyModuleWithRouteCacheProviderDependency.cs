using Nancy;
using Nancy.Routing;

namespace Unofficial.Nancy.Bootstrappers.Grace.Tests.Fakes
{
    public class FakeNancyModuleWithRouteCacheProviderDependency : NancyModule
    {
        public IRouteCacheProvider RouteCacheProvider { get; private set; }

        public FakeNancyModuleWithRouteCacheProviderDependency(IRouteCacheProvider routeCacheProvider)
        {
            RouteCacheProvider = routeCacheProvider;
        }
    }
}