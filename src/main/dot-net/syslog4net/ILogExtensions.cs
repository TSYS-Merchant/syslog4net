using System;
using log4net;

namespace MerchantWarehouse.Diagnostics
{
    /// <summary>
    /// Helper methods/extensions for leveraging log4net types within MerchantWarehouse
    /// </summary>
    // ReSharper disable once InconsistentNaming (This class exists only to provide extension methods to the ILog interface.)
    public static class ILogExtensions
    // ReSharper disable
    {
        public const string DefaultStackName = "mwas"; //MerchantWarehouseActivityStack

        /// <summary>
        /// Returns a stack context object for correlation logging
        /// </summary>
        /// <see cref="http://logging.apache.org/log4net/release/manual/contexts.html"/>
        /// <param name="log">current <see cref="ILog"/> instance</param>
        /// <param name="stackName">(optional) stack name to use</param>
        /// <param name="activityId">(optional) activity id DEFAULT = Guid.NewGuid()</param>
        /// <returns>pushed stack context object. Context is popped from stack when object is disposed.</returns>
        public static IDisposable StartThreadActivity(this ILog log, string stackName = null, string activityId = null)
        {
            var name = string.IsNullOrEmpty(stackName) ? DefaultStackName : stackName;
            var id = string.IsNullOrEmpty(activityId) ? Guid.NewGuid().ToString() : activityId;
            
            return ThreadContext.Stacks[name].Push(id);
        }

        /// <summary>
        /// Returns a stack context object for correlation logging
        /// </summary>
        /// <see cref="http://logging.apache.org/log4net/release/manual/contexts.html"/>
        /// <param name="log">current <see cref="ILog"/> instance</param>
        /// <param name="stackName">(optional) stack name to use</param>
        /// <param name="activityId">(optional) activity id DEFAULT = Guid.NewGuid()</param>
        /// <returns>pushed stack context object. Context is popped from stack when object is disposed.</returns>
        public static IDisposable StartThreadLogicialActivity(this ILog log, string stackName = null, string activityId = null)
        {
            var name = string.IsNullOrEmpty(stackName) ? DefaultStackName : stackName;
            var id = string.IsNullOrEmpty(activityId) ? Guid.NewGuid().ToString() : activityId;

            return LogicalThreadContext.Stacks[name].Push(id);
        }

        /// <summary>
        /// Returns a stack context object for the NDC stack using the provided ID or a randomly generated GUID. Similar to StartThread and StartThreadLogicial activity but is hard-coded to the NDC stack.
        /// </summary>
        /// <param name="log">current <see cref="ILog"/> instance</param>
        /// <param name="id">(optional) message id DEFAULT = Guid.NewGuid()</param>
        /// <returns>pushed stack context object. Context is popped from stack when object is disposed.</returns>
        public static IDisposable StartMessage(this ILog log, string id = null)
        {
            id = string.IsNullOrEmpty(id) ? Guid.NewGuid().ToString() : id;

            return ThreadContext.Stacks["NDC"].Push(id);
        }
    }
}
