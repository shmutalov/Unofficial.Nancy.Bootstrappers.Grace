using Nancy;
using Nancy.Routing;

namespace Unofficial.Nancy.Bootstrappers.Grace.Tests.Fakes
{
    /// <summary>
    /// NOTE: Coupling with IRouteCacheProvider can cause cyclic dependencies (cannot be resolved by Grace)
    /// SEE: https://github.com/ipjohnson/Grace/issues/180
    /// </summary>
    //public class FakeNancyModuleWithRouteCacheProviderDependency : NancyModule
    //{
    //    public IRouteCacheProvider RouteCacheProvider { get; private set; }

    //    public FakeNancyModuleWithRouteCacheProviderDependency(IRouteCacheProvider routeCacheProvider)
    //    {
    //        RouteCacheProvider = routeCacheProvider;
    //    }
    //}
}