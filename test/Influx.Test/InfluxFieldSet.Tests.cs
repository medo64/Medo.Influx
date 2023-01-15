using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Net.Influx;

namespace Tests;

[TestClass]
public class InfluxFieldSet_Tests {

    [TestMethod]
    public void InfluxFieldSet_Empty() {
        var set = new InfluxFieldSet();
        Assert.AreEqual(0, set.Count);
    }

    [TestMethod]
    public void InfluxFieldSet_Basic() {
        var set = new InfluxFieldSet {
                new InfluxField("A", "1"),
                new InfluxField("B", "2")
            };
        Assert.AreEqual(2, set.Count);
        Assert.AreEqual("A", set[0].Key);
        Assert.AreEqual("1", set[0].Value);
        Assert.AreEqual("B", set[1].Key);
        Assert.AreEqual("2", set[1].Value);
    }

    [TestMethod]
    public void InfluxFieldSet_Sorting() {
        var set = new InfluxFieldSet {
                new InfluxField("B", "2"),
                new InfluxField("A", "1")
            };
        Assert.AreEqual(2, set.Count);
        Assert.AreEqual("A", set[0].Key);
        Assert.AreEqual("1", set[0].Value);
        Assert.AreEqual("B", set[1].Key);
        Assert.AreEqual("2", set[1].Value);
    }

    [TestMethod]
    public void InfluxFieldSet_AddDuplicate() {
        var set = new InfluxFieldSet {
                new InfluxField("B", "2"),
                new InfluxField("A", "1")
            };
        Assert.ThrowsException<ArgumentException>(delegate {
            set.Add(new InfluxField("A", "11"));
        });
    }


    [TestMethod]
    public void InfluxFieldSet_Invalid() {
        var set = new InfluxFieldSet {
                new InfluxField("B", "2"),
                new InfluxField("A", "1")
            };
        Assert.ThrowsException<ArgumentNullException>(delegate {
            set.Add(null);
        });
    }

}
