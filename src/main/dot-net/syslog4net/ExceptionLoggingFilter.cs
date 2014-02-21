using System;
using log4net.Filter;

namespace MerchantWarehouse.Diagnostics
{
    /// <summary>
    /// Handles marking logging events that contain exception data so that the extended data may be handled and reported to all appenders with a unifying exception ID.
    /// </summary>
    public class ExceptionLoggingFilter : FilterSkeleton
    {
        public override FilterDecision Decide(log4net.Core.LoggingEvent loggingEvent)
        {
            // if there is an exception we generate a GUID and store it for the appender to use
            if (loggingEvent.ExceptionObject != null)
            {
                var exceptionId = Guid.NewGuid().ToString("N");
                loggingEvent.Properties["log4net:mw-exception-key"] = exceptionId;
            }

            // This filter should not change the behavior of any other configured filters of logging settings
            return FilterDecision.Neutral;
        }
    }
}
