using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using log4net.Core;
using log4net.Layout;
using log4net.Layout.Pattern;
using MerchantWarehouse.Diagnostics.Converters;

namespace MerchantWarehouse.Diagnostics
{

    /// <summary>
    /// Log4net layout class with default support for the Syslog message format as described in the TOPS Syslong standard: https://confluence.mw.inc/display/TO/TOps+Syslog+Standard
    /// </summary>
    class SyslogLayout : LayoutSkeleton
    {
        log4net.Layout.PatternLayout _layout;

        // http://tools.ietf.org/html/rfc5424#section-6.1
        private const int SYSLOG_MAX_MESSAGE_LENGTH = 2048;

        /// <summary>
        /// Instantiates a new instance of <see cref="SyslogLayout"/>
        /// </summary>
        public SyslogLayout()
        {
            IgnoresException = false;

            _layout = new PatternLayout("<%mw-priority>1 %utcdate{yyyy-MM-ddTHH:mm:ss:FFZ} %mw-hostname %appdomain %mw-process-id %mw-message-id %mw-structured-data %message%newline");

            _layout.AddConverter("mw-priority", typeof(PriorityConverter));
            _layout.AddConverter("mw-hostname", typeof(HostnameConverter));
            //_layout.AddConverter("mw-app-domain", typeof(ApplicationNameConverter)); //use %appdomain, same implmentation
            _layout.AddConverter("mw-process-id", typeof(ProcessIdConverter));
            _layout.AddConverter("mw-message-id", typeof(MessageIdConverter));
            _layout.AddConverter("mw-structured-data", typeof(StructuredDataConverter));
            _layout.AddConverter("mw-thread-id", typeof(ThreadIdConverter));
            _layout.AddConverter("mw-thread-name", typeof(ThreadNameConverter));
            _layout.AddConverter("mw-command-line", typeof(CommandLineConverter));
            _layout.AddConverter("mw-process-name", typeof(ProcessNameConverter));

            ActivateOptions();
        }

        /// <summary>
        /// Formats data within the event and writes the formatted data out to the provided writer instance
        /// </summary>
        /// <param name="writer">writer to output the formatted data to</param>
        /// <param name="logEvent">logging event data to use</param>
        override public void Format(TextWriter writer, LoggingEvent logEvent)
        {
            using (var stringWriter = new StringWriter())
            {
                _layout.Format(stringWriter, logEvent);

                // truncate the message to SYSLOG_MAX_MESSAGE_LENGTH or fewer bytes
                string message = stringWriter.ToString();

                var utf8 = System.Text.Encoding.UTF8;

                byte[] utfBytes = utf8.GetBytes(message);
                if (utfBytes.Length > SYSLOG_MAX_MESSAGE_LENGTH)
                {
                    message = utf8.GetString(utfBytes, 0, SYSLOG_MAX_MESSAGE_LENGTH);
                }

                writer.Write(message);
            }
        }

        /// <summary>
        /// Activates the use of options for the converter allowing the underlying PatternLayout implmentation to behave correctly
        /// </summary>
        override public void ActivateOptions()
        {
            _layout.ActivateOptions();
        }
    }
}
