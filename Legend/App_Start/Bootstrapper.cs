using System;
using System.Web.Routing;
using Legend.Models;
using Legend.Services;
using Legend.World;
using Ninject;
using Raven.Client;
using Raven.Client.Document;
using SignalR;
using SignalR.Ninject;

[assembly: WebActivator.PostApplicationStartMethod(typeof(Legend.App_Start.Bootstrapper), "PreAppStart")]

namespace Legend.App_Start
{
    public class Bootstrapper
    {
        internal static IKernel Kernel = null;

        public static void PreAppStart()
        {
            var kernel = new StandardKernel();

            //kernel.Bind<IRedisClientsManager>()
            //    .ToMethod(c => new PooledRedisClientManager(5, 600000, ConfigurationManager.AppSettings["RedisHostAddress"]))
            //    .InSingletonScope();

            kernel.Bind<IWorld>()
                .To<World.World>()
                .InSingletonScope();

            kernel.Bind<IWorldService>()
                .To<WorldService>()
                .InRequestScope();

            kernel.Bind<IDocumentStore>()
                .ToMethod(r => InitializeRaven())
                .InSingletonScope();

            kernel.Bind<IDocumentSession>()
                .ToMethod(r =>
                          {
                              var session = r.Kernel.Get<IDocumentStore>().OpenSession();
                              //session.Advanced.UseOptimisticConcurrency = true;
                              return session;
                          }).InRequestScope();

            kernel.Bind<ICache>()
                  .To<AspNetCache>()
                  .InSingletonScope();

            //kernel.Bind<IGameRepository>()
            //    .To<RavenRepository>()
            //    .InRequestScope();

            var resolver = new NinjectDependencyResolver(kernel);

            GlobalHost.DependencyResolver = resolver;
            
            RouteTable.Routes.MapHubs();

            var worldServiceFactory = new Func<IWorldService>(() => kernel.Get<IWorldService>());

            Initialize(worldServiceFactory());
        }

        private static IDocumentStore InitializeRaven()
        {
            //DocumentStore instance = new EmbeddableDocumentStore
            //{
            //    DataDirectory = "Data",
            //    UseEmbeddedHttpServer = true
            //};
            var instance = new DocumentStore { ConnectionStringName = "RavenDB" };

            instance.Initialize();
            return instance;
        }

        private static void Initialize(IWorldService worldService)
        {
            try
            {
                worldService.Initialize();
            }
            catch
            {
                // TODO.  Log
            }
        }
    }
}