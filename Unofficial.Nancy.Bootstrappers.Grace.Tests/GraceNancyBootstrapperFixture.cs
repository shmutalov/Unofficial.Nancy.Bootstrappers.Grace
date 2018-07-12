using System;
using System.Linq;
using Nancy;
using Nancy.Routing;
using Xunit;
using Unofficial.Nancy.Bootstrappers.Grace.Tests.Fakes;

namespace Unofficial.Nancy.Bootstrappers.Grace.Tests
{
    public class GraceNancyBootstrapperFixture : IDisposable
    {
        private readonly FakeGraceNancyBootstrapper bootstrapper;

        public GraceNancyBootstrapperFixture()
        {
            bootstrapper = new FakeGraceNancyBootstrapper();
            bootstrapper.Initialise();
        }

        [Fact]
        public void Should_be_able_to_resolve_engine()
        {
            // Given
            // When
            var result = bootstrapper.GetEngine();

            // Then
            Assert.NotNull(result);
            Assert.IsAssignableFrom<INancyEngine>(result);
        }

        [Fact]
        public void GetAllModules_Returns_As_MultiInstance()
        {
            // Given
            bootstrapper.GetEngine();
            var context = new NancyContext();

            // When
            var output1 = bootstrapper.GetAllModules(context).FirstOrDefault(nm => nm.GetType() == typeof(FakeNancyModuleWithBasePath));
            var output2 = bootstrapper.GetAllModules(context).FirstOrDefault(nm => nm.GetType() == typeof(FakeNancyModuleWithBasePath));

            // Then
            Assert.NotNull(output1);
            Assert.NotNull(output2);
            Assert.NotSame(output1, output2);
        }

        [Fact]
        public void GetAllModules_Configures_Child_Container()
        {
            // Given
            bootstrapper.GetEngine();
            bootstrapper.RequestContainerConfigured = false;

            // When
            bootstrapper.GetAllModules(new NancyContext());

            // Then
            Assert.True(bootstrapper.RequestContainerConfigured);
        }

        [Fact]
        public void GetModule_Configures_Child_Container()
        {
            // Given
            bootstrapper.GetEngine();
            bootstrapper.RequestContainerConfigured = false;

            // When
            bootstrapper.GetModule(typeof(FakeNancyModuleWithBasePath), new NancyContext());

            // Then
            Assert.True(bootstrapper.RequestContainerConfigured);
        }

        [Fact]
        public void GetEngine_ConfigureApplicationContainer_Should_Be_Called()
        {
            // Given
            // When
            bootstrapper.GetEngine();

            // Then
            Assert.True(bootstrapper.ApplicationContainerConfigured);
        }

        [Fact]
        public void Should_not_return_the_same_instance_when_getmodule_is_called_multiple_times_with_different_context()
        {
            // Given
            bootstrapper.GetEngine();
            var context1 = new NancyContext();
            var context2 = new NancyContext();

            // When
            var result = bootstrapper.GetModule(typeof(FakeNancyModuleWithDependency), context1) as FakeNancyModuleWithDependency;
            var result2 = bootstrapper.GetModule(typeof(FakeNancyModuleWithDependency), context2) as FakeNancyModuleWithDependency;

            // Then
            Assert.NotNull(result.FooDependency);
            Assert.NotNull(result2.FooDependency);
            Assert.NotSame(result.FooDependency, result2.FooDependency);
        }

        //[Fact]
        //public void Should_resolve_module_with_dependency_on_RouteCacheFactory()
        //{
        //    // Given
        //    bootstrapper.GetEngine();
        //    var context = new NancyContext();

        //    // When
        //    var result = bootstrapper.GetModule(typeof(FakeNancyModuleWithRouteCacheProviderDependency), context) as FakeNancyModuleWithRouteCacheProviderDependency;

        //    // Then
        //    Assert.NotNull(result.RouteCacheProvider);
        //    Assert.IsType<DefaultRouteCacheProvider>(result.RouteCacheProvider);
        //}

        [Fact]
        public void Should_resolve_IRequestStartup_types()
        {
            // Given
            var nancyEngine = bootstrapper.GetEngine();
            var context = new NancyContext();

            // When
            nancyEngine.RequestPipelinesFactory(context);

            // Then
            Assert.True(((bool)context.ViewBag.RequestStartupHasRun));
        }

        public void Dispose()
        {
            bootstrapper.Dispose();
        }
    }
}