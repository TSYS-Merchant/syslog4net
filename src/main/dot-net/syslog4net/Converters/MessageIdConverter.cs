using System.IO;
using log4net.Core;
using log4net.Layout.Pattern;
using syslog4net.Util;

namespace syslog4net.Converters
{
    /// <summary>
    /// Provides conversion of the MessageId logging event property or the NDC stack data as a message id for correlation purposes
    /// </summary>
    public class MessageIdConverter : PatternLayoutConverter
    {
        override protected void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            string messageId = null;

            // pop the NDC
            log4net.Util.ThreadContextStack ndc = loggingEvent.LookupProperty("NDC") as log4net.Util.ThreadContextStack;
            if (ndc != null && ndc.Count > 0)
            {
                // the NDC represents a context stack, whose levels are separated by whitespace. we will use this as our MessageId.
                messageId = ndc.ToString();
            }

            if (string.IsNullOrEmpty(messageId))
            {
                messageId = "-"; // the NILVALUE
            }
            else
            {
                messageId = messageId.Replace(' ', '.'); // replace spaces with periods
            }

            writer.Write(PrintableAsciiSanitizer.Sanitize(messageId, 32));
        }
    }
}
