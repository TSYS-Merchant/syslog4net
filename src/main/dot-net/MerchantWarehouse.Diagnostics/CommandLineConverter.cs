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
    public class CommandLineConverter : PatternLayoutConverter
    {
        override protected void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            writer.Write(PrintableAsciiSanitizer.Sanitize(Environment.CommandLine, 255));
        }
    }
}
