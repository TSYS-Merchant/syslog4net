using System;
using System.IO;
using log4net.Filter;
using log4net.Core;

namespace syslog4net.Filter
{
    /// <summary>
    /// Writes exceptions to a local file on disk, and passes along the log file location for other appenders to use.
    /// </summary>
    public class ExceptionLoggingFilter : FilterSkeleton
    {
        private const string IdToken = "{errorId}";
        private const string DefaultFileName = @"error_" + IdToken + ".txt";

        public string ExceptionLogFolder { get; set; }

        public override FilterDecision Decide(LoggingEvent loggingEvent)
        {
            // if there is an exception we generate a GUID and store it for the appender to use
            if (loggingEvent.ExceptionObject != null)
            {
                var exceptionId = Guid.NewGuid().ToString("N");

                var logfilePath = this.ExceptionLogFolder + Path.DirectorySeparatorChar + DefaultFileName.Replace(IdToken, exceptionId.ToString());
                loggingEvent.Properties["log4net:syslog-exception-log"] = logfilePath;

                // Should not need any complex locking or threading here as we dump the info
                // to the file and never touch that file again.
                File.WriteAllText(logfilePath, loggingEvent.ExceptionObject.ToString());
            }

            // This filter should not change the behavior of any other configured filters of logging settings
            return FilterDecision.Neutral;
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
        }
    }
}
