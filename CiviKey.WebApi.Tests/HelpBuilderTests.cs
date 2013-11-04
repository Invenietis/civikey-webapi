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
using NUnit.Framework;

namespace CiviKey.WebApi.Tests
{
    [TestFixture]
    public class HelpBuilderTests
    {
        [Test]
        public void CreateOrUpdateAllBuildsAvailableInSource()
        {
            // Given
            HashProvider hp = new HashProvider();
            IConfiguration config = new TestConfiguration();
            config.Settings.HelpDirectory = "Help";

            HelpBuilderService buildService = new HelpBuilderService( config, hp );

            DirectoryInfo frHelpDir = buildService.SourceDirectory.CreateSubdirectory( Path.Combine( Guid.NewGuid().ToString( "B" ), "1.0.0", "fr" ) );
            frHelpDir.Create();

            DirectoryInfo enHelpDir = frHelpDir.Parent.CreateSubdirectory( "en" );
            enHelpDir.Create();

            File.Create( Path.Combine( frHelpDir.FullName, "helpContent.txt" ) ).Dispose();
            File.Create( Path.Combine( enHelpDir.FullName, "helpContent.txt" ) ).Dispose();

            // When
            buildService.CreateOrUpdateBuilds();

            // Then
            DirectoryInfo pluginBuildDir = buildService.BuildsDirectory.EnumerateDirectories( frHelpDir.Parent.Parent.Name ).FirstOrDefault();
            Assert.That( pluginBuildDir, Is.Not.Null );

            DirectoryInfo versionBuildDir = pluginBuildDir.EnumerateDirectories( frHelpDir.Parent.Name ).FirstOrDefault();
            Assert.That( versionBuildDir, Is.Not.Null );

            DirectoryInfo frBuildDir = versionBuildDir.EnumerateDirectories( "fr" ).FirstOrDefault();
            DirectoryInfo enBuildDir = versionBuildDir.EnumerateDirectories( "en" ).FirstOrDefault();
            Assert.That( frBuildDir, Is.Not.Null );
            Assert.That( enBuildDir, Is.Not.Null );

            Assert.That( frBuildDir.EnumerateFiles().Any( f => f.Name == "hash" ) );
            Assert.That( frBuildDir.EnumerateFiles().Any( f => f.Name == "package.zip" ) );
            Assert.That( enBuildDir.EnumerateFiles().Any( f => f.Name == "hash" ) );
            Assert.That( enBuildDir.EnumerateFiles().Any( f => f.Name == "package.zip" ) );
        }
    }
}
