#if !__MonoCS__ 

using Nancy.Bootstrapper;
using Unofficial.Nancy.Bootstrappers.Grace.Tests.Fakes;

namespace Unofficial.Nancy.Bootstrappers.Grace.Tests
{
    public class BootstrapperBaseFixture : BootstrapperBaseFixtureBase<IGraceWrapper>
    {
        private readonly GraceNancyBootstrapper bootstrapper;

        public BootstrapperBaseFixture()
        {
            bootstrapper = new FakeGraceNancyBootstrapper(Configuration);
        }

        protected override NancyBootstrapperBase<IGraceWrapper> Bootstrapper
        {
            get { return bootstrapper; }
        }
    }
}

#endif