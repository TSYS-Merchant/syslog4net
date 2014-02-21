using System;
using System.IO;
using log4net.Core;
using log4net.Layout.Pattern;

namespace syslog4net.Converters
{
    /// <summary>
    /// Provides the Command path and arguments used to start the current process
    /// </summary>
    public class CommandLineConverter : PatternLayoutConverter
    {
        override protected void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            writer.Write(PrintableAsciiSanitizer.Sanitize(Environment.CommandLine, 255));
        }
    }
}
