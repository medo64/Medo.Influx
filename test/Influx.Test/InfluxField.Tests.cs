using System;
using Xunit;

namespace Tests.InfluxField {
    using Medo.Net.Influx;

    public class Tests {

        [Fact(DisplayName = "InfluxField: Basic (float)")]
        public void BasicFloat() {
            var x = new InfluxField("Key", 42.43);
            Assert.Equal("Key", x.Key);
            Assert.Equal(42.43, x.Value);
            Assert.Equal("Key=42.43", x.ToString());
        }

        [Fact(DisplayName = "InfluxField: Basic (integer)")]
        public void BasicInteger() {
            var x = new InfluxField("Key", -42);
            Assert.Equal("Key", x.Key);
            Assert.Equal(-42L, x.Value);
            Assert.Equal("Key=-42i", x.ToString());
        }

        [Fact(DisplayName = "InfluxField: Basic (uinteger)")]
        public void BasicUInteger() {
            var x = new InfluxField("Key", 42UL);
            Assert.Equal("Key", x.Key);
            Assert.Equal(42UL, x.Value);
            Assert.Equal("Key=42u", x.ToString());
        }

        [Fact(DisplayName = "InfluxField: Conversion (v1 uinteger)")]
        public void BasicUIntegerV1() {
            var x = new InfluxField("Key", 9223372036854775808UL);
            Assert.Equal("Key", x.Key);
            Assert.Equal(9223372036854775808UL, x.Value);
            Assert.Equal("Key=9223372036854775807i", x.ToString(InfluxProtocolVersion.V1));
        }

        [Fact(DisplayName = "InfluxField: Conversion (v1-strict uinteger)")]
        public void BasicUIntegerV1Strict() {
            Assert.Throws<InvalidOperationException>(delegate {
                var x = new InfluxField("Key", 9223372036854775808);
                x.ToString(InfluxProtocolVersion.V1Strict);
            });
        }

        [Fact(DisplayName = "InfluxField: Conversion (v2 uinteger)")]
        public void BasicUIntegerV2() {
            var x = new InfluxField("Key", 9223372036854775808);
            Assert.Equal("Key", x.Key);
            Assert.Equal(9223372036854775808UL, x.Value);
            Assert.Equal("Key=9223372036854775808u", x.ToString(InfluxProtocolVersion.V2));
        }

        [Fact(DisplayName = "InfluxField: Basic (string)")]
        public void BasicString() {
            var x = new InfluxField("Key", "Value");
            Assert.Equal("Key", x.Key);
            Assert.Equal("Value", x.Value);
            Assert.Equal("Key=\"Value\"", x.ToString());
        }

        [Fact(DisplayName = "InfluxField: Basic (boolean)")]
        public void BasicBoolean() {
            {
                var x = new InfluxField("Key", false);
                Assert.Equal("Key", x.Key);
                Assert.Equal(false, x.Value);
                Assert.Equal("Key=false", x.ToString());
            }
            {
                var x = new InfluxField("Key", true);
                Assert.Equal("Key", x.Key);
                Assert.Equal(true, x.Value);
                Assert.Equal("Key=true", x.ToString());
            }
        }


        [Fact(DisplayName = "InfluxField: Escaping key")]
        public void EscapingKey() {
            Assert.Equal(@"Key\ With\ Spaces=42", new InfluxField("Key With Spaces", 42.0).ToString());
            Assert.Equal(@"KeyWith\,Comma=42", new InfluxField("KeyWith,Comma", 42.0).ToString());
            Assert.Equal(@"KeyWith\=Equals=42", new InfluxField("KeyWith=Equals", 42.0).ToString());
            Assert.Equal(@"KeyWith\Backslash=42", new InfluxField(@"KeyWith\Backslash", 42.0).ToString());  // no need to escape backslash
            Assert.Equal(@"KeyWith""Quote=42", new InfluxField(@"KeyWith""Quote", 42.0).ToString());  // no need to escape quote
        }

        [Fact(DisplayName = "InfluxField: Escaping value")]
        public void EscapingValue() {
            Assert.Equal(@"Key=""Value With Spaces""", new InfluxField("Key", "Value With Spaces").ToString());
            Assert.Equal(@"Key=""ValueWith,Comma""", new InfluxField("Key", "ValueWith,Comma").ToString());
            Assert.Equal(@"Key=""ValueWith=Equals""", new InfluxField("Key", "ValueWith=Equals").ToString());
            Assert.Equal(@"Key=""ValueWith\\Backslash""", new InfluxField("Key", @"ValueWith\Backslash").ToString());  // no need to escape backslash
            Assert.Equal(@"Key=""ValueWith\""Quote""", new InfluxField("Key", @"ValueWith""Quote").ToString());  // no need to escape quote
        }

        [Fact(DisplayName = "InfluxField: Invalid key")]
        public void InvalidKey() {
            Assert.Throws<ArgumentNullException>(delegate {
                var _ = new InfluxField(null, "Value");
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                var _ = new InfluxField("", "Value");
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                var _ = new InfluxField(" ", "Value");
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                var _ = new InfluxField("A\n", "Value");
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                var _ = new InfluxField("A\t", "Value");
            });
        }

        [Fact(DisplayName = "InfluxField: Invalid value")]
        public void InvalidValue() {
            Assert.Throws<ArgumentNullException>(delegate {
                var _ = new InfluxField("Key", null);
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                var _ = new InfluxField("Key", "A\n");
            });
            Assert.Throws<ArgumentOutOfRangeException>(delegate {
                var _ = new InfluxField("Key", "A\t");
            });
        }

    }
}
