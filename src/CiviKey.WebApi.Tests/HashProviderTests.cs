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
using CiviKey.WebApi.Help;
using NUnit.Framework;

namespace CiviKey.WebApi.Tests
{
    [TestFixture]
    public class HashProviderTests
    {
        [Test]
        public void ComputeHashOfCurrentDirectory()
        {
            // Given
            HashProvider hp = new HashProvider();
            DirectoryInfo currentDir = new DirectoryInfo( new TestConfiguration().GetRootPath() );

            if( !currentDir.EnumerateFiles( "*", SearchOption.AllDirectories ).Any() )
                File.Create( Path.Combine( currentDir.FullName, string.Format( "{0}.test", Guid.NewGuid().ToString( "B" ) ) ) ).Dispose();

            // When
            byte[] hash1 = hp.ComputeHash( currentDir );
            byte[] hash2 = hp.ComputeHash( currentDir );

            File.Create( Path.Combine( currentDir.FullName, string.Format( "{0}.test", Guid.NewGuid().ToString( "B" ) ) ) ).Dispose();
            byte[] hashChanged = hp.ComputeHash( currentDir );

            // Then
            Assert.That( hash1, Is.Not.Null );
            Assert.That( hash2, Is.Not.Null );
            Assert.That( hashChanged, Is.Not.Null );

            CollectionAssert.AllItemsAreNotNull( hash1 );
            CollectionAssert.AllItemsAreNotNull( hash2 );
            CollectionAssert.AllItemsAreNotNull( hashChanged );

            Assert.That( hash1.SequenceEqual( hash2 ), Is.True );
            Assert.That( hash2.SequenceEqual( hashChanged ), Is.False );
        }

        [Test]
        public void ComputeHashOnEmptyDirectory()
        {
            // Given
            HashProvider hp = new HashProvider();
            DirectoryInfo emptyDir = new DirectoryInfo( Path.Combine( new TestConfiguration().GetRootPath(), Guid.NewGuid().ToString( "B" ) ) );
            emptyDir.Create();

            // When
            byte[] hash = hp.ComputeHash( emptyDir );

            // Then
            Assert.That( hash, Is.Not.Null );
            Assert.That( hash.All( b => b == 0 ) );

            emptyDir.Delete();
        }
    }
}
