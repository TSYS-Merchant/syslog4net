using log4net.Core;
using log4net.Layout.Pattern;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerchantWarehouse.Diagnostics
{
    public class ThreadIdConverter : PatternLayoutConverter
    {
        override protected void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            var id = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();
            writer.Write(PrintableAsciiSanitizer.Sanitize(id, 48));
        }

        private static ThreadIdConverter _converter = new ThreadIdConverter();
        internal static ThreadIdConverter Converter
        {
            get
            {
                return _converter;
            }
        }
    }
}
