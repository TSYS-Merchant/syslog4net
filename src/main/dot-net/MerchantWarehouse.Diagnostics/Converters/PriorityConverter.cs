using System.IO;

using log4net.Core;
using log4net.Layout.Pattern;


namespace MerchantWarehouse.Diagnostics.Converters
{
    /// <summary>
    /// Converts standard logging levels into merchant warehouse specific syslog priority codes as defined in the TOPS Syslog standard: https://confluence.mw.inc/display/TO/TOps+Syslog+Standard
    /// </summary>
    public class PriorityConverter : PatternLayoutConverter
    {
        /// <summary>
        /// Helper method to convert <see cref="Level"/> into a string ID that is mapped based upon the TOPS Syslog standard: https://confluence.mw.inc/display/TO/TOps+Syslog+Standard
        /// </summary>
        /// <param name="level"><see cref="Level"/> to convert to string</param>
        /// <returns>string representing the syslog priority code as defined in the TOPS Syslog standard</returns>
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
