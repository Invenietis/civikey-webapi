using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CK.Mailer;

namespace CiviKey.WebApi.Crash
{
    public class CrashReportModel : IMailConfigurator<CrashReportModel>
    {
        public CrashReportModel( DateTime lastReport )
        {
            LastReportDate = lastReport;
        }

        public CrashReportModel( IEnumerable<FileInfo> crashLogs, DateTime lastReport )
            : this( lastReport )
        {
            CrashLogs = crashLogs;
            CountByApplication = CrashLogs.GroupBy( c => c.Directory.Name ).ToDictionary( g => g.Key, g => g.Count() );
        }

        public void ConfigureMail( CrashReportModel model, MailParams mailParams )
        {
        }

        public IEnumerable<FileInfo> CrashLogs { get; private set; }
        public DateTime LastReportDate { get; private set; }
        public IDictionary<string, int> CountByApplication { get; private set; }

        public string GetSubject( CrashReportModel model )
        {
            if( model.CrashLogs != null )
            {
                if( model.CrashLogs.Count() > 1 )
                    return string.Format( "CiviKey Weekly Reports : {0} new crash logs", model.CrashLogs.Count() );
                else return string.Format( "CiviKey Weekly Reports : {0} new crash log", model.CrashLogs.Count() );
            }
            else
            {
                return "CiviKey Weekly Reports : Nothing to report";
            }
        }
    }
}
