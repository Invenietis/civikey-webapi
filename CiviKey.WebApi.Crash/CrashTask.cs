using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CiviKey.WebApi.Core.Configuration;
using CK.Mailer;
using CK.TaskHost;

namespace CiviKey.WebApi.Crash
{
    public class CrashTask : CKTask
    {
        static Regex RecipientRegex = new Regex( @"(?<displayname>.*).*\*(?<email>.*)\*" );

        CrashService _crashService;
        IMailerService _mailer;
        IConfiguration _config;

        public CrashTask( CKTaskBuilder builder, IConfiguration config, CrashService crashService, IMailerService mailer )
            : base( builder )
        {
            _crashService = crashService;
            _config = config;
            _mailer = mailer;
        }

        protected override void Execute()
        {
            if( DataBag.LastRun != null )
            {
                IEnumerable<FileInfo> crashLogs = _crashService.GetCrashsSince( DataBag.LastRun );
                if( crashLogs.Any() )
                {
                    string rawRecipients = _config.Settings.CrashReportsRecipents;

                    RecipientModel recipientModel = new RecipientModel();
                    foreach( var r in rawRecipients.Split( ';' ).Select( ParseRecipient ) )
                        recipientModel.Recipients.Add( r );

                    _mailer.SendMail( new CrashReportModel( crashLogs, DataBag.LastRun ), new RazorMailTemplateKey( "CrashReport" ), recipientModel );
                }
            }

            DataBag.LastRun = DateTime.Today;

            // next run set to next monday morning
            DateTime today = DateTime.Today;
            int daysUntilMonday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
            DateTime nextMonday = today.AddDays( daysUntilMonday );

            SetNextRunDate( today.AddDays( daysUntilMonday ).AddHours( 8.5 ) );
            SaveDataBag();
        }

        Recipient ParseRecipient( string recipient )
        {
            var match = RecipientRegex.Match( recipient );
            return new Recipient
            {
                DisplayName = match.Groups["displayname"].Value.Trim(),
                EmailAddress = match.Groups["email"].Value.Trim()
            };
        }
    }
}
