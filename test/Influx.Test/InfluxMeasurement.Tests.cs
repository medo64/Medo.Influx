using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Net.Influx;

namespace Tests;

[TestClass]
public class InfluxMeasurement_Tests {

    [TestMethod]
    public void InfluxMeasurement_Basic() {
        var data = new InfluxMeasurement("Test", DateTime.MaxValue);
        data.Fields.Add(new InfluxField("X", 42));
        Assert.AreEqual("Test", data.Name);
        Assert.AreEqual(3155378975999999999, data.Timestamp.Ticks);
        Assert.AreEqual("Test X=42i 253402300799999999900", data.ToString());
        Assert.AreEqual("Test X=42i 253402300799999999900", data.ToString(InfluxProtocolVersion.V2, InfluxTimestampResolution.Nanoseconds));
        Assert.AreEqual("Test X=42i 253402300799999999", data.ToString(InfluxProtocolVersion.V2, InfluxTimestampResolution.Microseconds));
        Assert.AreEqual("Test X=42i 253402300799999", data.ToString(InfluxProtocolVersion.V2, InfluxTimestampResolution.Miliseconds));
        Assert.AreEqual("Test X=42i 253402300799", data.ToString(InfluxProtocolVersion.V2, InfluxTimestampResolution.Seconds));
    }

    [TestMethod]
    public void InfluxMeasurement_EscapingName() {
        var fields = new InfluxFieldSet { new InfluxField("Key", 42.0) };
        Assert.AreEqual(@"Name\ With\ Spaces Key=42", new InfluxMeasurement("Name With Spaces", fields).ToString(InfluxProtocolVersion.V2, InfluxTimestampResolution.None));
        Assert.AreEqual(@"NameWith\,Comma Key=42", new InfluxMeasurement("NameWith,Comma", fields).ToString(InfluxProtocolVersion.V2, InfluxTimestampResolution.None));
        Assert.AreEqual(@"NameWith=Equals Key=42", new InfluxMeasurement("NameWith=Equals", fields).ToString(InfluxProtocolVersion.V2, InfluxTimestampResolution.None));  // no need to escape equals
        Assert.AreEqual(@"NameWith\Backslash Key=42", new InfluxMeasurement(@"NameWith\Backslash", fields).ToString(InfluxProtocolVersion.V2, InfluxTimestampResolution.None));  // no need to escape backslash
        Assert.AreEqual(@"NameWith""Quote Key=42", new InfluxMeasurement(@"NameWith""Quote", fields).ToString(InfluxProtocolVersion.V2, InfluxTimestampResolution.None));  // no need to escape quote
    }

    [TestMethod]
    public void InfluxMeasurement_InvalidName() {
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var _ = new InfluxMeasurement(null);
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var _ = new InfluxMeasurement("");
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var _ = new InfluxMeasurement(" ");
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var _ = new InfluxMeasurement("A\n");
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var _ = new InfluxMeasurement("A\t");
        });
    }

}
