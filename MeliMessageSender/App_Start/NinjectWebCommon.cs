using System.Configuration;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Ninject.Activation;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(MeliMessageSender.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(MeliMessageSender.App_Start.NinjectWebCommon), "Stop")]

namespace MeliMessageSender.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;

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
	        kernel.Bind<CloudQueue>().ToMethod(BindQueue);
        }

	    private static CloudQueue BindQueue(IContext context)
	    {
			var connectionString = ConfigurationManager.ConnectionStrings["MercadolibreStorageConnectionString"].ConnectionString;
			var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
			var queueName = CloudConfigurationManager.GetSetting("QueueName");
			var queueClient = cloudStorageAccount.CreateCloudQueueClient();
			var queue = queueClient.GetQueueReference(queueName);
			queue.CreateIfNotExists();
			return queue;

	    }
    }
}
