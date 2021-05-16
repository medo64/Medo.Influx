/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo.Net.Influx {
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Single field.
    /// </summary>
    [DebuggerDisplay("{Key,nq}: {Value}")]
    public record InfluxField {

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="key">Field key.</param>
        /// <param name="value">Field value.</param>
        /// <exception cref="ArgumentNullException">Key cannot be null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Key cannot be empty. -or- Key cannot start with underscore. -or- Key must not contain any control characters.</exception>
        public InfluxField(string key, double value) {
            if (key == null) { throw new ArgumentNullException(nameof(key), "Key cannot be null."); }
            if (string.IsNullOrWhiteSpace(key)) { throw new ArgumentOutOfRangeException(nameof(key), "Key cannot be empty."); }
            if (key.StartsWith('_')) { throw new ArgumentOutOfRangeException(nameof(key), "Key cannot start with underscore."); }
            if (!ValidateNoControlChars(key)) { throw new ArgumentOutOfRangeException(nameof(key), "Key must not contain any control characters."); }

            Key = key;
            Value = value;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="key">Field key.</param>
        /// <param name="value">Field value.</param>
        /// <exception cref="ArgumentNullException">Key cannot be null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Key cannot be empty. -or- Key cannot start with underscore. -or- Key must not contain any control characters.</exception>
        public InfluxField(string key, long value) {
            if (key == null) { throw new ArgumentNullException(nameof(key), "Key cannot be null."); }
            if (string.IsNullOrWhiteSpace(key)) { throw new ArgumentOutOfRangeException(nameof(key), "Key cannot be empty."); }
            if (key.StartsWith('_')) { throw new ArgumentOutOfRangeException(nameof(key), "Key cannot start with underscore."); }
            if (!ValidateNoControlChars(key)) { throw new ArgumentOutOfRangeException(nameof(key), "Key must not contain any control characters."); }

            Key = key;
            Value = value;
        }

        /// <summary>
        /// Creates a new instance.
        /// Note that unsigned integer values are only compatible with V2 of InfluxDB protocol.
        /// </summary>
        /// <param name="key">Field key.</param>
        /// <param name="value">Field value.</param>
        /// <exception cref="ArgumentNullException">Key cannot be null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Key cannot be empty. -or- Key cannot start with underscore. -or- Key must not contain any control characters.</exception>
        public InfluxField(string key, ulong value) {
            if (key == null) { throw new ArgumentNullException(nameof(key), "Key cannot be null."); }
            if (string.IsNullOrWhiteSpace(key)) { throw new ArgumentOutOfRangeException(nameof(key), "Key cannot be empty."); }
            if (key.StartsWith('_')) { throw new ArgumentOutOfRangeException(nameof(key), "Key cannot start with underscore."); }
            if (!ValidateNoControlChars(key)) { throw new ArgumentOutOfRangeException(nameof(key), "Key must not contain any control characters."); }

            Key = key;
            Value = value;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="key">Field key.</param>
        /// <param name="value">Field value.</param>
        /// <exception cref="ArgumentNullException">Key cannot be null. -or- Value cannot be null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Key cannot be empty. -or- Key cannot start with underscore. -or- Key must not contain any control characters. -or- Value must not contain any control characters.</exception>
        public InfluxField(string key, string value) {
            if (key == null) { throw new ArgumentNullException(nameof(key), "Key cannot be null."); }
            if (string.IsNullOrWhiteSpace(key)) { throw new ArgumentOutOfRangeException(nameof(key), "Key cannot be empty."); }
            if (key.StartsWith('_')) { throw new ArgumentOutOfRangeException(nameof(key), "Key cannot start with underscore."); }
            if (!ValidateNoControlChars(key)) { throw new ArgumentOutOfRangeException(nameof(key), "Key must not contain any control characters."); }

            if (value == null) { throw new ArgumentNullException(nameof(value), "Value cannot be null."); }
            if (!ValidateNoControlChars(value)) { throw new ArgumentOutOfRangeException(nameof(key), "Value must not contain any control characters."); }

            Key = key;
            Value = value;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="key">Field key.</param>
        /// <param name="value">Field value.</param>
        /// <exception cref="ArgumentNullException">Key cannot be null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Key cannot be empty. -or- Key cannot start with underscore. -or- Key must not contain any control characters.</exception>
        public InfluxField(string key, bool value) {
            if (key == null) { throw new ArgumentNullException(nameof(key), "Key cannot be null."); }
            if (string.IsNullOrWhiteSpace(key)) { throw new ArgumentOutOfRangeException(nameof(key), "Key cannot be empty."); }
            if (key.StartsWith('_')) { throw new ArgumentOutOfRangeException(nameof(key), "Key cannot start with underscore."); }
            if (!ValidateNoControlChars(key)) { throw new ArgumentOutOfRangeException(nameof(key), "Key must not contain any control characters."); }

            Key = key;
            Value = value;
        }


        /// <summary>
        /// Gets field key.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets field value.
        /// </summary>
        public object Value { get; }


        #region ToString

        /// <summary>
        /// Returns hash code.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            return Key.GetHashCode();
        }

        /// <summary>
        /// Returns object's string representation suitable for line protocol format.
        /// Version 2 format will be used.
        /// </summary>
        public override string ToString() {
            return ToString(InfluxProtocolVersion.V2);
        }

        /// <summary>
        /// Returns object's string representation suitable for line protocol format.
        /// </summary>
        /// <param name="protocolVersion">Protocol version.</param>
        /// <exception cref="InvalidOperationException">Protocol v1 doesn't support unsigned integer.</exception>
        public string ToString(InfluxProtocolVersion protocolVersion) {
            var sb = new StringBuilder(64);  // a few more bytes by default
            foreach (var ch in Key) {
                if ((ch == ',') || (ch == '=') || (ch == ' ')) {
                    sb.Append('\\');
                    sb.Append(ch);
                } else {
                    sb.Append(ch);
                }
            }
            sb.Append('=');
            if (Value is double floatValue) {
                sb.Append(floatValue.ToString("g", CultureInfo.InvariantCulture));
            } else if (Value is long integerValue) {
                sb.Append(integerValue.ToString("0", CultureInfo.InvariantCulture));
                sb.Append('i');
            } else if (Value is ulong uintegerValue) {
                if (protocolVersion == InfluxProtocolVersion.V1Strict) { throw new InvalidOperationException("Protocol v1 doesn't support unsigned integer."); }
                if (protocolVersion == InfluxProtocolVersion.V1) {
                    if (uintegerValue > long.MaxValue) { uintegerValue = long.MaxValue; }
                    sb.Append(uintegerValue.ToString("0", CultureInfo.InvariantCulture));
                    sb.Append('i');
                } else {
                    sb.Append(uintegerValue.ToString("0", CultureInfo.InvariantCulture));
                    sb.Append('u');
                }
            } else if (Value is string stringValue) {
                sb.Append('"');
                foreach (var ch in stringValue) {
                    if ((ch == '\\') || (ch == '"')) {
                        sb.Append('\\');
                        sb.Append(ch);
                    } else {
                        sb.Append(ch);
                    }
                }
                sb.Append('"');
            } else if (Value is bool booleanValue) {
                sb.Append(booleanValue ? "true" : "false");
            } else {
                throw new InvalidOperationException("Type not recognized.");  // shouldn't really happen
            }
            return sb.ToString();
        }

        #endregion Override

        #region Internal

        private static bool ValidateNoControlChars(string key) {
            foreach (var ch in key) {
                if (char.IsControl(ch)) { return false; }
            }
            return true;
        }

        #endregion Internal

    }

}
