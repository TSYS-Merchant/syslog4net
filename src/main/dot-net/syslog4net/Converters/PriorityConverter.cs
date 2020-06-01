using System.IO;

using log4net.Core;
using log4net.Layout.Pattern;


namespace syslog4net.Converters
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
        public static string ConvertLevelToPriority(LoggingEvent loggingEvent)
        {
            int facility = 16; // local0
            if (loggingEvent.Properties["log4net:FacilityCode"] != null && !string.IsNullOrEmpty(loggingEvent.Properties["log4net:FacilityCode"].ToString()))
                if (!int.TryParse(loggingEvent.Properties["log4net:FacilityCode"].ToString(), out facility))
                    facility = 16;

            int gravity = 7; // debugging;
            if (loggingEvent.Level >= Level.Emergency)
                gravity = 0;
            else if (loggingEvent.Level >= Level.Fatal)
                gravity = 2;
            else if (loggingEvent.Level >= Level.Error)
                gravity = 3;
            else if (loggingEvent.Level >= Level.Warn)
                gravity = 4;
            else if (loggingEvent.Level >= Level.Info)
                gravity = 6;

            return (facility * 8 + gravity).ToString();
        }

        override protected void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            writer.Write(ConvertLevelToPriority(loggingEvent));
        }
    }
}
