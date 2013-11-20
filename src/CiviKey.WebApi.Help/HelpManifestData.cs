using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CiviKey.WebApi.Help
{
    public class HelpManifestData
    {
        public string PluginId { get; set; }

        public string Version { get; set; }
        
        public string Culture { get; set; }
        
        public string Hash { get; set; }

        public static HelpManifestData Deserialize( Stream data )
        {
            XmlSerializer x = new XmlSerializer( typeof( HelpManifestData ) );
            return x.Deserialize( data ) as HelpManifestData;
        }

        public void Serialize( TextWriter data )
        {
            XmlSerializer x = new XmlSerializer( typeof( HelpManifestData ) );
            x.Serialize( data, this );
        }
    }
}
