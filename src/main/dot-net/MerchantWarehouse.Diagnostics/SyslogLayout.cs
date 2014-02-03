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
    // TODO: save exceptions to a configurable place instead of the temp directory

    // https://confluence.mw.inc/display/TO/TOps+Syslog+Standard
    class SyslogLayout : LayoutSkeleton
    {
        log4net.Layout.PatternLayout _layout;

        // http://tools.ietf.org/html/rfc5424#section-6.1
        private const int SYSLOG_MAX_MESSAGE_LENGTH = 2048;

        public SyslogLayout()
        {
            IgnoresException = false;

            _layout = new PatternLayout("<%mw-priority>1 %utcdate{yyyy-MM-ddTHH:mm:ss:FFZ} %mw-hostname %mw-app-domain %mw-process-id %mw-message-id %mw-structured-data %message%newline");

            _layout.AddConverter("mw-priority", typeof(PriorityConverter));
            _layout.AddConverter("mw-hostname", typeof(HostnameConverter));
            _layout.AddConverter("mw-app-domain", typeof(ApplicationNameConverter));
            _layout.AddConverter("mw-process-id", typeof(ProcessIdConverter));
            _layout.AddConverter("mw-message-id", typeof(MessageIdConverter));
            _layout.AddConverter("mw-structured-data", typeof(StructuredDataConverter));
            _layout.AddConverter("mw-thread-id", typeof(ThreadIdConverter));
            _layout.AddConverter("mw-thread-name", typeof(ThreadNameConverter));
            _layout.AddConverter("mw-command-line", typeof(CommandLineConverter));
            _layout.AddConverter("mw-process-name", typeof(ProcessNameConverter));

            ActivateOptions();
        }

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

        override public void ActivateOptions()
        {
            _layout.ActivateOptions();
        }
    }
}
