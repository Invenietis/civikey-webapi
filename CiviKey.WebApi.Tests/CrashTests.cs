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
using CiviKey.WebApi.Crash;
using NUnit.Framework;

namespace CiviKey.WebApi.Tests
{
    [TestFixture]
    public class CrashTests
    {
        [Test]
        public void CreateCrash()
        {
            // Given
            TestConfiguration config = new TestConfiguration();
            config.Settings.CrashStorageDirectory = "CrashDirectory";

            CrashService svc = new CrashService( config );

            // When
            FileInfo crashFileStream = null;
            FileInfo crashFile = null;
            MemoryStream ms = new MemoryStream();
            using( TextWriter tx = new StreamWriter( ms ) )
            {
                for( int i = 0; i < 100; i++ )
                    tx.WriteLine( "line {0}", i );

                crashFileStream = svc.RegisterCrash( "unittests", ms );
                crashFile = svc.RegisterCrash( "unittests", ms, "crashLog-2009-01-01 10-17-42Z.log" );
            }

            // Then
            Assert.That( crashFileStream, Is.Not.Null );
            Assert.That( crashFileStream.Exists, Is.True );
            Assert.That( crashFileStream.Directory.Name, Is.EqualTo( DateTime.Today.ToString( "yyyy-MM-dd" ) ) );
            Assert.That( crashFileStream.Directory.Parent.Name, Is.EqualTo( "unittests" ) );

            Assert.That( crashFile, Is.Not.Null );
            Assert.That( crashFile.Exists, Is.True );
            Assert.That( crashFile.Directory.Name, Is.EqualTo( "unittests" ) );
            Assert.That( crashFile.Directory.Parent.Name, Is.EqualTo( "2009-01-01" ) );
        }
    }
}
