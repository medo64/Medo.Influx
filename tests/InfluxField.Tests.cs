using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Net.Influx;

namespace Tests;

[TestClass]
public class InfluxField_Tests {

    [TestMethod]
    public void InfluxField_BasicFloat() {
        var x = new InfluxField("Key", 42.43);
        Assert.AreEqual("Key", x.Key);
        Assert.AreEqual(42.43, x.Value);
        Assert.AreEqual("Key=42.43", x.ToString());
    }

    [TestMethod]
    public void InfluxField_BasicInteger() {
        var x = new InfluxField("Key", -42);
        Assert.AreEqual("Key", x.Key);
        Assert.AreEqual(-42L, x.Value);
        Assert.AreEqual("Key=-42i", x.ToString());
    }

    [TestMethod]
    public void InfluxField_BasicUInteger() {
        var x = new InfluxField("Key", 42UL);
        Assert.AreEqual("Key", x.Key);
        Assert.AreEqual(42UL, x.Value);
        Assert.AreEqual("Key=42u", x.ToString());
    }

    [TestMethod]
    public void InfluxField_BasicUIntegerV1() {
        var x = new InfluxField("Key", 9223372036854775808UL);
        Assert.AreEqual("Key", x.Key);
        Assert.AreEqual(9223372036854775808UL, x.Value);
        Assert.AreEqual("Key=9223372036854775807i", x.ToString(InfluxProtocolVersion.V1));
    }

    [TestMethod]
    public void InfluxField_BasicUIntegerV1Strict() {
        Assert.ThrowsException<InvalidOperationException>(delegate {
            var x = new InfluxField("Key", 9223372036854775808);
            x.ToString(InfluxProtocolVersion.V1Strict);
        });
    }

    [TestMethod]
    public void InfluxField_BasicUIntegerV2() {
        var x = new InfluxField("Key", 9223372036854775808);
        Assert.AreEqual("Key", x.Key);
        Assert.AreEqual(9223372036854775808UL, x.Value);
        Assert.AreEqual("Key=9223372036854775808u", x.ToString(InfluxProtocolVersion.V2));
    }

    [TestMethod]
    public void InfluxField_BasicString() {
        var x = new InfluxField("Key", "Value");
        Assert.AreEqual("Key", x.Key);
        Assert.AreEqual("Value", x.Value);
        Assert.AreEqual("Key=\"Value\"", x.ToString());
    }

    [TestMethod]
    public void InfluxField_BasicBoolean() {
        {
            var x = new InfluxField("Key", false);
            Assert.AreEqual("Key", x.Key);
            Assert.AreEqual(false, x.Value);
            Assert.AreEqual("Key=false", x.ToString());
        }
        {
            var x = new InfluxField("Key", true);
            Assert.AreEqual("Key", x.Key);
            Assert.AreEqual(true, x.Value);
            Assert.AreEqual("Key=true", x.ToString());
        }
    }


    [TestMethod]
    public void InfluxField_EscapingKey() {
        Assert.AreEqual(@"Key\ With\ Spaces=42", new InfluxField("Key With Spaces", 42.0).ToString());
        Assert.AreEqual(@"KeyWith\,Comma=42", new InfluxField("KeyWith,Comma", 42.0).ToString());
        Assert.AreEqual(@"KeyWith\=Equals=42", new InfluxField("KeyWith=Equals", 42.0).ToString());
        Assert.AreEqual(@"KeyWith\Backslash=42", new InfluxField(@"KeyWith\Backslash", 42.0).ToString());  // no need to escape backslash
        Assert.AreEqual(@"KeyWith""Quote=42", new InfluxField(@"KeyWith""Quote", 42.0).ToString());  // no need to escape quote
    }

    [TestMethod]
    public void InfluxField_EscapingValue() {
        Assert.AreEqual(@"Key=""Value With Spaces""", new InfluxField("Key", "Value With Spaces").ToString());
        Assert.AreEqual(@"Key=""ValueWith,Comma""", new InfluxField("Key", "ValueWith,Comma").ToString());
        Assert.AreEqual(@"Key=""ValueWith=Equals""", new InfluxField("Key", "ValueWith=Equals").ToString());
        Assert.AreEqual(@"Key=""ValueWith\\Backslash""", new InfluxField("Key", @"ValueWith\Backslash").ToString());  // no need to escape backslash
        Assert.AreEqual(@"Key=""ValueWith\""Quote""", new InfluxField("Key", @"ValueWith""Quote").ToString());  // no need to escape quote
    }

    [TestMethod]
    public void InfluxField_InvalidKey() {
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var _ = new InfluxField(null, "Value");
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var _ = new InfluxField("", "Value");
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var _ = new InfluxField(" ", "Value");
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var _ = new InfluxField("A\n", "Value");
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var _ = new InfluxField("A\t", "Value");
        });
    }

    [TestMethod]
    public void InfluxField_InvalidValue() {
        Assert.ThrowsException<ArgumentNullException>(delegate {
            var _ = new InfluxField("Key", null);
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var _ = new InfluxField("Key", "A\n");
        });
        Assert.ThrowsException<ArgumentOutOfRangeException>(delegate {
            var _ = new InfluxField("Key", "A\t");
        });
    }

}
