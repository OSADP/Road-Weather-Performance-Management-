[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(RWPMPortal.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(RWPMPortal.App_Start.NinjectWebCommon), "Stop")]

namespace RWPMPortal.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using InfloCommon.Repositories;
    using System.Data.Entity;
    using RoadSegmentMapping;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
#if DEBUG
            //debug binding
#else
            //release binding
#endif

            //ninject will handle calling dispose on things that implement idisposable.

            //This binding means that whenever Ninject encounters a dependency on IUnitOfWork, it will resolve an instance of UnitOfWork and inject it using the connection string as an arg. 
          
           //The inflo connection string is an Entity Framework style connection string (e.g. has "metadata" etc).
            //This type can also just supply the Name param to look it up in the web.config file.
            //So InfloDatabaseConnectionString is the 'name' of a connection string in web.config and it finds it and passes it in.
            kernel.Bind<IUnitOfWork>().To<UnitOfWork>().InRequestScope().WithConstructorArgument("connectionString", "InfloDatabaseConnectionString");

            // The OSM database connection string is the older type.This type does not have the slick 'name' override,
            //so we need to manually grab the name from the web.configuration and then pass it in.
            string osmCS = System.Configuration.ConfigurationManager.ConnectionStrings["OsmMapModelDbConnectionString"].ConnectionString;
            kernel.Bind<OsmMapModel>().To<OsmMapModel>().InRequestScope().WithConstructorArgument("connectionString", osmCS);
        
        }        
    }
}
