using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Medo.Net.Influx;

namespace Tests;

[TestClass]
public class InfluxTagSet_Tests {

    [TestMethod]
    public void InfluxTagSet_Empty() {
        var set = new InfluxTagSet();
        Assert.AreEqual(0, set.Count);
    }

    [TestMethod]
    public void InfluxTagSet_Basic() {
        var set = new InfluxTagSet {
                new InfluxTag("A", "1"),
                new InfluxTag("B", "2")
            };
        Assert.AreEqual(2, set.Count);
        Assert.AreEqual("A", set[0].Key);
        Assert.AreEqual("1", set[0].Value);
        Assert.AreEqual("B", set[1].Key);
        Assert.AreEqual("2", set[1].Value);
    }

    [TestMethod]
    public void InfluxTagSet_Sorting() {
        var set = new InfluxTagSet {
                new InfluxTag("B", "2"),
                new InfluxTag("A", "1")
            };
        Assert.AreEqual(2, set.Count);
        Assert.AreEqual("A", set[0].Key);
        Assert.AreEqual("1", set[0].Value);
        Assert.AreEqual("B", set[1].Key);
        Assert.AreEqual("2", set[1].Value);
    }

    [TestMethod]
    public void InfluxTagSet_AddDuplicate() {
        var set = new InfluxTagSet {
                new InfluxTag("B", "2"),
                new InfluxTag("A", "1")
            };
        Assert.ThrowsException<ArgumentException>(delegate {
            set.Add(new InfluxTag("A", "11"));
        });
    }


    [TestMethod]
    public void InfluxTagSet_Invalid() {
        var set = new InfluxTagSet {
                new InfluxTag("B", "2"),
                new InfluxTag("A", "1")
            };
        Assert.ThrowsException<ArgumentNullException>(delegate {
            set.Add(null);
        });
    }

}
