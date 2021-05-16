using System;
using Xunit;

namespace Tests.InfluxMeasurement {
    using Medo.Net.Influx;

    public class Tests {

        [Fact(DisplayName = "InfluxMeasurement: Basic")]
        public void Basic() {
            var data = new InfluxMeasurement("Test", DateTime.MaxValue);
            data.Fields.Add(new InfluxField("X", 42));
            Assert.Equal("Test", data.Name);
            Assert.Equal(3155378975999999999, data.Timestamp.Ticks);
            Assert.Equal("Test X=42i 253402300799999999900", data.ToString());
            Assert.Equal("Test X=42i 253402300799999999900", data.ToString(InfluxProtocolVersion.V2, InfluxTimestampResolution.Nanoseconds));
            Assert.Equal("Test X=42i 253402300799999999", data.ToString(InfluxProtocolVersion.V2, InfluxTimestampResolution.Microseconds));
            Assert.Equal("Test X=42i 253402300799999", data.ToString(InfluxProtocolVersion.V2, InfluxTimestampResolution.Miliseconds));
            Assert.Equal("Test X=42i 253402300799", data.ToString(InfluxProtocolVersion.V2, InfluxTimestampResolution.Seconds));
        }

        [Fact(DisplayName = "InfluxMeasurement: Escaping name")]
        public void EscapingName() {
            var fields = new InfluxFieldSet { new InfluxField("Key", 42.0) };
            Assert.Equal(@"Name\ With\ Spaces Key=42", new InfluxMeasurement("Name With Spaces", fields).ToString(InfluxProtocolVersion.V2, InfluxTimestampResolution.None));
            Assert.Equal(@"NameWith\,Comma Key=42", new InfluxMeasurement("NameWith,Comma", fields).ToString(InfluxProtocolVersion.V2, InfluxTimestampResolution.None));
            Assert.Equal(@"NameWith=Equals Key=42", new InfluxMeasurement("NameWith=Equals", fields).ToString(InfluxProtocolVersion.V2, InfluxTimestampResolution.None));  // no need to escape equals
            Assert.Equal(@"NameWith\Backslash Key=42", new InfluxMeasurement(@"NameWith\Backslash", fields).ToString(InfluxProtocolVersion.V2, InfluxTimestampResolution.None));  // no need to escape backslash
            Assert.Equal(@"NameWith""Quote Key=42", new InfluxMeasurement(@"NameWith""Quote", fields).ToString(InfluxProtocolVersion.V2, InfluxTimestampResolution.None));  // no need to escape quote
        }

        [Fact(DisplayName = "InfluxMeasurement: Invalid name")]
        public void InvalidName() {
            Assert.Throws<ArgumentNullException>(delegate {
                var _ = new InfluxMeasurement(null);
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                var _ = new InfluxMeasurement("");
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                var _ = new InfluxMeasurement(" ");
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                var _ = new InfluxMeasurement("A\n");
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                var _ = new InfluxMeasurement("A\t");
            });
        }

    }
}
