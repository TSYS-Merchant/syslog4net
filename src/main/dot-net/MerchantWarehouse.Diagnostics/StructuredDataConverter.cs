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
    /// <summary>
    /// Converts data found within the properties of a logging event into Key/Value pairs to be displayed using syslog's Extended Data format as described 
    /// in the TOPS Syslog standard: https://confluence.mw.inc/display/TO/TOps+Syslog+Standard
    /// </summary>
    public class StructuredDataConverter : PatternLayoutConverter
    {
        public StructuredDataConverter()
        {
            // This converter handles the exception
            IgnoresException = false;
        }

        private static string SanitizeSdName(string sdName)
        {
            // sanitize the SD-NAME as per http://tools.ietf.org/html/rfc5424#section-6.3.3
            // SD-NAME         = 1*32PRINTUSASCII; except '=', SP, ']', %d34 (")

            return PrintableAsciiSanitizer.Sanitize(sdName, 32, new byte[] { 0x5D, 0x22, 0x3D });
        }

        private static string SanitizeSdParamValue(string sdParamValue)
        {
            // sanitize the SD-PARAM-VALUE as per http://tools.ietf.org/html/rfc5424#section-6.3.3
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();

            foreach (char ch in sdParamValue)
            {
                if (ch == '"' || ch == '\\' || ch == ']')
                {
                    // escape the character by prepending a literal '\'
                    stringBuilder.Append('\\');
                }
                stringBuilder.Append(ch);
            }

            return stringBuilder.ToString();
        }

        private static void AddStructuredData(TextWriter writer, string sdParamName, string sdParamValue)
        {
            if (!string.IsNullOrEmpty(sdParamValue))
            {
                writer.Write(" ");
                writer.Write(SanitizeSdName(sdParamName));
                writer.Write("=\"");
                writer.Write(SanitizeSdParamValue(sdParamValue));
                writer.Write("\"");
            }
        }

        /*
        public static string GetTempFilePathWithExtension(string extension)
        {
            var path = Path.GetTempPath();
            var fileName = System.Guid.NewGuid().ToString() + extension;
            return Path.Combine(path, fileName);
        }*/

        /*
        private static void SaveException(System.Exception exceptionObject, out System.Uri uri)
        {
            var path = GetTempFilePathWithExtension(".txt");

            using (var file = new System.IO.StreamWriter(path))
            {
                file.WriteLine(exceptionObject.ToString());
                uri = new System.Uri(path);
            }
        }*/

        private void HandleException(TextWriter writer, LoggingEvent loggingEvent)
        {
            System.Exception exceptionObject = loggingEvent.ExceptionObject;

            if (exceptionObject != null)
            {
                AddStructuredData(writer, "ExceptionSource", exceptionObject.Source);
                AddStructuredData(writer, "ExceptionType", exceptionObject.GetType().FullName);
                AddStructuredData(writer, "ExceptionMessage", exceptionObject.Message);
                AddStructuredData(writer, "EventHelp", exceptionObject.HelpLink);

                // Minor cheat at the moment by using a hardcoded path that is relative to the running
                // application container.
                AddStructuredData(writer, "EventLog", System.IO.Directory.GetCurrentDirectory() + "logs\\error\\error_" + loggingEvent.Properties["log4net:mw-exception-key"] + ".txt");
            }
            else
            {
                string exceptionString = loggingEvent.GetExceptionString();
                if (!string.IsNullOrEmpty(exceptionString))
                {
                    AddStructuredData(writer, "ExceptionMessage", exceptionString);
                }
            }
        }

        override protected void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            writer.Write("[MW@55555");

            var properties = loggingEvent.GetProperties();
            foreach (var key in properties.GetKeys())
            {
                if (!key.StartsWith("log4net:")) // ignore built-in log4net diagnostics. keep the NDC stack in there.
                {
                    AddStructuredData(writer, key, properties[key].ToString());
                }
            }

            AddStructuredData(writer, "EventSeverity", loggingEvent.Level.DisplayName);
            HandleException(writer, loggingEvent);

            writer.Write("]");
        }
    }
}
