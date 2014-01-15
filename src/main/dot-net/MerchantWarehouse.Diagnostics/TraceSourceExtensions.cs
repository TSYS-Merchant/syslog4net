using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerchantWarehouse.Diagnostics
{
    public static class TraceSourceExtensions
    {
        /*
        private static TraceSource exceptionTrace = new TraceSource("ExceptionTrace");

        public static void FlushAll(this TraceSource trace)
        {
            exceptionTrace.Flush();
        }*/

        public static void Debug(this TraceSource trace, int id, string format, params object[] args)
        {
            trace.TraceEvent(TraceEventType.Verbose, id, format, args);
        }

        public static void Debug(this TraceSource trace, int id, params object[] data)
        {
            trace.TraceData(TraceEventType.Verbose, id, data);
        }

        public static void Debug(this TraceSource trace, int id, Exception exception, params object[] data)
        {
            trace.TraceData(TraceEventType.Verbose, id, exception, data);
            //exceptionTrace.TraceData(TraceEventType.Verbose, id, exception, data);
        }

        public static void Info(this TraceSource trace, string format, params object[] args)
        {
            trace.TraceEvent(TraceEventType.Information, -1, format, args);
        }

        public static void Info(this TraceSource trace, int id, string format, params object[] args)
        {
            trace.TraceEvent(TraceEventType.Information, id, format, args);
        }

        public static void Info(this TraceSource trace, int id, params object[] data)
        {
            trace.TraceData(TraceEventType.Information, id, data);
        }

        public static void Info(this TraceSource trace, int id, Exception exception, params object[] data)
        {
            trace.TraceData(TraceEventType.Information, id, exception, data);
            //exceptionTrace.TraceData(TraceEventType.Information, id, exception, data);
        }

        public static void Warn(this TraceSource trace, string format, params object[] args)
        {
            trace.TraceEvent(TraceEventType.Warning, -1, format, args);
        }

        public static void Warn(this TraceSource trace, int id, string format, params object[] args)
        {
            trace.TraceEvent(TraceEventType.Warning, id, format, args);
        }

        public static void Warn(this TraceSource trace, int id, params object[] data)
        {
            trace.TraceData(TraceEventType.Warning, id, data);
        }

        public static void Warn(this TraceSource trace, int id, Exception exception, params object[] data)
        {
            trace.TraceData(TraceEventType.Warning, id, exception, data);
            //exceptionTrace.TraceData(TraceEventType.Warning, id, exception, data);
        }

        public static void Error(this TraceSource trace, string format, params object[] args)
        {
            trace.TraceEvent(TraceEventType.Error, -1, format, args);
        }

        public static void Error(this TraceSource trace, int id, string format, params object[] args)
        {
            trace.TraceEvent(TraceEventType.Error, id, format, args);
        }

        public static void Error(this TraceSource trace, int id, params object[] data)
        {
            trace.TraceData(TraceEventType.Error, id, data);
        }

        public static void Error(this TraceSource trace, int id, Exception exception, params object[] data)
        {
            trace.TraceData(TraceEventType.Error, id, exception, data);
            //exceptionTrace.TraceData(TraceEventType.Error, id, exception, data);
        }

        public static void Fatal(this TraceSource trace, string format, params object[] args)
        {
            trace.TraceEvent(TraceEventType.Critical, -1, format, args);
        }

        public static void Fatal(this TraceSource trace, int id, string format, params object[] args)
        {
            trace.TraceEvent(TraceEventType.Critical, id, format, args);
        }

        public static void Fatal(this TraceSource trace, int id, params object[] data)
        {
            trace.TraceData(TraceEventType.Critical, id, data);
        }

        public static void Fatal(this TraceSource trace, int id, Exception exception, params object[] data)
        {
            trace.TraceData(TraceEventType.Critical, id, exception, data);
            //exceptionTrace.TraceData(TraceEventType.Critical, id, exception, data);
        }
    }
}
