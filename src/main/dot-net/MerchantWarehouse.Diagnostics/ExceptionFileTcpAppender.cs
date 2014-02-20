using System;
using System.Globalization;
using System.IO;
using System.Net;
using log4net;
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
    public class ExceptionFileTcpAppender : TcpAppender
    {
        private const string IdToken = "{errorId}";
        private const string DefaultFileName = @"error_" + IdToken + ".txt";

        public string ExceptionLogFolder { get; set; }

        public string StructuredDataPrefix { get; set; }

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

        public override void ActivateOptions()
        {
            base.ActivateOptions();

            if (string.IsNullOrWhiteSpace(this.ExceptionLogFolder))
            {
                throw new ArgumentNullException("The required property 'ExceptionFileLogFolder' was not specified.");
            }
            if (!Directory.Exists(this.ExceptionLogFolder))
            {
                Directory.CreateDirectory(this.ExceptionLogFolder);
            }
            GlobalContext.Properties["log4net:StructuredDataPrefix"] = StructuredDataPrefix;
            GlobalContext.Properties["log4net:ExceptionLogFolder"] = ExceptionLogFolder;
        }

        private void WriteExceptionFile(params LoggingEvent[] loggingEvents)
        {
            foreach (var evt in loggingEvents)
            {
                if (evt.ExceptionObject != null)
                {
                    var logfilePath = this.ExceptionLogFolder + Path.DirectorySeparatorChar + DefaultFileName.Replace(IdToken, evt.Properties["log4net:mw-exception-key"].ToString());

                    //TODO what happens during file name collision?

                    // Should not need any complex locking or threading here as we dump the info
                    // to the file and never touch that file again.
                    File.WriteAllText(logfilePath, evt.ExceptionObject.ToString());
                }
            }
        }
    }
}
