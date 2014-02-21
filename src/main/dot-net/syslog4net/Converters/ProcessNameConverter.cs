using System.Diagnostics;
using System.IO;
using log4net.Core;
using log4net.Layout.Pattern;

namespace syslog4net.Converters
{
    /// <summary>
    /// Provides the current Process name as recorded by the operating system (often the name of the executable or host runtime)
    /// </summary>
    public class ProcessNameConverter : PatternLayoutConverter
    {
        private static string _name;

        override protected void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            _name = string.IsNullOrEmpty(_name) ? Process.GetCurrentProcess().ProcessName : _name;

            if (string.IsNullOrEmpty(_name))
            {
                _name = "-"; // the NILVALUE
            }

            writer.Write(PrintableAsciiSanitizer.Sanitize(_name, 48));
        }
    }
}
