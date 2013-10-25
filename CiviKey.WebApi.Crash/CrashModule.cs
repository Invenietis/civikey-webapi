using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;

namespace CiviKey.WebApi.Crash
{
    public class CrashModule : NancyModule
    {
        public CrashModule( CrashService crashService )
            : base( "/v2/crash" )
        {
            // Add or update a crash log for the given civikey instance identifier
            Post["/{civiKeyInstanceIdentifier}"] = parameters =>
            {
                if( Request.Files.Any() )
                {
                    foreach( var f in Request.Files )
                    {
                        crashService.RegisterCrash( parameters.civiKeyInstanceIdentifier, f );
                    }
                    return HttpStatusCode.Created;
                }
                else if( Request.Body.Length > 0 )
                {
                    crashService.RegisterCrash( parameters.civiKeyInstanceIdentifier, Request.Body );
                    return HttpStatusCode.Created;
                }

                return HttpStatusCode.NoContent;
            };
        }
    }
}
