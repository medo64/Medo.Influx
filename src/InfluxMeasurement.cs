/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo.Net.Influx;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

/// <summary>
/// A single measurement.
/// </summary>
[DebuggerDisplay("{Name,nq} ({Tags.Count} tags, {Fields.Count} fields)")]
public record InfluxMeasurement {

    /// <summary>
    /// Creates a new instance.
    /// Uses current time as a timestamp.
    /// </summary>
    /// <param name="name">Measurement name.</param>
    public InfluxMeasurement(string name)
        : this(name, DateTime.UtcNow) {
    }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="name">Measurement name.</param>
    /// <param name="fields">Fields.</param>
    /// <exception cref="ArgumentNullException">Name cannot be null. -or- Fields cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Name cannot be empty. -or- Name cannot start with underscore. -or- Name must not contain any control characters.</exception>
    public InfluxMeasurement(string name, IEnumerable<InfluxField> fields)
        : this(name, Array.Empty<InfluxTag>(), fields, DateTime.UtcNow) {
    }


    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="name">Measurement name.</param>
    /// <param name="timestamp">Measurement timestamp.</param>
    /// <exception cref="ArgumentNullException">Name cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Name cannot be empty. -or- Name cannot start with underscore. -or- Name must not contain any control characters.</exception>
    public InfluxMeasurement(string name, DateTime timestamp)
        : this(name, Array.Empty<InfluxTag>(), Array.Empty<InfluxField>(), timestamp) {
    }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="name">Measurement name.</param>
    /// <param name="fields">Fields.</param>
    /// <param name="timestamp">Measurement timestamp.</param>
    /// <exception cref="ArgumentNullException">Name cannot be null. -or- Fields cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Name cannot be empty. -or- Name cannot start with underscore. -or- Name must not contain any control characters.</exception>
    public InfluxMeasurement(string name, IEnumerable<InfluxField> fields, DateTime timestamp)
        : this(name, Array.Empty<InfluxTag>(), fields, timestamp) {
    }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="name">Measurement name.</param>
    /// <param name="tags">Tags.</param>
    /// <param name="fields">Fields.</param>
    /// <param name="timestamp">Measurement timestamp.</param>
    /// <exception cref="ArgumentNullException">Name cannot be null. -or- Tags cannot be null. -or- Fields cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Name cannot be empty. -or- Name cannot start with underscore. -or- Name must not contain any control characters.</exception>
    public InfluxMeasurement(string name, IEnumerable<InfluxTag> tags, IEnumerable<InfluxField> fields, DateTime timestamp) {
        if (name == null) { throw new ArgumentNullException(nameof(name), "Name cannot be null."); }
        if (string.IsNullOrWhiteSpace(name)) { throw new ArgumentOutOfRangeException(nameof(name), "Name cannot be empty."); }
        if (name.StartsWith('_')) { throw new ArgumentOutOfRangeException(nameof(name), "Name cannot start with underscore."); }
        if (!ValidateNoControlChars(name)) { throw new ArgumentOutOfRangeException(nameof(name), "Name must not contain any control characters."); }
        if (tags == null) { throw new ArgumentNullException(nameof(tags), "Tags cannot be null."); }
        if (fields == null) { throw new ArgumentNullException(nameof(fields), "Fields cannot be null."); }

        Name = name;
        foreach (var tag in tags) {
            Tags.Add(tag);
        }
        foreach (var field in fields) {
            Fields.Add(field);
        }
        Timestamp = timestamp;
    }


    /// <summary>
    /// Gets measurement name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets measurement timestamp.
    /// </summary>
    public DateTime Timestamp { get; }

    /// <summary>
    /// Gets tags.
    /// </summary>
    public InfluxTagSet Tags { get; } = new();

    /// <summary>
    /// Gets fields.
    /// </summary>
    public InfluxFieldSet Fields { get; } = new();


    #region Shortcuts

    /// <summary>
    /// Adds a new tag to the set.
    /// </summary>
    /// <param name="name">Tag key.</param>
    /// <param name="value">Tag value.</param>
    /// <exception cref="ArgumentNullException">Key cannot be null. -or- Value cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Key cannot be empty. -or- Key cannot start with underscore. -or- Key must not contain any control characters. -or- Reserved key. -or- Value must not contain any control characters.</exception>
    /// <exception cref="ArgumentException">Cannot add duplicate field.</exception>
    public InfluxMeasurement AddTag(string name, string value) {
        Tags.Add(new InfluxTag(name, value));
        return this;
    }

    /// <summary>
    /// Adds a new field to the set.
    /// </summary>
    /// <param name="key">Field key.</param>
    /// <param name="value">Field value.</param>
    /// <exception cref="ArgumentNullException">Key cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Key cannot be empty. -or- Key cannot start with underscore. -or- Key must not contain any control characters.</exception>
    /// <exception cref="ArgumentException">Cannot add duplicate field.</exception>
    public InfluxMeasurement AddField(string key, double value) {
        Fields.Add(new InfluxField(key, value));
        return this;
    }

