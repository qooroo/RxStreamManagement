using System.Reflection;
using Autofac;
using Autofac.Integration.SignalR;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;
using RxStreamManagement.Server.DataGenerator;
using RxStreamManagement.Server.Subscription;

namespace RxStreamManagement.Server
{
    public class Bootstrapper
    {
        private IContainer _container;

        public void Run()
        {
            WebApp.Start("http://localhost:8888", app =>
            {
                app.UseWelcomePage("/");
                app.UseCors(CorsOptions.AllowAll);

                var builder = new ContainerBuilder();
                builder.RegisterType<DummyMarginUpdateSource>().As<IMarginUpdateSource>();
                builder.RegisterType<SubscriptionManager>().As<ISubscriptionManager>().SingleInstance();
                builder.RegisterHubs(Assembly.GetExecutingAssembly());

                _container = builder.Build();

                var config = new HubConfiguration
                {
                    Resolver = new AutofacDependencyResolver(_container),
                    EnableDetailedErrors = true
                };

                app.UseAutofacMiddleware(_container);
                app.MapSignalR("/signalr", config);
            });

            _container.Resolve<ISubscriptionManager>().Initialise();
        }
    }
}
