using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;
using CiviKey.WebApi.Core.Configuration;
using CiviKey.WebApi.Crash;
using CiviKey.WebApi.Help;
using Ionic.Zip;
using NUnit.Framework;

namespace CiviKey.WebApi.Tests
{
    [TestFixture]
    public class HelpServiceTests
    {
        [Test]
        public void ObtainSpecificVersionWithoutFallback()
        {
            // Given
            HashProvider hp = new HashProvider();
            IConfiguration config = new TestConfiguration();
            config.Settings.HelpDirectory = "Help";
            HelpBuilderService buildService = new HelpBuilderService( config, hp );
            HelpService helpService = new HelpService( buildService );

            string myFakePluginId = Guid.NewGuid().ToString( "B" );
            CreateFakeHelp( buildService, myFakePluginId, new Version[] { new Version( "1.0.0" ), new Version( "2.0.0" ) }, new string[] { "fr-FR", "fr", "en" } );
            buildService.CreateOrUpdateBuilds();

            // When
            using( Stream helpPackage = helpService.GetHelpPackage( myFakePluginId, new Version( "1.0.0" ), "fr-FR" ) )
            {
                // Then
                Assert.That( helpPackage, Is.Not.Null );
                Assert.That( helpPackage.CanRead, Is.True );
                
                CheckHelpPackage( helpPackage, ( m ) =>
                {
                    Assert.That( m.Culture, Is.EqualTo( "fr-FR" ) );
                    Assert.That( m.PluginId, Is.EqualTo( myFakePluginId ) );
                    Assert.That( m.Version, Is.EqualTo( "1.0.0" ) );
                } );
            }
        }

        [Test]
        public void ObtainSpecificVersionWithFallback()
        {
            // Given
            HashProvider hp = new HashProvider();
            IConfiguration config = new TestConfiguration();
            config.Settings.HelpDirectory = "Help";
            HelpBuilderService buildService = new HelpBuilderService( config, hp );
            HelpService helpService = new HelpService( buildService );

            string myFakePluginId = Guid.NewGuid().ToString( "B" );
            CreateFakeHelp( buildService, myFakePluginId, new Version[] { new Version( "1.0.0" ), new Version( "2.0.0" ) }, new string[] { "fr", "en" } );
            buildService.CreateOrUpdateBuilds();

            // When
            using( Stream helpPackage = helpService.GetHelpPackage( myFakePluginId, new Version( "3.0.0" ), "fr-FR" ) )
            {
                // Then
                Assert.That( helpPackage, Is.Not.Null );
                Assert.That( helpPackage.CanRead, Is.True );

                CheckHelpPackage( helpPackage, ( m ) =>
                {
                    Assert.That( m.Culture, Is.EqualTo( "fr" ) );
                    Assert.That( m.PluginId, Is.EqualTo( myFakePluginId ) );
                    Assert.That( m.Version, Is.EqualTo( "2.0.0" ) );
                } );
            }

            // When
            using( Stream helpPackage = helpService.GetHelpPackage( myFakePluginId, new Version( "3.0.0" ), "pl-PL" ) )
            {
                // Then
                Assert.That( helpPackage, Is.Not.Null );
                Assert.That( helpPackage.CanRead, Is.True );

                CheckHelpPackage( helpPackage, ( m ) =>
                {
                    Assert.That( m.Culture, Is.EqualTo( "en" ) );
                    Assert.That( m.PluginId, Is.EqualTo( myFakePluginId ) );
                    Assert.That( m.Version, Is.EqualTo( "2.0.0" ) );
                } );
            }
        }

        [Test]
        public void ObtainSpecificVersionThatDoesNotExists()
        {
            // Given
            HashProvider hp = new HashProvider();
            IConfiguration config = new TestConfiguration();
            config.Settings.HelpDirectory = "Help";
            HelpBuilderService buildService = new HelpBuilderService( config, hp );
            HelpService helpService = new HelpService( buildService );

            string myFakePluginId = Guid.NewGuid().ToString( "B" );
            CreateFakeHelp( buildService, myFakePluginId, new Version[] { new Version( "1.0.0" ) }, new string[] { "fr" } );
            buildService.CreateOrUpdateBuilds();

            // When
            using( Stream helpPackage = helpService.GetHelpPackage( myFakePluginId, new Version( "3.0.0" ), "en-US" ) )
            {
                // Then
                Assert.That( helpPackage, Is.Null );
            }
        }

        void CreateFakeHelp( HelpBuilderService helpBuilderService, string pluginId, IEnumerable<Version> versions, IEnumerable<string> cultures, Stream helpContent = null )
        {
            var pluginDir = helpBuilderService.SourceDirectory.CreateSubdirectory( pluginId );
            if( pluginDir.Exists ) pluginDir.Delete( true );
            pluginDir.Create();

            foreach( var v in versions )
            {
                var versionDir = pluginDir.CreateSubdirectory( v.ToString() );
                versionDir.Create();
                foreach( var c in cultures )
                {
                    var cultureDir = versionDir.CreateSubdirectory( c );
                    cultureDir.Create();

                    using( Stream s = File.Create( Path.Combine( cultureDir.FullName, "file.txt" ) ) )
                    {
                        if( helpContent != null ) helpContent.CopyTo( s );
                        else
                        {
                            using( var sw = new StreamWriter( s ) )
                            {
                                sw.WriteLine( Path.GetTempFileName() );
                            }
                        }
                    }
                }
            }
        }

        void CheckHelpPackage( Stream helpPackageStream, Action<HelpManifestData> manifestChecker )
        {
            // Then
            Assert.That( helpPackageStream, Is.Not.Null );
            Assert.That( helpPackageStream.CanRead, Is.True );

            using( ZipFile z = ZipFile.Read( helpPackageStream ) )
            {
                var manifestEntry = z.Entries.Where( e => e.FileName == "manifest.xml" ).FirstOrDefault();
                var fakeContentFile = z.Entries.Where( e => e.FileName == "content/file.txt" ).FirstOrDefault();

                Assert.That( manifestEntry, Is.Not.Null );
                Assert.That( fakeContentFile, Is.Not.Null );

                using( var manifestInputStream = new MemoryStream() )
                {
                    manifestEntry.Extract( manifestInputStream );
                    manifestInputStream.Seek( 0, SeekOrigin.Begin );

                    HelpManifestData manifest = HelpManifestData.Deserialize( manifestInputStream );
                    manifestChecker( manifest );
                }
            }
        }
    }
}
