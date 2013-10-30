using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CiviKey.WebApi.Core.Configuration;
using Ionic.Zip;

namespace CiviKey.WebApi.Help
{
    public class HelpBuilderService : IDisposable
    {
        HashProvider _hashProvider;

        DirectoryInfo _sourceDirectory;
        DirectoryInfo _buildsDirectory;

        FileSystemWatcher _watcher;

        public HelpBuilderService( IConfiguration configuration, HashProvider hashProvider )
        {
            _hashProvider = hashProvider;

            _sourceDirectory = new DirectoryInfo( Path.Combine( configuration.GetRootPath(), configuration.Settings.HelpDirectory, "Source" ) );
            _buildsDirectory = new DirectoryInfo( Path.Combine( configuration.GetRootPath(), configuration.Settings.HelpDirectory, "Build" ) );

            if( !_sourceDirectory.Exists ) _sourceDirectory.Create();
            if( !_buildsDirectory.Exists ) _buildsDirectory.Create();

            _watcher = new FileSystemWatcher( _sourceDirectory.FullName );

            _watcher.Changed += OnFSChanged;
            _watcher.Created += OnFSChanged;
            _watcher.Deleted += OnFSChanged;
            _watcher.Renamed += OnFSChanged;

            _watcher.IncludeSubdirectories = true;
            _watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;

            _watcher.EnableRaisingEvents = true;
        }

        public DirectoryInfo SourceDirectory { get { return _sourceDirectory; } }
        public DirectoryInfo BuildsDirectory { get { return _buildsDirectory; } }

        void OnFSChanged( object sender, FileSystemEventArgs e )
        {
            // TODO
            // find the level of the file changed
            // find all impacted directories
            // find the right(s) cultures to build
            // BuildCulture( culturedDirectory )
        }

        /// <summary>
        /// Create all builds for the cultured helps in the source directory if they don't exists yet.
        /// </summary>
        public void CreateOrUpdateBuilds()
        {
            foreach( var pluginDir in _sourceDirectory.EnumerateDirectories() )
            {
                foreach( var versionDir in pluginDir.EnumerateDirectories() )
                {
                    foreach( var cultureDir in versionDir.EnumerateDirectories() )
                    {
                        DirectoryInfo buildDir = GetBuildDirectoryBySourceDirectory( cultureDir );
                        if( !buildDir.Exists || !buildDir.EnumerateFiles( "package.zip" ).Any() || !buildDir.EnumerateFiles( "hash" ).Any() )
                            BuildCulture( cultureDir );
                    }
                }
            }
        }

        void BuildCulture( DirectoryInfo sourceDirectory )
        {
            DirectoryInfo buildDirectory = GetBuildDirectoryBySourceDirectory( sourceDirectory );

            CreateOrUpdateHashFile( sourceDirectory, buildDirectory );
            CreateOrUpdateZipFile( sourceDirectory, buildDirectory );
        }

        DirectoryInfo GetBuildDirectoryBySourceDirectory( DirectoryInfo sourceDirectory )
        {
            DirectoryInfo buildDir = _buildsDirectory.CreateSubdirectory
            (
                Path.Combine
                (
                    sourceDirectory.Parent.Parent.Name, // plugin unique id
                    sourceDirectory.Parent.Name, // version
                    sourceDirectory.Name // culture
                )
            );
            if( !buildDir.Exists ) buildDir.Create();
            buildDir.Refresh();

            return buildDir;
        }

        void CreateOrUpdateHashFile( DirectoryInfo sourceDirectory, DirectoryInfo buildDirectory )
        {
            byte[] hash = _hashProvider.ComputeHash( sourceDirectory );
            using( FileStream hFile = File.Create( Path.Combine( buildDirectory.FullName, "hash" ) ) )
            using( StreamWriter sw = new StreamWriter( hFile ) )
            {
                sw.WriteLine( "0x{0}", BitConverter.ToString( hash ).Replace( "-", string.Empty ) );
            }
        }

        void CreateOrUpdateZipFile( DirectoryInfo sourceDirectory, DirectoryInfo buildDirectory )
        {
            ZipFile build = new ZipFile();
            build.AddDirectory( sourceDirectory.FullName, "/" );

            build.Save( Path.Combine( buildDirectory.FullName, "package.zip" ) );
        }

        public void Dispose()
        {
            _watcher.Changed -= OnFSChanged;
            _watcher.Created -= OnFSChanged;
            _watcher.Deleted -= OnFSChanged;
            _watcher.Renamed -= OnFSChanged;
            _watcher.Dispose();
        }
    }
}
