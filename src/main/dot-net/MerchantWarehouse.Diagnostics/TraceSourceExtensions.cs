using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerchantWarehouse.Diagnostics
{
    /// <summary>
    /// Extensions for the type System.Diagnostics.TraceSource to simpifly developer consumption and assist
    /// in adherence in MerchantWarehouse logging policies 
    /// </summary>
    public static class TraceSourceExtensions
    {
        /*
        private static TraceSource exceptionTrace = new TraceSource("ExceptionTrace");

        public static void FlushAll(this TraceSource trace)
        {
            exceptionTrace.Flush();
        }*/

        #region log4net Analogs

        /// <summary>
        /// log4net analog ILog.Debug(object, exception) - trace deals in strings for
        /// the main message, the exception parameter is actually a parameter array
        /// accepting many types, upto and including exception types for times when the developer
        /// wants to log an exception but not have it be recorded as an exception by higher level
        /// processes that watch log output. 
        /// </summary>
        /// <param name="trace">current TraceSource instance</param>
        /// <param name="format">formatted message string <see cref="http://msdn.microsoft.com/en-us/library/txafckwd.aspx"/></param>
        /// <param name="args">data argument array</param>
        public static void Debug(this TraceSource trace, string format, params object[] args)
        {
            trace.TraceEvent(TraceEventType.Verbose, -1, format, args);
        }

        /// <summary>
        /// log4net analog ILog.Debug(object, exception) - trace deals in strings for
        /// the main message, the exception parameter is actually a parameter array
        /// accepting many types, upto and including exception types for times when the developer
        /// wants to log an exception but not have it be recorded as an exception by higher level
        /// processes that watch log output. 
        /// </summary>
        /// <param name="trace">current TraceSource instance</param>
        /// <param name="id">System.Diagnostics message ID</param>
        /// <param name="format">formatted message string <see cref="http://msdn.microsoft.com/en-us/library/txafckwd.aspx"/></param>
        /// <param name="args">data argument array</param>
        public static void Debug(this TraceSource trace, int id, string format, params object[] args)
        {
            trace.TraceEvent(TraceEventType.Verbose, id, format, args);
        }

        /// <summary>
        /// log4net analog ILog.Info(object, exception) - trace deals in strings for
        /// the main message, the exception parameter is actually a parameter array
        /// accepting many types, upto and including exception types for times when the developer
        /// wants to log an exception but not have it be recorded as an exception by higher level
        /// processes that watch log output. 
        /// </summary>
        /// <param name="trace">current TraceSource instance</param>
        /// <param name="format">formatted message string <see cref="http://msdn.microsoft.com/en-us/library/txafckwd.aspx"/></param>
        /// <param name="args">data argument array</param>
        public static void Info(this TraceSource trace, string format, params object[] args)
        {
            trace.TraceEvent(TraceEventType.Information, -1, format, args);
        }

        /// <summary>
        /// log4net analog ILog.Info(object, exception) - trace deals in strings for
        /// the main message, the exception parameter is actually a parameter array
        /// accepting many types, upto and including exception types for times when the developer
        /// wants to log an exception but not have it be recorded as an exception by higher level
        /// processes that watch log output. 
        /// </summary>
        /// <param name="trace">current TraceSource instance</param>
        /// <param name="id">System.Diagnostics message ID</param>
        /// <param name="format">formatted message string <see cref="http://msdn.microsoft.com/en-us/library/txafckwd.aspx"/></param>
        /// <param name="args">data argument array</param>
        public static void Info(this TraceSource trace, int id, string format, params object[] args)
        {
            trace.TraceEvent(TraceEventType.Information, id, format, args);
        }

        /// <summary>
        /// log4net analog ILog.Info(object, exception) - trace deals in strings for
        /// the main message, the exception parameter is actually a parameter array
        /// accepting many types, upto and including exception types for times when the developer
        /// wants to log an exception but not have it be recorded as an exception by higher level
        /// processes that watch log output. 
        /// </summary>
        /// <param name="trace">current TraceSource instance</param>
        /// <param name="format">formatted message string <see cref="http://msdn.microsoft.com/en-us/library/txafckwd.aspx"/></param>
        /// <param name="args">data argument array</param>
        public static void Warn(this TraceSource trace, string format, params object[] args)
        {
            trace.TraceEvent(TraceEventType.Warning, -1, format, args);
        }

        /// <summary>
        /// log4net analog ILog.Warn(object, exception) - trace deals in strings for
        /// the main message, the exception parameter is actually a parameter array
        /// accepting many types, upto and including exception types for times when the developer
        /// wants to log an exception but not have it be recorded as an exception by higher level
        /// processes that watch log output. 
        /// </summary>
        /// <param name="trace">current TraceSource instance</param>
        /// <param name="id">System.Diagnostics message ID</param>
        /// <param name="format">formatted message string <see cref="http://msdn.microsoft.com/en-us/library/txafckwd.aspx"/></param>
        /// <param name="args">data argument array</param>
        public static void Warn(this TraceSource trace, int id, string format, params object[] args)
        {
            trace.TraceEvent(TraceEventType.Warning, id, format, args);
        }

        /// <summary>
        /// log4net analog ILog.Error(object, exception) - trace deals in strings for
        /// the main message, the exception parameter is actually a parameter array
        /// accepting many types, upto and including exception types for times when the developer
        /// wants to log an exception but not have it be recorded as an exception by higher level
        /// processes that watch log output. 
        /// </summary>
        /// <param name="trace">current TraceSource instance</param>
        /// <param name="format">formatted message string <see cref="http://msdn.microsoft.com/en-us/library/txafckwd.aspx"/></param>
        /// <param name="args">data argument array</param>
        public static void Error(this TraceSource trace, string format, params object[] args)
        {
            trace.TraceEvent(TraceEventType.Error, -1, format, args);
        }

        /// <summary>
        /// log4net analog ILog.Error(object, exception) - trace deals in strings for
        /// the main message, the exception parameter is actually a parameter array
        /// accepting many types, upto and including exception types for times when the developer
        /// wants to log an exception but not have it be recorded as an exception by higher level
        /// processes that watch log output. 
        /// </summary>
        /// <param name="trace">current TraceSource instance</param>
        /// <param name="id">System.Diagnostics message ID</param>
        /// <param name="format">formatted message string <see cref="http://msdn.microsoft.com/en-us/library/txafckwd.aspx"/></param>
        /// <param name="args">data argument array</param>
        public static void Error(this TraceSource trace, int id, string format, params object[] args)
        {
            trace.TraceEvent(TraceEventType.Error, id, format, args);
        }

        /// <summary>
        /// log4net analog ILog.Fatal(object, exception) - trace deals in strings for
        /// the main message, the exception parameter is actually a parameter array
        /// accepting many types, upto and including exception types for times when the developer
        /// wants to log an exception but not have it be recorded as an exception by higher level
        /// processes that watch log output. 
        /// </summary>
        /// <param name="trace">current TraceSource instance</param>
        /// <param name="format">formatted message string <see cref="http://msdn.microsoft.com/en-us/library/txafckwd.aspx"/></param>
        /// <param name="args">data argument array</param>
        public static void Fatal(this TraceSource trace, string format, params object[] args)
        {
            trace.TraceEvent(TraceEventType.Critical, -1, format, args);
        }

        /// <summary>
        /// log4net analog ILog.Fatal(object, exception) - trace deals in strings for
        /// the main message, the exception parameter is actually a parameter array
        /// accepting many types, upto and including exception types for times when the developer
        /// wants to log an exception but not have it be recorded as an exception by higher level
        /// processes that watch log output. 
        /// </summary>
        /// <param name="trace">current TraceSource instance</param>
        /// <param name="id">System.Diagnostics message ID</param>
        /// <param name="format">formatted message string <see cref="http://msdn.microsoft.com/en-us/library/txafckwd.aspx"/></param>
        /// <param name="args">data argument array</param>
        public static void Fatal(this TraceSource trace, int id, string format, params object[] args)
        {
            trace.TraceEvent(TraceEventType.Critical, id, format, args);
        }

        /// <summary>
        /// log4net analog - ILog.IsDebugEnabled - Checks if this trace is enabled for the <see cref="System.Diagnostics.TraceEventType.Debug"/> level
        /// </summary>
        /// <param name="trace">current trace</param>
        /// <returns>true=DEBUG LOGGING ENABLED;false=DEBUG LOGGING DISABLED</returns>
        public static bool IsDebugEnabled(this TraceSource trace)
        {
            return trace.Switch.ShouldTrace(TraceEventType.Verbose);
        }

        /// <summary>
        /// log4net analog - ILog.IsErrorEnabled - Checks if this trace is enabled for the <see cref="System.Diagnostics.TraceEventType.Error"/> level
        /// </summary>
        /// <param name="trace">current trace</param>
        /// <returns>true=Fatal LOGGING ENABLED;false=Fatal LOGGING DISABLED</returns>
        public static bool IsErrorEnabled(this TraceSource trace)
        {
            return trace.Switch.ShouldTrace(TraceEventType.Error);
        }

        /// <summary>
        /// log4net analog - ILog.IsFatalEnabled - Checks if this trace is enabled for the <see cref="System.Diagnostics.TraceEventType.Critical"/> level
        /// </summary>
        /// <param name="trace">current trace</param>
        /// <returns>true=Fatal LOGGING ENABLED;false=Fatal LOGGING DISABLED</returns>
        public static bool IsFatalEnabled(this TraceSource trace)
        {
            return trace.Switch.ShouldTrace(TraceEventType.Critical);
        }

        /// <summary>
        /// log4net analog - ILog.IsInfoEnabled - Checks if this trace is enabled for the <see cref="System.Diagnostics.TraceEventType.Information"/> level
        /// </summary>
        /// <param name="trace">current trace</param>
        /// <returns>true=Info LOGGING ENABLED;false=Info LOGGING DISABLED</returns>
        public static bool IsInfoEnabled(this TraceSource trace)
        {
            return trace.Switch.ShouldTrace(TraceEventType.Information);
        }

        /// <summary>
        /// log4net analog - ILog.IsWarnEnabled - Checks if this trace is enabled for the <see cref="System.Diagnostics.TraceEventType.Warn"/> level
        /// </summary>
        /// <param name="trace">current trace</param>
        /// <returns>true=Warn LOGGING ENABLED;false=Warn LOGGING DISABLED</returns>
        public static bool IsWarnEnabled(this TraceSource trace)
        {
            return trace.Switch.ShouldTrace(TraceEventType.Warning);
        }
        #endregion
    }
}
