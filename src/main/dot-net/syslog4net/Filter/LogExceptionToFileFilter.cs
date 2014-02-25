using System;
using System.IO;
using log4net.Filter;
using log4net.Core;

namespace syslog4net.Filter
{
    /// <summary>
    /// Writes exceptions to a local file on disk, and passes along the log file location for other appenders to use.
    /// </summary>
    public class LogExceptionToFileFilter : FilterSkeleton
    {
        private const string IdToken = "{errorId}";
        private const string DefaultFileName = @"error_" + IdToken + ".txt";

        /// <summary>
        /// The folder that all exceptions will be logged to. Must be set before any messages are filtered.
        /// </summary>
        public string ExceptionLogFolder { get; set; }

        /// <summary>
        /// If any exceptions are present on the logging event, save them to a file and set the "log4net:syslog-exception-log" property on the event.
        /// </summary>
        /// <param name="loggingEvent">a logging event</param>
        /// <returns>FilterDecision.Neutral</returns>
        public override FilterDecision Decide(LoggingEvent loggingEvent)
        {
            // if there is an exception we generate a GUID and store it for the appender to use
            if (loggingEvent.ExceptionObject != null)
            {
                var exceptionId = Guid.NewGuid().ToString("N");

                var logfilePath = this.ExceptionLogFolder + Path.DirectorySeparatorChar + DefaultFileName.Replace(IdToken, exceptionId);
                loggingEvent.Properties["log4net:syslog-exception-log"] = logfilePath;

                // Should not need any complex locking or threading here as we dump the info
                // to the file and never touch that file again.
                File.WriteAllText(logfilePath, loggingEvent.ExceptionObject.ToString());
            }

            // This filter should not change the behavior of any other configured filters of logging settings
            return FilterDecision.Neutral;
        }

        /// <summary>
        /// Activates log4net's options
        /// </summary>
        public override void ActivateOptions()
        {
            base.ActivateOptions();

            if (string.IsNullOrWhiteSpace(this.ExceptionLogFolder))
            {
                throw new ArgumentNullException("ExceptionLogFolder");
            }

            if (!Directory.Exists(this.ExceptionLogFolder))
            {
                Directory.CreateDirectory(this.ExceptionLogFolder);
            }
        }
    }
}
