/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo.Net.Influx {
    using System;
    using System.Diagnostics;
    using System.Text;

    /// <summary>
    /// Single tag.
    /// </summary>
    [DebuggerDisplay("{Key,nq}: {Value}")]
    public record InfluxTag {

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="key">Tag key.</param>
        /// <param name="value">Tag value.</param>
        /// <exception cref="ArgumentNullException">Key cannot be null. -or- Value cannot be null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Key cannot be empty. -or- Key cannot start with underscore. -or- Key must not contain any control characters. -or- Reserved key. -or- Value must not contain any control characters.</exception>
        public InfluxTag(string key, string value) {
            if (key == null) { throw new ArgumentNullException(nameof(key), "Key cannot be null."); }
            if (string.IsNullOrWhiteSpace(key)) { throw new ArgumentOutOfRangeException(nameof(key), "Key cannot be empty."); }
            if (key.StartsWith('_')) { throw new ArgumentOutOfRangeException(nameof(key), "Key cannot start with underscore."); }
            if (key.Equals("time")) { throw new ArgumentOutOfRangeException(nameof(key), "Reserved key."); }
            if (!ValidateNoControlChars(key)) { throw new ArgumentOutOfRangeException(nameof(key), "Key must not contain any control characters."); }

            if (value == null) { throw new ArgumentNullException(nameof(value), "Value cannot be null."); }
            if (!ValidateNoControlChars(value)) { throw new ArgumentOutOfRangeException(nameof(key), "Value must not contain any control characters."); }

            Key = key;
            Value = value;
        }


        /// <summary>
        /// Gets tag key.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets tag value.
        /// </summary>
        public string Value { get; }


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
        /// </summary>
        public override string ToString() {
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
            foreach (var ch in Value) {
                if ((ch == ',') || (ch == '=') || (ch == ' ')) {
                    sb.Append('\\');
                    sb.Append(ch);
                } else {
                    sb.Append(ch);
                }
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
