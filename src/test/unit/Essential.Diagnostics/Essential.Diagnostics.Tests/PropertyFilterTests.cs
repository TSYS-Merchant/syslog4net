using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NUnit.Framework; 

namespace Essential.Diagnostics.Tests
{
   // [TestFixture]no need to test obsolete members since we are not going to modify them.
    //public class PropertyFilterTests
    //{
    //    [Test]
    //    public void ShouldAllowValidTrace()
    //    {
    //        var filter = new PropertyFilter("id == 1");

    //        var shouldTrace = filter.ShouldTrace(null, "Source", TraceEventType.Information, 1, "Message", null, null, null);

    //        Assert.IsTrue(shouldTrace);
    //    }

    //    [Test]
    //    public void ShouldBlockInvalidTrace()
    //    {
    //        var filter = new PropertyFilter("id == 1");

    //        var shouldTrace = filter.ShouldTrace(null, "Source", TraceEventType.Information, 2, "Message", null, null, null);

    //        Assert.IsFalse(shouldTrace);
    //    }


    //    [Test]
    //    public void FilterOnSingleDataItemShouldWork()
    //    {
    //        var filter = new PropertyFilter("data == 'A'");

    //        var shouldTrace1 = filter.ShouldTrace(null, "Source", TraceEventType.Information, 1, "Message", null, "A", null);
    //        var shouldTrace2 = filter.ShouldTrace(null, "Source", TraceEventType.Information, 1, "Message", null, "B", null);

    //        Assert.IsTrue(shouldTrace1);
    //        Assert.IsFalse(shouldTrace2);
    //    }
    //}
}
