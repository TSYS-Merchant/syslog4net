using System.IO;
using System.Net.NetworkInformation;

using log4net.Core;
using log4net.Layout.Pattern;

namespace syslog4net.Converters
{
    /// <summary>
    /// Provides the currently configured FQDN for the current host. Does not validate name with DNS.
    /// </summary>
    public class HostnameConverter : PatternLayoutConverter
    {
        public static string GetLocalhostFqdn()
        {
            var ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            return string.Format("{0}.{1}", ipProperties.HostName, ipProperties.DomainName);
        }

        override protected void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            writer.Write(PrintableAsciiSanitizer.Sanitize(GetLocalhostFqdn().ToUpperInvariant(), 255));
        }
    }
}
