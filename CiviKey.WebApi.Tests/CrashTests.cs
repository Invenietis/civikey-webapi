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
        //void SetupControllerForPostAction<T>( T controller, dynamic routeValues ) where T : ApiController
        //{
        //    var config = new HttpConfiguration();
        //    var request = new HttpRequestMessage( HttpMethod.Post, "http://localhost/api/" + routeValues.action );
        //    var route = config.Routes.MapHttpRoute( "Route", "api/{controller}/{action}" );
        //    var routeData = new HttpRouteData( route, new HttpRouteValueDictionary( routeValues ) );

        //    controller.ControllerContext = new HttpControllerContext( config, routeData, request );
        //    controller.Request = request;
        //    controller.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;
        //}

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
            Assert.That( crashFile.Directory.Name, Is.EqualTo( "2009-01-01" ) );
            Assert.That( crashFile.Directory.Parent.Name, Is.EqualTo( "unittests" ) );
        }

        //[Test]
        //public void CreateCrashWithFile()
        //{
        //    // Given
        //    TestConfiguration config = new TestConfiguration();
        //    config.Settings.CrashStorageDirectory = "CrashDirectory";

        //    CrashService svc = new CrashService( config );

        //    CrashController ctrl = new CrashController( svc );
        //    SetupControllerForPostAction( ctrl, new { action = "Post", civiKeyInstanceIdentifier = "unittest" } );


        //    // When
        //    MemoryStream ms = new MemoryStream();
        //    using( TextWriter tx = new StreamWriter( ms, Encoding.Unicode, 128, true ) )
        //    {
        //        for( int i = 0; i < 100; i++ )
        //            tx.WriteLine( "line {0}", i );

        //        using( var formData = new MultipartFormDataContent() )
        //        {
        //            ms.Seek( 0, SeekOrigin.Begin );
        //            formData.Add( new StreamContent( ms ), "file", "crashLog-2013-10-24 10-17-42Z.log" );
        //            ctrl.Request.Content = formData;

        //            var response = ctrl.Post( "unittest" );
        //            Assert.That( response.Result, Is.EqualTo( HttpStatusCode.Created ) );

        //            Thread.Sleep( 5000 );
        //        }
        //    }

        //// Then
        //Assert.That( crashFile, Is.Not.Null );
        //Assert.That( crashFile.Exists, Is.True );
        //}
    }
}
