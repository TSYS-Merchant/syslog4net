using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using log4net.Core;
using log4net.Layout;
using log4net.Layout.Pattern;

namespace MerchantWarehouse.Diagnostics
{
    class ApplicationNameConverter : PatternLayoutConverter
    {
        override protected void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            string applicationName = loggingEvent.Domain;

            if (loggingEvent.Properties.Contains("ApplicationName"))
            {
                applicationName = loggingEvent.Properties["ApplicationName"].ToString();
            }

            if (string.IsNullOrEmpty(applicationName))
            {
                applicationName = "-"; // the NILVALUE
            }

            writer.Write(PrintableAsciiSanitizer.Sanitize(applicationName, 48));
        }
    }
}
