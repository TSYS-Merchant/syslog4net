using log4net.Core;
using log4net.Layout.Pattern;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MerchantWarehouse.Diagnostics
{
    class ProcessNameConverter : PatternLayoutConverter
    {
        override protected void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            var name = Process.GetCurrentProcess().ProcessName;
            if (string.IsNullOrEmpty(name))
            {
                ProcessIdConverter.Converter.Format(writer, loggingEvent);
            }

            writer.Write(PrintableAsciiSanitizer.Sanitize(name, 48));
        }
    }
}