    /// <summary>
    /// Adds a new field to the set.
    /// </summary>
    /// <param name="key">Field key.</param>
    /// <param name="value">Field value.</param>
    /// <exception cref="ArgumentNullException">Key cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Key cannot be empty. -or- Key cannot start with underscore. -or- Key must not contain any control characters.</exception>
    /// <exception cref="ArgumentException">Cannot add duplicate field.</exception>
    public InfluxMeasurement AddField(string key, long value) {
        Fields.Add(new InfluxField(key, value));
        return this;
    }

    /// <summary>
    /// Adds a new field to the set.
    /// </summary>
    /// <param name="key">Field key.</param>
    /// <param name="value">Field value.</param>
    /// <exception cref="ArgumentNullException">Key cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Key cannot be empty. -or- Key cannot start with underscore. -or- Key must not contain any control characters.</exception>
    /// <exception cref="ArgumentException">Cannot add duplicate field.</exception>
    public InfluxMeasurement AddField(string key, ulong value) {
        Fields.Add(new InfluxField(key, value));
        return this;
    }

    /// <summary>
    /// Adds a new field to the set.
    /// </summary>
    /// <param name="key">Field key.</param>
    /// <param name="value">Field value.</param>
    /// <exception cref="ArgumentNullException">Key cannot be null. -or- Value cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Key cannot be empty. -or- Key cannot start with underscore. -or- Key must not contain any control characters.</exception>
    /// <exception cref="ArgumentException">Cannot add duplicate field.</exception>
    public InfluxMeasurement AddField(string key, string value) {
        Fields.Add(new InfluxField(key, value));
        return this;
    }

    /// <summary>
    /// Adds a new field to the set.
    /// </summary>
    /// <param name="key">Field key.</param>
    /// <param name="value">Field value.</param>
    /// <exception cref="ArgumentNullException">Key cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Key cannot be empty. -or- Key cannot start with underscore. -or- Key must not contain any control characters.</exception>
    /// <exception cref="ArgumentException">Cannot add duplicate field.</exception>
    public InfluxMeasurement AddField(string key, bool value) {
        Fields.Add(new InfluxField(key, value));
        return this;
    }

    #endregion Shortcuts


    #region ToString

    /// <summary>
    /// Returns hash code.
    /// </summary>
    public override int GetHashCode() {
        return Name.GetHashCode(StringComparison.Ordinal);
    }

    /// <summary>
    /// Returns object's string representation suitable for line protocol format.
    /// Version 2 format with nanosecond resolution will be used.
    /// </summary>
    public override string ToString() {
        return ToString(InfluxProtocolVersion.V2, InfluxTimestampResolution.Nanoseconds);
    }

    /// <summary>
    /// Returns object's string representation suitable for line protocol format.
    /// </summary>
    /// <param name="protocolVersion">Protocol version.</param>
    /// <param name="resolution">Timestamp resolution.</param>
    /// <exception cref="InvalidOperationException">Protocol v1 doesn't support unsigned integer. -or- Must have at least one field. -or- Date cannot be older than the start of Unix epoch.</exception>
    public string ToString(InfluxProtocolVersion protocolVersion, InfluxTimestampResolution resolution) {
        if (Fields.Count == 0) { throw new InvalidOperationException("Must have at least one field."); }
        var sb = new StringBuilder(1024);  // a few more bytes by default
        foreach (var ch in Name) {
            if ((ch == ',') || (ch == ' ')) {
                sb.Append('\\');
                sb.Append(ch);
            } else {
                sb.Append(ch);
            }
        }
        foreach (var tag in Tags) {
            sb.Append(',');
            sb.Append(tag.ToString());
        }
        sb.Append(' ');
        var isFirstField = true;
        foreach (var field in Fields) {
            if (!isFirstField) { sb.Append(','); } else { isFirstField = false; }
            sb.Append(field.ToString(protocolVersion));
        }

        var ticksUnix = (Timestamp - DateTime.UnixEpoch).Ticks;
        if (ticksUnix < 0) { throw new InvalidOperationException("Date cannot be older than the start of Unix epoch."); }
        switch (resolution) {
            case InfluxTimestampResolution.Nanoseconds:
                sb.Append(' ');
                sb.Append(ticksUnix.ToString("0", CultureInfo.InvariantCulture) + "00");
                break;

            case InfluxTimestampResolution.Microseconds:
                sb.Append(' ');
                sb.Append((ticksUnix / 10).ToString("0", CultureInfo.InvariantCulture));
                break;

            case InfluxTimestampResolution.Miliseconds:
                sb.Append(' ');
                sb.Append((ticksUnix / 10000).ToString("0", CultureInfo.InvariantCulture));
                break;

            case InfluxTimestampResolution.Seconds:
                sb.Append(' ');
                sb.Append((ticksUnix / 10000000).ToString("0", CultureInfo.InvariantCulture));
                break;

            default: break;
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
