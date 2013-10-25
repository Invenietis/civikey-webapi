using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CiviKey.WebApi.Core.Configuration;

namespace CiviKey.WebApi.Tests
{
    public class TestConfiguration : IConfiguration
    {
        ExpandoObject _settings;

        public dynamic Settings
        {
            get { return _settings ?? (_settings = new ExpandoObject()); }
        }
    }
}
