using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using CiviKey.WebApi.Core;
using CiviKey.WebApi.Core.Configuration;
using CiviKey.WebApi.Web.Configuration;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using System.Linq;
using System;
using System.Reflection;

namespace CiviKey.WebApi.Web
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        // The bootstrapper enables you to reconfigure the composition of the framework,
        // by overriding the various methods and properties.
        // For more information https://github.com/NancyFx/Nancy/wiki/Bootstrapper

        protected override void ConfigureApplicationContainer( TinyIoCContainer container )
        {
            container.Register<IConfiguration, WebConfiguration>().AsSingleton();

            base.ConfigureApplicationContainer( container );
        }

        protected override IEnumerable<System.Func<System.Reflection.Assembly, bool>> AutoRegisterIgnoredAssemblies
        {
            get { return base.AutoRegisterIgnoredAssemblies.Concat( ObtainIgnoredAssemblies ); }
        }

        IEnumerable<Func<Assembly, bool>> ObtainIgnoredAssemblies
        {
            get { yield return ( a ) => a.FullName.Contains( "Tests" ); }
        }
    }
}