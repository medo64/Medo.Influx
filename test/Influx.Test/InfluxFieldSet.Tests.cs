using System;
using Xunit;

namespace Tests.InfluxFieldSet {
    using Medo.Net.Influx;

    public class Tests {

        [Fact(DisplayName = "InfluxFieldSet: Empty")]
        public void Empty() {
            var set = new InfluxFieldSet();
            Assert.Equal(0, set.Count);
        }

        [Fact(DisplayName = "InfluxFieldSet: Basic")]
        public void Basic() {
            var set = new InfluxFieldSet {
                new InfluxField("A", "1"),
                new InfluxField("B", "2")
            };
            Assert.Equal(2, set.Count);
            Assert.Equal("A", set[0].Key);
            Assert.Equal("1", set[0].Value);
            Assert.Equal("B", set[1].Key);
            Assert.Equal("2", set[1].Value);
        }

        [Fact(DisplayName = "InfluxFieldSet: Sorting")]
        public void Sorting() {
            var set = new InfluxFieldSet {
                new InfluxField("B", "2"),
                new InfluxField("A", "1")
            };
            Assert.Equal(2, set.Count);
            Assert.Equal("A", set[0].Key);
            Assert.Equal("1", set[0].Value);
            Assert.Equal("B", set[1].Key);
            Assert.Equal("2", set[1].Value);
        }

        [Fact(DisplayName = "InfluxFieldSet: Add duplicate")]
        public void AddDuplicate() {
            var set = new InfluxFieldSet {
                new InfluxField("B", "2"),
                new InfluxField("A", "1")
            };
            Assert.Throws<ArgumentException>(delegate {
                set.Add(new InfluxField("A", "11"));
            });
        }


        [Fact(DisplayName = "InfluxFieldSet: Add invalid")]
        public void Invalid() {
            var set = new InfluxFieldSet {
                new InfluxField("B", "2"),
                new InfluxField("A", "1")
            };
            Assert.Throws<ArgumentNullException>(delegate {
                set.Add(null);
            });
        }

    }
}
