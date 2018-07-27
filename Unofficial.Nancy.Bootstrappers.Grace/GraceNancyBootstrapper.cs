using Grace.DependencyInjection;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Configuration;
using Nancy.Diagnostics;
using Nancy.ViewEngines;
using System;
using System.Collections.Generic;
using System.Linq;
using Unofficial.Nancy.Bootstrappers.Grace.Extensions;

namespace Unofficial.Nancy.Bootstrappers.Grace
{
    /// <summary>
    /// Nancy bootstrapper for the Grace container.
    /// </summary>
    public abstract class GraceNancyBootstrapper : NancyBootstrapperWithRequestContainerBase<IGraceWrapper>
    {
        /// <summary>
        /// Gets the application level container
        /// </summary>
        /// <returns>Container instance</returns>
        protected override IGraceWrapper GetApplicationContainer()
        {
            return new GraceWrapper(new DependencyInjectionContainer());
        }

        /// <summary>
        /// Creates a per request child/nested container
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>Request container instance</returns>
        protected override IGraceWrapper CreateRequestContainer(NancyContext context)
        {
            return ApplicationContainer.CreateScope();
        }

        /// <summary>
        /// Gets the diagnostics for initialisation
        /// </summary>
        /// <returns>An <see cref="IDiagnostics"/> implementation</returns>
        protected override IDiagnostics GetDiagnostics()
        {
            return ApplicationContainer.Locate<IDiagnostics>();
        }

        /// <summary>
        /// Gets all registered application startup tasks
        /// </summary>
        /// <returns>An <see cref="System.Collections.Generic.IEnumerable{T}"/> instance containing <see cref="IApplicationStartup"/> instances. </returns>
        protected override IEnumerable<IApplicationStartup> GetApplicationStartupTasks()
        {
            return ApplicationContainer.LocateAll<IApplicationStartup>();
        }

        /// <summary>
        /// Gets all registered application registration tasks
        /// </summary>
        /// <returns>An <see cref="System.Collections.Generic.IEnumerable{T}"/> instance containing <see cref="IRegistrations"/> instances.</returns>
        protected override IEnumerable<IRegistrations> GetRegistrationTasks()
        {
            return ApplicationContainer.LocateAll<IRegistrations>();
        }

        /// <summary>
        /// Resolve <see cref="INancyEngine"/>
        /// </summary>
        /// <returns><see cref="INancyEngine"/> implementation</returns>
        protected override INancyEngine GetEngineInternal()
        {
            return ApplicationContainer.Locate<INancyEngine>();
        }

        /// <summary>
        /// Gets the <see cref="INancyEnvironmentConfigurator"/> used by th.
        /// </summary>
        /// <returns>An <see cref="INancyEnvironmentConfigurator"/> instance.</returns>
        protected override INancyEnvironmentConfigurator GetEnvironmentConfigurator()
        {
            return ApplicationContainer.Locate<INancyEnvironmentConfigurator>();
        }

        /// <summary>
        /// Get the <see cref="INancyEnvironment" /> instance.
        /// </summary>
        /// <returns>An configured <see cref="INancyEnvironment" /> instance.</returns>
        /// <remarks>The boostrapper must be initialised (<see cref="INancyBootstrapper.Initialise" />) prior to calling this.</remarks>
        public override INancyEnvironment GetEnvironment()
        {
            return ApplicationContainer.Locate<INancyEnvironment>();
        }

        /// <summary>
        /// Retrieve all module instances from the container
        /// </summary>
        /// <param name="container">Container to use</param>
        /// <returns>Collection of <see cref="INancyModule"/> instances</returns>
        protected override IEnumerable<INancyModule> GetAllModules(IGraceWrapper container)
        {
            return container.LocateAll<INancyModule>();
        }

        /// <summary>
        /// Retreive a specific module instance from the container
        /// </summary>
        /// <param name="container">Container to use</param>
        /// <param name="moduleType">Type of the module</param>
        /// <returns>A <see cref="INancyModule"/> instance</returns>
        protected override INancyModule GetModule(IGraceWrapper container, Type moduleType)
        {
            // container.Configure(registry => registry.Export(moduleType));

            return (INancyModule)container.Locate(moduleType);
        }

        /// <summary>
        /// Gets all registered request startup tasks
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="IRequestStartup"/> instances.</returns>
        protected override IEnumerable<IRequestStartup> RegisterAndGetRequestStartupTasks(
            IGraceWrapper container,
            Type[] requestStartupTypes)
        {
            return requestStartupTypes
                .Select(container.Locate)
                .Cast<IRequestStartup>();
        }

