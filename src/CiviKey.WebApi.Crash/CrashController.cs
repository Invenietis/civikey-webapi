using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using CK.Mailer;

namespace CiviKey.WebApi.Crash
{
    [RoutePrefix( "v2/crash" )]
    public class CrashController : ApiController
    {
        CrashService _crashService;

        public CrashController( CrashService crashService )
        {
            _crashService = crashService;
        }

        [Route( "{civiKeyInstanceIdentifier}" )]
        public async Task<HttpStatusCode> Post( string civiKeyInstanceIdentifier )
        {
            // Add or update a crash log for the given civikey instance identifier
            if( Request.Content.IsMimeMultipartContent() )
            {
                var multipart = await Request.Content.ReadAsMultipartAsync();
                if( multipart.Contents.Any() )
                {
                    foreach( var c in multipart.Contents )
                    {
                        string filename = c.Headers.ContentDisposition.FileName;
                        if( string.IsNullOrEmpty( filename ) ) throw new ArgumentException();

                        _crashService.RegisterCrash( civiKeyInstanceIdentifier, await c.ReadAsStreamAsync(), filename );
                    }
                    return HttpStatusCode.Created;
                }
            }
            else
            {
                _crashService.RegisterCrash( civiKeyInstanceIdentifier, await Request.Content.ReadAsStreamAsync() );
                return HttpStatusCode.OK;
            }

            return HttpStatusCode.NoContent;
        }
    }
}
