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
    class PriorityConverter : PatternLayoutConverter
    {
        public static string ConvertLevelToPriority(Level level)
        {
            if (level >= Level.Emergency)
            {
                return "128";
            }
            else if (level >= Level.Fatal)
            {
                return "130";
            }
            else if (level >= Level.Error)
            {
                return "131";
            }
            else if (level >= Level.Warn)
            {
                return "132";
            }
            else if (level >= Level.Info)
            {
                return "134";
            }
            else
            {
                return "135"; // debug
            }
        }

        override protected void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            writer.Write(ConvertLevelToPriority(loggingEvent.Level));
        }
    }
}
