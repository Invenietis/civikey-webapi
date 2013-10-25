using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiviKey.WebApi.Core.Configuration
{
    public interface IConfiguration
    {
        dynamic Settings { get; }
    }
}
