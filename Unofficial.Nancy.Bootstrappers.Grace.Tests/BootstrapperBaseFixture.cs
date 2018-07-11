#if !__MonoCS__ 

using Grace.DependencyInjection;
using Nancy.Bootstrapper;
using Unofficial.Nancy.Bootstrappers.Grace.Tests.Fakes;

namespace Unofficial.Nancy.Bootstrappers.Grace.Tests
{
    public class BootstrapperBaseFixture : BootstrapperBaseFixtureBase<IInjectionScope>
    {
        private readonly GraceNancyBootstrapper bootstrapper;

        public BootstrapperBaseFixture()
        {
            bootstrapper = new FakeGraceNancyBootstrapper(Configuration);
        }

        protected override NancyBootstrapperBase<IInjectionScope> Bootstrapper
        {
            get { return bootstrapper; }
        }
    }
}

#endif