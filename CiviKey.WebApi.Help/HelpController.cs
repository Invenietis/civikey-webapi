using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace CiviKey.WebApi.Help
{
    [RoutePrefix( "v2/help" )]
    public class HelpController : ApiController
    {
        [Route( "{pluginId}/{version}/{culture}" )]
        public void GetHelp( string pluginId, Version version, string culture )
        {
            // TODO 
            // find the finest help in the builds
        }

        [Route( "{pluginId}/{version}/{culture}/{hash}/isupdated" )]
        public bool IsHelpUpdated( string pluginId, Version version, string culture, string hash )
        {
            // TODO
            // check weither the given hash is different than the hash in the build directory

            return false;
        }
    }
}
