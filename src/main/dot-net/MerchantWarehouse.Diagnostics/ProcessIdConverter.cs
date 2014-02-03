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
    class ProcessIdConverter : PatternLayoutConverter
    {
        override protected void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            string processId = Process.GetCurrentProcess().Id.ToString();

            if (loggingEvent.Properties.Contains("ProcessId"))
            {
                processId = loggingEvent.Properties["ProcessId"].ToString();
            }

            if (string.IsNullOrEmpty(processId))
            {
                processId = "-"; // the NILVALUE
            }

            writer.Write(PrintableAsciiSanitizer.Sanitize(processId, 48));
        }

        private static ProcessIdConverter _converter = new ProcessIdConverter();
        internal static ProcessIdConverter Converter
        {
            get
            {
                return _converter;
            }
        }
    }
}
