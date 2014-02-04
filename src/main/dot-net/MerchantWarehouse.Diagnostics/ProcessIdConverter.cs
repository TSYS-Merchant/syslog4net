using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

using log4net.Core;
using log4net.Layout;
using log4net.Layout.Pattern;

namespace MerchantWarehouse.Diagnostics
{
    public class ProcessIdConverter : PatternLayoutConverter
    {
        private static string _processId;

        override protected void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            _processId = string.IsNullOrEmpty(_processId) ? Process.GetCurrentProcess().Id.ToString() : _processId;

            // TODO: Do we really want to give the option to override the process ID like this? How would it be used?
            if (loggingEvent.Properties.Contains("ProcessId"))
            {
                _processId = loggingEvent.Properties["ProcessId"].ToString();
            }

            if (string.IsNullOrEmpty(_processId))
            {
                _processId = "-"; // the NILVALUE
            }

            writer.Write(PrintableAsciiSanitizer.Sanitize(_processId, 48));
        }
    }
}
