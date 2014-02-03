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
    class MessageIdConverter : PatternLayoutConverter
    {
        override protected void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            string messageId = null;

            // first, try to pop the message id from the MDC
            if (loggingEvent.Properties.Contains("MessageId"))
            {
                messageId = loggingEvent.Properties["MessageId"].ToString();
            }

            // if that's not there, pop the NDC
            if (string.IsNullOrEmpty(messageId))
            {
                log4net.Util.ThreadContextStack ndc = loggingEvent.LookupProperty("NDC") as log4net.Util.ThreadContextStack;
                if (ndc != null)
                {
                    // the NDC represents a context stack, whose levels are separated by whitespace. here, we get the
                    // rightmost segement of the NDC. we will use this as our MessageId.
                    var ndcMessage = ndc.ToString();
                    if (!string.IsNullOrEmpty(ndcMessage))
                    {
                        int lastSpace = ndcMessage.LastIndexOf(' ');
                        if (lastSpace > -1 && ndcMessage.Length > lastSpace + 1)
                        {
                            messageId = ndcMessage.Substring(lastSpace + 1);
                        }
                        else
                        {
                            // no spaces and the NDC wasn't empty. there's just a single segment.
                            messageId = ndcMessage;
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(messageId))
            {
                messageId = "-"; // the NILVALUE
            }

            writer.Write(PrintableAsciiSanitizer.Sanitize(messageId, 32));
        }
    }
}
