using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CK.TaskHost;

namespace CiviKey.WebApi.Help
{
    public class HelpBuilderTask : CKTask
    {
        HelpBuilderService _builderService;

        public HelpBuilderTask( CKTaskBuilder builder, HelpBuilderService builderService )
            : base( builder )
        {
            _builderService = builderService;
        }

        protected override void Execute()
        {
            _builderService.CreateOrUpdateBuilds();

            SetNextRunDate( DateTime.UtcNow.AddHours( 1 ) );
        }
    }
}
