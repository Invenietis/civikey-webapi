using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Routing;
using CiviKey.WebApi.Core.Configuration;
using CiviKey.WebApi.Crash;
using CK.Mailer;
using CK.TaskHost;
using CK.TaskHost.Impl;
using CK.TaskHost.Unity;
using CK.Toolkit.Http;
using Microsoft.Practices.Unity;

namespace CiviKey.WebApi
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterInstance<IUnityContainer>( container );
            container.RegisterType<IHttpControllerActivator, UnityHttpControllerActivator>( new ContainerControlledLifetimeManager() );
            container.RegisterType<IConfiguration, WebConfiguration>( new ContainerControlledLifetimeManager() );

            container.RegisterInstance<IMailerService>( new DefaultMailerService() );

            GlobalConfiguration.Configure( ( c ) => WebApiConfig.Register( c, container ) );

            IConfiguration config =  container.Resolve<IConfiguration>();
            DirectoryInfo tasksRepoDirectory = new DirectoryInfo( Path.Combine( config.GetRootPath(), config.Settings.TasksRepository ) );
            if( !tasksRepoDirectory.Exists )
                tasksRepoDirectory.Create();

            ICKTaskFactory taskFactory = new UnityCKTaskFactory( container, true );
            CKHost.Start( new HostMultiFileRepository( tasksRepoDirectory.FullName ), taskFactory );

            CKHost.RegisterUniqueTask( typeof( CrashTask ), "Send mail report of new crash logs" );
        }
    }
}
