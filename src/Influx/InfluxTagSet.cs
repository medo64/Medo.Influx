/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo.Net.Influx;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// Sorted list of tags.
/// This class is thread safe.
/// </summary>
[DebuggerDisplay("{Count} tags")]
public sealed class InfluxTagSet : IEnumerable<InfluxTag> {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public InfluxTagSet() { }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <exception cref="ArgumentNullException">Tags cannot be null.</exception>
    /// <exception cref="ArgumentException">Cannot add duplicate tag.</exception>
    public InfluxTagSet(IEnumerable<InfluxTag> tags) {
        if (tags == null) { throw new ArgumentNullException(nameof(tags), "Tags cannot be null."); }
        foreach (var tag in tags) {
            Add(tag);
        }
    }


    /// <summary>
    /// Gets number of tags in the set.
    /// </summary>
    public int Count => Base.Count;

    /// <summary>
    /// Gets tag by index.
    /// </summary>
    /// <param name="index">Index of tag.</param>
    public InfluxTag this[int index] {
        get {
            lock (SyncLock) {
                SortIfNeedBe();
                return Base[index];
            }
        }
    }

    /// <summary>
    /// Adds a new tag to the set.
    /// </summary>
    /// <param name="tag">Tag.</param>
    /// <exception cref="ArgumentNullException">Tag cannot be null.</exception>
    /// <exception cref="ArgumentException">Cannot add duplicate tag.</exception>
    public void Add(InfluxTag tag) {
        if (tag == null) { throw new ArgumentNullException(nameof(tag), "Tag cannot be null."); }
        lock (SyncLock) {
            foreach (var existing in Base) {
                if (tag.Key.Equals(existing.Key, StringComparison.Ordinal)) {
                    throw new ArgumentException("Cannot add duplicate tag.", nameof(tag));
                }
            }
            Base.Add(tag);
            if (Base.Count > 1) { IsBaseSorted = false; }
        }
    }

    #region Shortcuts

    /// <summary>
    /// Adds a new tag to the set.
    /// </summary>
    /// <param name="key">Tag key.</param>
    /// <param name="value">Tag value.</param>
    /// <exception cref="ArgumentNullException">Key cannot be null. -or- Value cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Key cannot be empty. -or- Key cannot start with underscore. -or- Key must not contain any control characters. -or- Reserved key. -or- Value must not contain any control characters.</exception>
    /// <exception cref="ArgumentException">Cannot add duplicate tag.</exception>
    public InfluxTagSet Add(string key, string value) {
        Add(new InfluxTag(key, value));
        return this;
    }

    #endregion Shortcuts


    #region IEnumerable

    /// <summary>
    /// Return enumerator.
    /// </summary>
    public IEnumerator<InfluxTag> GetEnumerator() {
        IList<InfluxTag> list;
        lock (SyncLock) {
            SortIfNeedBe();
            list = Base.AsReadOnly();
        }
        return list.GetEnumerator();
    }

    /// <summary>
    /// Return enumerator.
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    #endregion IEnumerable

    #region Internal

    private readonly object SyncLock = new();
    private readonly List<InfluxTag> Base = new();
    private bool IsBaseSorted = true;

    internal void SortIfNeedBe() {
        if (!IsBaseSorted) {
            Base.Sort(delegate (InfluxTag item1, InfluxTag item2) {
                return string.CompareOrdinal(item1.Key, item2.Key);
            });
            IsBaseSorted = true;
        }
    }

    #endregion Internal

}
