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
