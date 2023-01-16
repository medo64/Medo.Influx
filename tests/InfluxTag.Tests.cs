using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Net.Influx;

namespace Tests;

[TestClass]
public class InfluxTag_Tests {

    [TestMethod]
    public void InfluxTag_Basic() {
        var x = new InfluxTag("Key", "Value");
        Assert.AreEqual("Key", x.Key);
        Assert.AreEqual("Value", x.Value);
        Assert.AreEqual("Key=Value", x.ToString());
    }

    [TestMethod]
    public void InfluxTag_EscapingKey() {
        Assert.AreEqual(@"Key\ With\ Spaces=", new InfluxTag("Key With Spaces", "").ToString());
        Assert.AreEqual(@"KeyWith\,Comma=", new InfluxTag("KeyWith,Comma", "").ToString());
        Assert.AreEqual(@"KeyWith\=Equals=", new InfluxTag("KeyWith=Equals", "").ToString());
        Assert.AreEqual(@"KeyWith\Backslash=", new InfluxTag(@"KeyWith\Backslash", "").ToString());  // no need to escape backslash
        Assert.AreEqual(@"KeyWith""Quote=", new InfluxTag(@"KeyWith""Quote", "").ToString());  // no need to escape quote
    }

    [TestMethod]
    public void InfluxTag_EscapingValue() {
        Assert.AreEqual(@"Key=Value\ With\ Spaces", new InfluxTag("Key", "Value With Spaces").ToString());
        Assert.AreEqual(@"Key=ValueWith\,Comma", new InfluxTag("Key", "ValueWith,Comma").ToString());
        Assert.AreEqual(@"Key=ValueWith\=Equals", new InfluxTag("Key", "ValueWith=Equals").ToString());
        Assert.AreEqual(@"Key=ValueWith\Backslash", new InfluxTag("Key", @"ValueWith\Backslash").ToString());  // no need to escape backslash
        Assert.AreEqual(@"Key=ValueWith""Quote", new InfluxTag("Key", @"ValueWith""Quote").ToString());  // no need to escape quote
    }

    [TestMethod]
    public void InfluxTag_InvalidKey() {
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var _ = new InfluxTag(null, "Value");
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var _ = new InfluxTag("", "Value");
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var _ = new InfluxTag(" ", "Value");
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var _ = new InfluxTag("time", "Value");
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var _ = new InfluxTag("A\n", "Value");
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var _ = new InfluxTag("A\t", "Value");
        });
    }

    [TestMethod]
    public void InfluxTag_InvalidValue() {
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var _ = new InfluxTag("Key", null);
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var _ = new InfluxTag("Key", "A\n");
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var _ = new InfluxTag("Key", "A\t");
        });
    }

}
