using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CiviKey.WebApi.Core;
using CiviKey.WebApi.Core.Configuration;

namespace CiviKey.WebApi.Crash
{
    public class CrashService
    {
        DirectoryInfo _crashDirectory;

        public CrashService( IConfiguration config )
        {
            _crashDirectory = new DirectoryInfo( Path.Combine( config.GetRootPath(), config.Settings.CrashStorageDirectory ) );
            if( !_crashDirectory.Exists ) _crashDirectory.Create();
        }

        public FileInfo RegisterCrash( string applicationId, Stream crashLogContent )
        {
            DirectoryInfo todayDirectory = _crashDirectory
                                                .CreateSubdirectory( applicationId )
                                                .CreateSubdirectory( DateTime.Today.ToString( "yyyy-MM-dd" ) );

            return WriteStream( Path.Combine( todayDirectory.FullName, DateTime.UtcNow.ToString( "u" ).Replace( ":", "-" ) + ".log" ), crashLogContent );
        }

        public FileInfo RegisterCrash( string applicationId, Stream crashLogContent, string filename )
        {
            filename = filename.Replace( "\"", string.Empty );

            string parsableDate = Regex.Replace( filename, "[a-z]|[A-Z]|-|\\.|\"", string.Empty );
            DateTime date = DateTime.ParseExact( parsableDate, "yyyyMMdd HHmmss", null );

            DirectoryInfo crashLogDirectory = _crashDirectory
                                                .CreateSubdirectory( date.ToString( "yyyy-MM-dd" ) )
                                                .CreateSubdirectory( applicationId );

            return WriteStream( Path.Combine( crashLogDirectory.FullName, filename ), crashLogContent );
        }

        public IEnumerable<FileInfo> GetCrashsSince( DateTime date )
        {
            foreach( var dateCrashDir in _crashDirectory.EnumerateDirectories() )
            {
                DateTime crashDate = DateTime.ParseExact( dateCrashDir.Name, "yyyy-MM-dd", null );
                if( crashDate > date )
                {
                    foreach( var appCrashDir in dateCrashDir.EnumerateDirectories() )
                    {
                        foreach( var crash in appCrashDir.EnumerateFiles() )
                        {
                            yield return crash;
                        }
                    }
                }
            }
        }

        FileInfo WriteStream( string path, Stream content )
        {
            FileInfo file = new FileInfo( path );
            if( file.Exists ) file.Delete();
            using( var fs = file.OpenWrite() )
            {
                content.CopyTo( fs );
            }

            file.Refresh();
            return file;
        }
    }
}
