using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using CiviKey.WebApi.Core.Configuration;
using CK.Toolkit.Http;
using Microsoft.Practices.Unity;

namespace CiviKey.WebApi
{
    public static class WebApiConfig
    {
        public static void Register( HttpConfiguration config )
        {
            // Web API configuration and services
            IUnityContainer container = new UnityContainer();

            container.RegisterInstance<IUnityContainer>( container );
            container.RegisterType<IHttpControllerActivator, UnityHttpControllerActivator>( new ContainerControlledLifetimeManager() );
            container.RegisterType<IConfiguration, WebConfiguration>( new ContainerControlledLifetimeManager() );
            
            config.DependencyResolver = new ApiUnityDependencyResolver( container );

            // Web API routes
            config.MapHttpAttributeRoutes();
        }
    }
}
