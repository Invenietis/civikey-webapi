using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;

namespace CiviKey.WebApi.Tests
{
    public class TestRootPathProvider : IRootPathProvider
    {
        public string GetRootPath()
        {
            return "../../TestOutput";
        }
    }
}
