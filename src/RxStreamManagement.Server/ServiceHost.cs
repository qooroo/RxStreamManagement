using System;
using System.Web.Http;
using Microsoft.Owin.Hosting;
using Owin;

namespace RxStreamManagement.Server
{
    public class ServiceHost : IDisposable
    {
        private IDisposable _webApp;

        public void Start()
        {
            _webApp = WebApp.Start("localhost:8888", app =>
            {
                app.UseWelcomePage("/");

                var config = new HttpConfiguration
                {
                    IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always
                };
                config.MapHttpAttributeRoutes();
                app.UseWebApi(config);
            });
        }

        public void Dispose()
        {
            if (_webApp != null)
            {
                _webApp.Dispose();
            }
        }
    }
}
