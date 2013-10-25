using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using CiviKey.WebApi.Core.Configuration;
using Nancy;

namespace CiviKey.WebApi.Web.Configuration
{
    internal sealed class WebConfiguration : IConfiguration
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
    }
}