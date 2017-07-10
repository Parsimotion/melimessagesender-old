using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(MeliMessageSender.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(MeliMessageSender.App_Start.NinjectWebCommon), "Stop")]

namespace MeliMessageSender.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using MeliMessageSender.Services;

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
	        BindQueueClient(kernel);
	        BindMercadolibreApi(kernel);
        }

	    private static void BindMercadolibreApi(IKernel kernel)
	    {
		    var mercadolibreUrl = CloudConfigurationManager.GetSetting("MercadolibreUrl") ?? "https://mercadolibre-development.azurewebsites.net";
		    kernel.Bind<MercadolibreApi>().ToConstant(new MercadolibreApi(mercadolibreUrl));
	    }

	    private static void BindQueueClient(IKernel kernel)
	    {
		    kernel.Bind<QueueClient>().ToConstant(
			    QueueClient.CreateFromConnectionString(
				    CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString"),
				    CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.Queue")));
	    }
    }
}
