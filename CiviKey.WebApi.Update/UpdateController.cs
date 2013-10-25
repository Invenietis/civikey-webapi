using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.Http;
using Semver;
using System.Net;
using System.Net.Http.Headers;

namespace CiviKey.WebApi.Update
{
    [RoutePrefix( "v2/update" )]
    public class UpdateController : ApiController
    {
        UpdateService _updateService;

        public UpdateController( UpdateService updateService )
        {
            _updateService = updateService;
        }

        [Route( "{distrib}" ), HttpGet]
        public IHttpActionResult GetLastVersion( string distrib, [FromUri] bool includePrerelease = false )
        {
            // Obtain the last version available
            var lastVersion = _updateService.GetLastVersion( distrib, includePrerelease );
            if( lastVersion != null )
            {
                return Ok( lastVersion.ToString() );
            }

            return NotFound();
        }

        [Route( "{distrib}/{version}/installer" ), HttpGet]
        public HttpResponseMessage GetInstaller( string distrib, string version )
        {
            SemVersion semVer = SemVersion.Parse( version );
            FileInfo installer = _updateService.GetInstaller( distrib, semVer );
            if( installer == null )
                return new HttpResponseMessage( HttpStatusCode.NotFound );

            var res = new HttpResponseMessage( HttpStatusCode.OK );
            res.Content = new StreamContent( installer.OpenRead() );
            res.Content.Headers.ContentType = new MediaTypeHeaderValue( "application/octet-stream" );

            return res;
        }

        [Route( "{distrib}/{version}/release-notes" ), HttpGet]
        public HttpResponseMessage GetReleaseNote( string distrib, string version )
        {
            SemVersion semVer = SemVersion.Parse( version );
            FileInfo installer = _updateService.GetReleaseNotes( distrib, semVer );
            if( installer == null )
                return new HttpResponseMessage( HttpStatusCode.NotFound );

            var res = new HttpResponseMessage( HttpStatusCode.OK );
            res.Content = new StreamContent( installer.OpenRead() );
            res.Content.Headers.ContentType = new MediaTypeHeaderValue( "text/markdown" );

            return res;
        }
    }
}
