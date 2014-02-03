using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerchantWarehouse.Diagnostics
{
    /// <summary>
    /// Helper methods/extensions for leveraging log4net types within MerchantWarehouse
    /// </summary>
    public static class ILogExtensions
    {
        public const string DEFAULT_STACK_NAME = "mwas"; //MerchantWarehouseActivityStack

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
            var name = string.IsNullOrEmpty(stackName) ? DEFAULT_STACK_NAME : stackName;
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
            var name = string.IsNullOrEmpty(stackName) ? DEFAULT_STACK_NAME : stackName;
            var id = string.IsNullOrEmpty(activityId) ? Guid.NewGuid().ToString() : activityId;
            
            return LogicalThreadContext.Stacks[name].Push(id);
        }
    }
}
