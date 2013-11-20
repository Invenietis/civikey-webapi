using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CiviKey.WebApi.Help
{
    public class HashProvider
    {
        public byte[] ComputeHash( DirectoryInfo directory )
        {
            if( directory.Exists )
            {
                var files = directory.EnumerateFiles( "*", SearchOption.AllDirectories );
                if( files.Any() )
                {
                    byte[] bytes = new byte[files.Count() * HashArrayLength];

                    int currentIndex = 0;
                    foreach( var f in files )
                    {
                        ComputeHash( f ).CopyTo( bytes, currentIndex );
                        currentIndex += HashArrayLength;
                    }

                    return ComputeHash( bytes );
                }
            }

            return new byte[0];
        }

        public byte[] ComputeHash( FileInfo file )
        {
            return ComputeHash( file.OpenRead() );
        }

        public byte[] ComputeHash( byte[] bytes )
        {
            using( var h = GetHashAlgorithm() )
            {
                return h.ComputeHash( bytes );
            }
        }

        public byte[] ComputeHash( Stream stream )
        {
            using( var h = GetHashAlgorithm() )
            {
                return h.ComputeHash( stream );
            }
        }

        HashAlgorithm GetHashAlgorithm()
        {
            return SHA512.Create();
        }

        int _hashArrayLength = -1;
        int HashArrayLength
        {
            get
            {
                if( _hashArrayLength == -1 )
                {
                    using( var h = GetHashAlgorithm() )
                        _hashArrayLength = h.ComputeHash( new byte[0] ).Length;
                }
                return _hashArrayLength;
            }
        }
    }
}
