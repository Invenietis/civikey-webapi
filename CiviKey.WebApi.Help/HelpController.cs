using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace CiviKey.WebApi.Help
{
    [RoutePrefix( "v2/help" )]
    public class HelpController : ApiController
    {
        HelpService _helpService;

        public HelpController( HelpService helpService )
        {
            _helpService = helpService;
        }

        [Route( "{pluginId}/{version}/{culture}" )]
        public HttpResponseMessage GetHelp( string pluginId, string version, string culture )
        {
            var res = new HttpResponseMessage();

            Version parsedVersion = null;
            if( Version.TryParse( version, out parsedVersion ) )
            {
                var content = _helpService.GetHelpPackage( pluginId, parsedVersion, culture );
                if( content != null )
                {
                    res.StatusCode = System.Net.HttpStatusCode.OK;
                    res.Content = new StreamContent( content );
                    res.Content.Headers.ContentType = new MediaTypeHeaderValue( "application/zip" );
                    return res;
                }
            }

            res.StatusCode = System.Net.HttpStatusCode.NotFound;
            return res;
        }

        [Route( "{pluginId}/{version}/{culture}/{hash}/isupdated" )]
        public IHttpActionResult GetIsHelpUpdated( string pluginId, string version, string culture, string hash )
        {
            Version parsedVersion = null;
            if( Version.TryParse( version, out parsedVersion ) )
            {
                var isUpdated = _helpService.IsHelpUpdated( pluginId, parsedVersion, culture, hash );

                if( isUpdated.HasValue )
                    return Ok( isUpdated.Value );
            }

            return NotFound();
        }
    }
}
