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
        public CrashReportModel( IEnumerable<FileInfo> crashLogs, DateTime lastReport )
        {
            CrashLogs = crashLogs;
            LastReportDate = lastReport;

            CountByApplication = CrashLogs.GroupBy( c => c.Directory.Parent.Name ).ToDictionary( g => g.Key, g => g.Count() );
        }

        public void ConfigureMail( CrashReportModel model, MailParams mailParams )
        {
        }

        public IEnumerable<FileInfo> CrashLogs { get; private set; }
        public DateTime LastReportDate { get; private set; }
        public IDictionary<string, int> CountByApplication { get; private set; }

        public string GetSubject( CrashReportModel model )
        {
            return string.Format( "CiviKey Weekly Reports : {0} new crash logs", model.CrashLogs.Count() );
        }
    }
}
