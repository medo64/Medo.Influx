using System;
using Xunit;

namespace Tests.InfluxTagSet {
    using Medo.Net.Influx;

    public class Tests {

        [Fact(DisplayName = "InfluxTagSet: Empty")]
        public void Empty() {
            var set = new InfluxTagSet();
            Assert.Equal(0, set.Count);
        }

        [Fact(DisplayName = "InfluxTagSet: Basic")]
        public void Basic() {
            var set = new InfluxTagSet {
                new InfluxTag("A", "1"),
                new InfluxTag("B", "2")
            };
            Assert.Equal(2, set.Count);
            Assert.Equal("A", set[0].Key);
            Assert.Equal("1", set[0].Value);
            Assert.Equal("B", set[1].Key);
            Assert.Equal("2", set[1].Value);
        }

        [Fact(DisplayName = "InfluxTagSet: Sorting")]
        public void Sorting() {
            var set = new InfluxTagSet {
                new InfluxTag("B", "2"),
                new InfluxTag("A", "1")
            };
            Assert.Equal(2, set.Count);
            Assert.Equal("A", set[0].Key);
            Assert.Equal("1", set[0].Value);
            Assert.Equal("B", set[1].Key);
            Assert.Equal("2", set[1].Value);
        }

        [Fact(DisplayName = "InfluxTagSet: Add duplicate")]
        public void AddDuplicate() {
            var set = new InfluxTagSet {
                new InfluxTag("B", "2"),
                new InfluxTag("A", "1")
            };
            Assert.Throws<ArgumentException>(delegate {
                set.Add(new InfluxTag("A", "11"));
            });
        }


        [Fact(DisplayName = "InfluxTagSet: Add invalid")]
        public void Invalid() {
            var set = new InfluxTagSet {
                new InfluxTag("B", "2"),
                new InfluxTag("A", "1")
            };
            Assert.Throws<ArgumentNullException>(delegate {
                set.Add(null);
            });
        }

    }
}
