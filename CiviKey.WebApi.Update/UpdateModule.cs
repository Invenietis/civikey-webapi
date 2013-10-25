using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Semver;

namespace CiviKey.WebApi.Update
{
    public class UpdateModule : NancyModule
    {
        public UpdateModule( UpdateService updateService )
            : base( "/v2/update" )
        {
            // Obtain the last version available
            Get["/{distrib}"] = parameters =>
            {
                bool includePreRelease = false;
                bool.TryParse( Request.Query.IncludePrerelease, out includePreRelease );

                var lastVersion = updateService.GetLastVersion( parameters.distrib, includePreRelease );
                if( lastVersion != null )
                    return lastVersion.ToString();

                return HttpStatusCode.NotFound;
            };

            // Obtain the installer of the given version
            Get["/{distrib}/{version}/installer"] = parameters =>
            {
                SemVersion version = SemVersion.Parse( parameters.version );
                FileInfo installer = updateService.GetInstaller( parameters.distrib, version );
                if( installer == null )
                    return HttpStatusCode.NotFound;

                return Response.FromStream( installer.OpenRead, "application/zip" );
            };

            // Obtain the release notes of the given version
            Get["/{distrib}/{version}/release-notes"] = parameters =>
            {
                SemVersion version = SemVersion.Parse( parameters.version );
                FileInfo notes = updateService.GetReleaseNotes( parameters.distrib, version );
                if( notes == null )
                    return HttpStatusCode.NotFound;

                return Response.FromStream( notes.OpenRead, "text/markdown" );
            };
        }
    }
}
