#if !MONO

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Routing;
using Xunit;

namespace Unofficial.Nancy.Bootstrappers.Grace.Tests
{
    /// <summary>
    /// Base class for testing the basic behaviour of a bootstrapper that
    /// implements either of the two bootstrapper base classes.
    /// These tests only test basic external behaviour, they are not exhaustive;
    /// it is expected that additional tests specific to the bootstrapper implementation
    /// are also created.
    /// </summary>
    public abstract class BootstrapperBaseFixtureBase<TContainer>
        where TContainer : class
    {
        protected abstract NancyBootstrapperBase<TContainer> Bootstrapper { get; }

        protected Func<ITypeCatalog, NancyInternalConfiguration> Configuration { get; }

        protected BootstrapperBaseFixtureBase()
        {
            Configuration = NancyInternalConfiguration.WithOverrides(
                builder =>
                {
                    builder.NancyEngine = typeof(FakeEngine);
                });
        }

        [Fact]
        public virtual void Should_throw_if_get_engine_called_without_being_initialised()
        {
            // Given / When
            var result = Record.Exception(() => Bootstrapper.GetEngine());

            Assert.NotNull(result);
        }

        [Fact]
        public virtual void Should_resolve_engine_when_initialised()
        {
            // Given
            Bootstrapper.Initialise();

            // When
            var result = Bootstrapper.GetEngine();

            // Then
            Assert.NotNull(result);
            Assert.IsAssignableFrom<INancyEngine>(result);
        }

        [Fact]
        public virtual void Should_use_types_from_config()
        {
            // Given
            Bootstrapper.Initialise();

            // When
            var result = Bootstrapper.GetEngine();

            // Then
            Assert.IsType<FakeEngine>(result);
        }

        [Fact]
        public virtual void Should_register_config_types_as_singletons()
        {
            // Given
            Bootstrapper.Initialise();

            // When
            var result1 = Bootstrapper.GetEngine();
            var result2 = Bootstrapper.GetEngine();

            // Then
            Assert.Same(result1, result2);
        }

        [Fact]
        public void Should_honour_typeregistration_singleton_lifetimes()
        {
            // Given
            Bootstrapper.Initialise();

            // When
            var result1 = ((TestDependencyModule)Bootstrapper.GetModule(typeof(TestDependencyModule), new NancyContext()));
            var result2 = ((TestDependencyModule)Bootstrapper.GetModule(typeof(TestDependencyModule), new NancyContext()));

            // Then
            Assert.Same(result1.Singleton, result2.Singleton);
        }

        [Fact]
        public void Should_honour_typeregistration_transient_lifetimes()
        {
            // Given
            Bootstrapper.Initialise();

            // When
            var result1 = ((TestDependencyModule)Bootstrapper.GetModule(typeof(TestDependencyModule), new NancyContext()));
            var result2 = ((TestDependencyModule)Bootstrapper.GetModule(typeof(TestDependencyModule), new NancyContext()));

            // Then
            Assert.NotSame(result1.Transient, result2.Transient);
        }

        public class FakeEngine : INancyEngine
        {
            public INancyContextFactory ContextFactory { get; }

            public IRouteCache RouteCache { get; }

            public IRouteResolver Resolver { get; }

            public Func<NancyContext, Response> PreRequestHook { get; set; }

            public Action<NancyContext> PostRequestHook { get; set; }

            public Func<NancyContext, Exception, dynamic> OnErrorHook { get; set; }

            public Func<NancyContext, IPipelines> RequestPipelinesFactory { get; set; }

            public Task<NancyContext> HandleRequest(Request request, Func<NancyContext, NancyContext> preRequest, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

            public FakeEngine(IRouteResolver resolver, IRouteCache routeCache, INancyContextFactory contextFactory)
            {
                Resolver = resolver ?? throw new ArgumentNullException(nameof(resolver), "The resolver parameter cannot be null.");
                RouteCache = routeCache ?? throw new ArgumentNullException(nameof(routeCache), "The routeCache parameter cannot be null.");
                ContextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            }

            public void Dispose()
            { }
        }
    }

    public class TestRegistrations : IRegistrations
    {
        public IEnumerable<TypeRegistration> TypeRegistrations { get; private set; }

        public IEnumerable<CollectionTypeRegistration> CollectionTypeRegistrations { get; private set; }

        public IEnumerable<InstanceRegistration> InstanceRegistrations { get; private set; }

        public TestRegistrations()
        {
            TypeRegistrations = new[]
                                         {
                                             new TypeRegistration(
                                                 typeof(Singleton),
                                                 typeof(Singleton),
                                                 Lifetime.Singleton),
                                             new TypeRegistration(
                                                 typeof(Transient),
                                                 typeof(Transient),
                                                 Lifetime.Transient),
                                         };
        }
    }

    public class Singleton
    {
    }

    public class Transient
    {
    }

    public class TestDependencyModule : NancyModule
    {
        public Singleton Singleton { get; set; }

        public Transient Transient { get; set; }

        public TestDependencyModule(Singleton singleton, Transient transient)
        {
            Singleton = singleton;
            Transient = transient;
        }
    }
}
#endif