using System.Diagnostics;
using System.IO;
using log4net.Core;
using log4net.Layout.Pattern;
using System.Globalization;

namespace syslog4net.Converters
{
    /// <summary>
    /// Provides conversion to string the current process ID.
    /// </summary>
    public class ProcessIdConverter : PatternLayoutConverter
    {
        private static string _processId;

        override protected void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            _processId = string.IsNullOrEmpty(_processId) ? Process.GetCurrentProcess().Id.ToString(CultureInfo.InvariantCulture) : _processId;

            if (string.IsNullOrEmpty(_processId))
            {
                _processId = "-"; // the NILVALUE
            }

            writer.Write(PrintableAsciiSanitizer.Sanitize(_processId, 48));
        }
    }
}
