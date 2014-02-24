using System.IO;
using System.Net.NetworkInformation;
using System.Globalization;
using log4net.Core;
using log4net.Layout.Pattern;
using syslog4net.Util;

namespace syslog4net.Converters
{
    /// <summary>
    /// Provides the currently configured FQDN for the current host. Does not validate name with DNS.
    /// </summary>
    public class HostnameConverter : PatternLayoutConverter
    {
        internal static string GetLocalhostFqdn()
        {
            var ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", ipProperties.HostName, ipProperties.DomainName);
        }

        override protected void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            writer.Write(PrintableAsciiSanitizer.Sanitize(GetLocalhostFqdn().ToUpperInvariant(), 255));
        }
    }
}