        /// <summary>
        /// Registers an <see cref="INancyEnvironment"/> instance in the container.
        /// </summary>
        /// <param name="container">The container to register into.</param>
        /// <param name="environment">The <see cref="INancyEnvironment"/> instance to register.</param>
        protected override void RegisterNancyEnvironment(
            IGraceWrapper container, 
            INancyEnvironment environment)
        {
            container.Configure(registry => registry.ExportInstance(environment));
        }

        /// <summary>
        /// Register the bootstrapper's implemented types into the container.
        /// This is necessary so a user can pass in a populated container but not have
        /// to take the responsibility of registering things like <see cref="INancyModuleCatalog"/> manually.
        /// </summary>
        /// <param name="applicationContainer">Application container to register into</param>
        protected override void RegisterBootstrapperTypes(IGraceWrapper applicationContainer)
        {
            applicationContainer.Configure(registry =>
            {
                registry.ExportInstance(this).As<INancyModuleCatalog>();
                registry.Export<DefaultFileSystemReader>().As<IFileSystemReader>().Lifestyle.Singleton();
            });
        }

        /// <summary>
        /// Register the default implementations of internally used types into the container as singletons
        /// </summary>
        /// <param name="container">Container to register into</param>
        /// <param name="typeRegistrations">Type registrations to register</param>
        protected override void RegisterTypes(
            IGraceWrapper container, 
            IEnumerable<TypeRegistration> typeRegistrations)
        {
            container.Configure(registry =>
            {
                foreach (var typeRegistration in typeRegistrations)
                {
                    RegisterType(
                        typeRegistration.RegistrationType,
                        typeRegistration.ImplementationType,
                        // container.IsChild() ? Lifetime.PerRequest : typeRegistration.Lifetime,
                        typeRegistration.Lifetime,
                        registry);
                }
            });
        }

        /// <summary>
        /// Register the various collections into the container as singletons to later be resolved
        /// by IEnumerable{Type} constructor dependencies.
        /// </summary>
        /// <param name="container">Container to register into</param>
        /// <param name="collectionTypeRegistrationsn">Collection type registrations to register</param>
        protected override void RegisterCollectionTypes(
            IGraceWrapper container,
            IEnumerable<CollectionTypeRegistration> collectionTypeRegistrationsn)
        {
            container.Configure(registry =>
            {
                foreach (var collectionTypeRegistration in collectionTypeRegistrationsn)
                {
                    foreach (var implementationType in collectionTypeRegistration.ImplementationTypes)
                    {
                        RegisterType(
                            collectionTypeRegistration.RegistrationType,
                            implementationType,
                            //container.IsChild() ? Lifetime.PerRequest : collectionTypeRegistration.Lifetime,
                            collectionTypeRegistration.Lifetime,
                            registry);
                    }
                }
            });
        }

        /// <summary>
        /// Register the given instances into the container
        /// </summary>
        /// <param name="container">Container to register into</param>
        /// <param name="instanceRegistrations">Instance registration types</param>
        protected override void RegisterInstances(
            IGraceWrapper container, 
            IEnumerable<InstanceRegistration> instanceRegistrations)
        {
            container.Configure(registry =>
            {
                foreach (var instanceRegistration in instanceRegistrations)
                {
                    registry.ExportInstance(instanceRegistration.Implementation)
                        .As(instanceRegistration.RegistrationType)
                        .Lifestyle.Singleton();
                }
            });
        }

        /// <summary>
        /// Register the given module types into the request container
        /// </summary>
        /// <param name="container">Container to register into</param>
        /// <param name="moduleRegistrationTypes"><see cref="INancyModule"/> types</param>
        protected override void RegisterRequestContainerModules(
            IGraceWrapper container,
            IEnumerable<ModuleRegistration> moduleRegistrationTypes)
        {
            container.Configure(registry =>
            {
                foreach (var registrationType in moduleRegistrationTypes)
                {
                    registry.Export(registrationType.ModuleType).As(typeof(INancyModule));
                }
            });
        }

        private static void RegisterType(
            Type registrationType, 
            Type implementationType, 
            Lifetime lifetime, 
            IExportRegistrationBlock registry)
        {
            switch (lifetime)
            {
                case Lifetime.Transient:
                    registry.Export(implementationType)
                        .As(registrationType);
                    break;
                case Lifetime.Singleton:
                    registry.Export(implementationType)
                        .As(registrationType)
                        .Lifestyle.Singleton();
                    break;
                case Lifetime.PerRequest:
                    registry.Export(implementationType)
                        .As(registrationType)
                        .Lifestyle.SingletonPerRequest();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("lifetime", lifetime, $"Unknown Lifetime: {lifetime}.");
            }
        }
    }
}
