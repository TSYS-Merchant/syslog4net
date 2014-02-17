using System;
using System.IO;
using System.Text;
using log4net.Appender;
using log4net.Core;

namespace MerchantWarehouse.Diagnostics
{
    /// <summary>
    /// Wrapper for the UDP appender to enable dual writing to different targets under different circumstances.
    /// The MW requirement is that all detialed excpetion data be written to a file local to the application 
    /// rather than to syslog via UDP. This version of the appender uses the ID set by the exception filter to ensure
    /// that the correct information pointing to the error file is included in the syslog message.
    /// </summary>
    public class ExceptionFileUdpAppender : UdpAppender
    {
        private const string IdToken = "{errorId}";
        private const string BaseErrorLogPath = @"log\errors\";
        private const string DefaultFileName = @"error_" + IdToken + ".txt";

        public ExceptionFileUdpAppender()
        {
            if (!Directory.Exists(BaseErrorLogPath))
            {
                Directory.CreateDirectory(BaseErrorLogPath);
            }
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            WriteExceptionFile(loggingEvent);
            base.Append(loggingEvent);
        }

        protected override void Append(LoggingEvent[] loggingEvents)
        {
            WriteExceptionFile(loggingEvents);
            base.Append(loggingEvents);
        }

        private void WriteExceptionFile(params LoggingEvent[] loggingEvents)
        {
            foreach (var evt in loggingEvents)
            {
                if (evt.ExceptionObject != null)
                {
                    var logfilePath = BaseErrorLogPath + DefaultFileName.Replace(IdToken, evt.Properties["log4net:mw-exception-key"].ToString());

                    // Should not need any complex locking or threading here as we dump the info
                    // to the file and never touch that file again.
                    File.WriteAllText(logfilePath, BuildErrorString(evt.ExceptionObject));
                }
            }
        }

        private string BuildErrorString(Exception ex)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Source : " + ex.Source);
            sb.AppendLine("Type : " + ex.GetType());
            sb.AppendLine("Message : " + ex.Message);
            sb.AppendLine("Target Site : " + ex.TargetSite);
            sb.AppendLine("Help Link : " + ex.HelpLink);
            sb.AppendLine("HResult : " + ex.HResult);
            sb.AppendLine("Stack Trace : " + ex.StackTrace);

            if (ex.InnerException != null)
            {
                sb.AppendLine();
                sb.AppendLine("---INNER EXCEPTION DATA---");
                sb.AppendLine(BuildErrorString(ex.InnerException));
            }

            return sb.ToString();
        }
    }
}
