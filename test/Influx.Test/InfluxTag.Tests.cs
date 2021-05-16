using System;
using Xunit;

namespace Tests.InfluxTag {
    using Medo.Net.Influx;

    public class Tests {

        [Fact(DisplayName = "InfluxTag: Basic")]
        public void Basic() {
            var x = new InfluxTag("Key", "Value");
            Assert.Equal("Key", x.Key);
            Assert.Equal("Value", x.Value);
            Assert.Equal("Key=Value", x.ToString());
        }

        [Fact(DisplayName = "InfluxTag: Escaping key")]
        public void EscapingKey() {
            Assert.Equal(@"Key\ With\ Spaces=", new InfluxTag("Key With Spaces", "").ToString());
            Assert.Equal(@"KeyWith\,Comma=", new InfluxTag("KeyWith,Comma", "").ToString());
            Assert.Equal(@"KeyWith\=Equals=", new InfluxTag("KeyWith=Equals", "").ToString());
            Assert.Equal(@"KeyWith\Backslash=", new InfluxTag(@"KeyWith\Backslash", "").ToString());  // no need to escape backslash
            Assert.Equal(@"KeyWith""Quote=", new InfluxTag(@"KeyWith""Quote", "").ToString());  // no need to escape quote
        }

        [Fact(DisplayName = "InfluxTag: Escaping value")]
        public void EscapingValue() {
            Assert.Equal(@"Key=Value\ With\ Spaces", new InfluxTag("Key", "Value With Spaces").ToString());
            Assert.Equal(@"Key=ValueWith\,Comma", new InfluxTag("Key", "ValueWith,Comma").ToString());
            Assert.Equal(@"Key=ValueWith\=Equals", new InfluxTag("Key", "ValueWith=Equals").ToString());
            Assert.Equal(@"Key=ValueWith\Backslash", new InfluxTag("Key", @"ValueWith\Backslash").ToString());  // no need to escape backslash
            Assert.Equal(@"Key=ValueWith""Quote", new InfluxTag("Key", @"ValueWith""Quote").ToString());  // no need to escape quote
        }

        [Fact(DisplayName = "InfluxTag: Invalid key")]
        public void InvalidKey() {
            Assert.Throws<ArgumentNullException>(delegate {
                var _ = new InfluxTag(null, "Value");
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                var _ = new InfluxTag("", "Value");
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                var _ = new InfluxTag(" ", "Value");
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                var _ = new InfluxTag("time", "Value");
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                var _ = new InfluxTag("A\n", "Value");
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                var _ = new InfluxTag("A\t", "Value");
            });
        }

        [Fact(DisplayName = "InfluxTag: Invalid value")]
        public void InvalidValue() {
            Assert.Throws<ArgumentNullException>(delegate {
                var _ = new InfluxTag("Key", null);
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                var _ = new InfluxTag("Key", "A\n");
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                var _ = new InfluxTag("Key", "A\t");
            });
        }

    }
}
