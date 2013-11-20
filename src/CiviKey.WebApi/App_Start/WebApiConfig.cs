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
        public static void Register( HttpConfiguration config, IUnityContainer container )
        {
            // Web API configuration and services
            config.DependencyResolver = new ApiUnityDependencyResolver( container );

            // Web API routes
            config.MapHttpAttributeRoutes();
        }
    }
}
