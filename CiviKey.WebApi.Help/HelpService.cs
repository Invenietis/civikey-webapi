using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CiviKey.WebApi.Core.Configuration;
using Ionic.Zip;

namespace CiviKey.WebApi.Help
{
    public class HelpService
    {
        const string DefaultCulture = "en";
        DirectoryInfo _buildsDirectory;

        public HelpService( HelpBuilderService helpBuilderService )
        {
            _buildsDirectory = helpBuilderService.BuildsDirectory;
        }

        public Stream GetHelpPackage( string pluginId, Version version, string culture )
        {
            var cultureDirectory = GetCultureDirectory( pluginId, version, culture );
            if( cultureDirectory != null )
                return File.Open( Path.Combine( cultureDirectory.FullName, HelpBuilderService.PackageFileName ), FileMode.Open );

            return null;
        }

        public bool? IsHelpUpdated( string pluginId, Version version, string culture, string hash )
        {
            var cultureDir = GetCultureDirectory( pluginId, version, culture );
            if( cultureDir != null )
            {
                using( var sr = File.OpenText(Path.Combine( cultureDir.FullName, HelpBuilderService.HashFileFileName)))
                {
                    return string.Compare( sr.ReadLine(), hash ) != 0;
                }
            }

            return null;
        }

        DirectoryInfo GetCultureDirectory( string pluginId, Version version, string culture )
        {
            var pluginDir = new DirectoryInfo( Path.Combine( _buildsDirectory.FullName, pluginId ) );
            if( pluginDir.Exists )
            {
                var versionDirs = new Stack<DirectoryInfo>( GetVersionsDirectories( pluginDir, version ).OrderBy( dir => dir.Name ) );
                DirectoryInfo cultureDir = null;
                while( cultureDir == null && versionDirs.Count > 0 )
                {
                    cultureDir = GetCultureDirectory( versionDirs.Pop(), culture );
                }

                return cultureDir;
            }

            return null;
        }

        IEnumerable<DirectoryInfo> GetVersionsDirectories( DirectoryInfo pluginDirectory, Version maxVersion )
        {
            foreach( var d in pluginDirectory.EnumerateDirectories() )
            {
                if( Version.Parse( d.Name ) <= maxVersion )
                    yield return d;
            }
        }

        DirectoryInfo GetCultureDirectory( DirectoryInfo versionDirectory, string culture )
        {
            string culturedDirectoryPath = Path.Combine( versionDirectory.FullName, culture );
            if( Directory.Exists( culturedDirectoryPath ) )
                return new DirectoryInfo( culturedDirectoryPath );
            else if( culture.Contains( '-' ) )
                return GetCultureDirectory( versionDirectory, culture.Split( '-' )[0] );
            else if( culture != DefaultCulture )
                return GetCultureDirectory( versionDirectory, DefaultCulture );
            else
                return null;
        }
    }
}
