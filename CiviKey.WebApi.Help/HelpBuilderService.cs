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
    public class HelpBuilderService
    {
        HashProvider _hashProvider;

        DirectoryInfo _sourceDirectory;
        DirectoryInfo _buildsDirectory;

        public HelpBuilderService( IConfiguration configuration, HashProvider hashProvider )
        {
            _hashProvider = hashProvider;

            _sourceDirectory = new DirectoryInfo( Path.Combine( configuration.GetRootPath(), configuration.Settings.HelpDirectory, "Source" ) );
            _buildsDirectory = new DirectoryInfo( Path.Combine( configuration.GetRootPath(), configuration.Settings.HelpDirectory, "Build" ) );

            if( !_sourceDirectory.Exists ) _sourceDirectory.Create();
            if( !_buildsDirectory.Exists ) _buildsDirectory.Create();
        }

        public DirectoryInfo SourceDirectory { get { return _sourceDirectory; } }
        public DirectoryInfo BuildsDirectory { get { return _buildsDirectory; } }
        public const string PackageFileName = "package.zip";
        public const string HashFileFileName = "hash";

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
                        else
                        {
                            string hash = "0x" + BitConverter.ToString( _hashProvider.ComputeHash( cultureDir ) ).Replace( "-", string.Empty );
                            string existingHash = string.Empty;
                            using( TextReader rdr = File.OpenText( Path.Combine( buildDir.FullName, "hash" ) ) )
                            {
                                existingHash = rdr.ReadLine();
                            }

                            if( string.Compare( hash, existingHash ) != 0 )
                                BuildCulture( cultureDir );
                        }
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
            using( FileStream hFile = File.Create( Path.Combine( buildDirectory.FullName, HashFileFileName ) ) )
            using( StreamWriter sw = new StreamWriter( hFile ) )
            {
                sw.WriteLine( "0x{0}", BitConverter.ToString( hash ).Replace( "-", string.Empty ) );
            }
        }

        void CreateOrUpdateZipFile( DirectoryInfo sourceDirectory, DirectoryInfo buildDirectory )
        {
            ZipFile build = new ZipFile();
            build.AddDirectory( sourceDirectory.FullName, "/content" );
            build.AddEntry( "manifest.xml", GetManifest( buildDirectory ), Encoding.Unicode );

            build.Save( Path.Combine( buildDirectory.FullName, PackageFileName ) );
        }

        string GetManifest( DirectoryInfo cultureDirectory )
        {
            var manifest = new HelpManifestData { PluginId = cultureDirectory.Parent.Parent.Name, Version = cultureDirectory.Parent.Name, Culture = cultureDirectory.Name };
            using( var fr = File.OpenText( Path.Combine( cultureDirectory.FullName, HelpBuilderService.HashFileFileName ) ) )
                manifest.Hash = fr.ReadLine();

            StringBuilder sb = new StringBuilder();
            using( StringWriter w = new StringWriter( sb ) )
                manifest.Serialize( w );

            return sb.ToString();
        }
    }
}
