using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CiviKey.WebApi.Core.Configuration;
using Nancy;
using Semver;

namespace CiviKey.WebApi.Update
{
    public class UpdateService
    {
        DirectoryInfo _installersDirectory;

        public UpdateService( IConfiguration config, IRootPathProvider rootPathProvider )
        {
            _installersDirectory = new DirectoryInfo( Path.Combine( rootPathProvider.GetRootPath(), config.Settings.UpdatesDirectory ) );

            if( !_installersDirectory.Exists ) _installersDirectory.Create();
        }

        public SemVersion GetLastVersion( string distrib, bool includePrerelease )
        {
            DirectoryInfo distribDirectory = _installersDirectory.EnumerateDirectories( distrib ).FirstOrDefault();
            SemVersion lastVersion = null;

            if( distribDirectory != null )
            {
                foreach( var f in distribDirectory.EnumerateFiles().Where( x => x.Extension == ".exe" ) )
                {
                    SemVersion v = SemVersion.Parse( f.Name.Replace( f.Extension, string.Empty ) );
                    if( (string.IsNullOrEmpty( v.Prerelease ) || includePrerelease) && (lastVersion == null || v > lastVersion) )
                        lastVersion = v;
                }
            }

            return lastVersion;
        }

        public FileInfo GetInstaller( string distrib, SemVersion version )
        {
            return GetVersionedFile( distrib, version, ( f ) => f.Extension == ".exe" );
        }

        public FileInfo GetReleaseNotes( string distrib, SemVersion version )
        {
            return GetVersionedFile( distrib, version, ( f ) => f.Extension == ".md" );
        }


        FileInfo GetVersionedFile( string distrib, SemVersion version, Func<FileInfo, bool> filterFiles = null )
        {
            DirectoryInfo distribDirectory = _installersDirectory.EnumerateDirectories( distrib ).FirstOrDefault();
            if( distribDirectory != null )
            {
                IEnumerable<FileInfo> files = distribDirectory.EnumerateFiles();
                if( filterFiles != null )
                    files = files.Where( filterFiles );

                foreach( var f in files )
                {
                    SemVersion v = SemVersion.Parse( f.Name.Replace( f.Extension, string.Empty ) );
                    if( v == version ) return f;
                }
            }

            return null;
        }
    }
}
