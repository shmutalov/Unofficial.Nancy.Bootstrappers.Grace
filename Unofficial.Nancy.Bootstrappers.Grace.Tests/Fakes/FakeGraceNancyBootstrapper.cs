using System;
using Nancy;
using Nancy.Bootstrapper;

namespace Unofficial.Nancy.Bootstrappers.Grace.Tests.Fakes
{
    public class FakeGraceNancyBootstrapper : GraceNancyBootstrapper
    {
        public bool ApplicationContainerConfigured { get; set; }
        public bool RequestContainerConfigured { get; set; }

        private readonly Func<ITypeCatalog, NancyInternalConfiguration> configuration;

        public FakeGraceNancyBootstrapper()
            : this(null)
        {
        }

        public FakeGraceNancyBootstrapper(Func<ITypeCatalog, NancyInternalConfiguration> configuration)
        {
            this.configuration = configuration;
        }

        protected override Func<ITypeCatalog, NancyInternalConfiguration> InternalConfiguration
        {
            get { return configuration ?? base.InternalConfiguration; }

        }

        public IGraceWrapper Container
        {
            get { return ApplicationContainer; }
        }

        protected override void ConfigureApplicationContainer(IGraceWrapper existingContainer)
        {
            ApplicationContainerConfigured = true;
            base.ConfigureApplicationContainer(existingContainer);
        }

        protected override void ConfigureRequestContainer(IGraceWrapper container, NancyContext context)
        {
            container.Configure(registry =>
            {
                registry.Export<Foo>().As<IFoo>();
                registry.Export<Dependency>().As<IDependency>();
            });

            RequestContainerConfigured = true;
            base.ConfigureRequestContainer(container, context);
        }
    }

    public class FakeNancyRequestStartup : IRequestStartup
    {
        public void Initialize(IPipelines pipelines, NancyContext context)
        {
            // Observable side-effect of the execution of this IRequestStartup.
            context.ViewBag.RequestStartupHasRun = true;
        }
    }
}