using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Nancy;

namespace CiviKey.WebApi.Core.Configuration
{
    public sealed class WebConfiguration : IConfiguration
    {
        DynamicDictionary _settings;

        public WebConfiguration()
        {
            _settings = new DynamicDictionary();
            foreach( string k in ConfigurationManager.AppSettings.AllKeys )
            {
                _settings.Add( k, ConfigurationManager.AppSettings[k] );
            }
        }

        public dynamic Settings
        {
            get { return _settings; }
        }

        public string GetRootPath()
        {
            return HostingEnvironment.MapPath( "~/" );
        }
    }
}