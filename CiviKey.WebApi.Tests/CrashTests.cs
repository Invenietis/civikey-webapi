using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CiviKey.WebApi.Crash;
using Nancy;
using NUnit.Framework;

namespace CiviKey.WebApi.Tests
{
    [TestFixture]
    public class CrashTests
    {
        [Test]
        public void CreateCrashWithRequestContent()
        {
            // Given
            TestConfiguration config = new TestConfiguration();
            config.Settings.CrashStorageDirectory = "CrashDirectory";

            CrashService svc = new CrashService( config, new TestRootPathProvider() );

            // When
            FileInfo crashFile = null;
            MemoryStream ms = new MemoryStream();
            using( TextWriter tx = new StreamWriter( ms ) )
            {
                for( int i = 0; i < 100; i++ )
                    tx.WriteLine( "line {0}", i );

                crashFile = svc.RegisterCrash( "unittests", ms );
            }

            // Then
            Assert.That( crashFile, Is.Not.Null );
            Assert.That( crashFile.Exists, Is.True );
        }

        [Test]
        public void CreateCrashWithFile()
        {
            // Given
            TestConfiguration config = new TestConfiguration();
            config.Settings.CrashStorageDirectory = "CrashDirectory";

            CrashService svc = new CrashService( config, new TestRootPathProvider() );

            // When
            FileInfo crashFile = null;
            MemoryStream ms = new MemoryStream();
            using( TextWriter tx = new StreamWriter( ms ) )
            {
                for( int i = 0; i < 100; i++ )
                    tx.WriteLine( "line {0}", i );

                HttpFile file = new HttpFile( "octet/stream", "crashLog-2013-10-24 10-17-42Z.log", ms, "file" );

                crashFile = svc.RegisterCrash( "unittests", file );
            }

            // Then
            Assert.That( crashFile, Is.Not.Null );
            Assert.That( crashFile.Exists, Is.True );
        }
    }
}
