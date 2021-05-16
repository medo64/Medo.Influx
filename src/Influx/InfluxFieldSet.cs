/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo.Net.Influx {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Sorted list of fields.
    /// This class is thread safe.
    /// </summary>
    [DebuggerDisplay("{Count} fields")]
    public sealed class InfluxFieldSet : IEnumerable<InfluxField> {

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public InfluxFieldSet() { }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <exception cref="ArgumentNullException">Fields cannot be null.</exception>
        /// <exception cref="ArgumentException">Cannot add duplicate field.</exception>
        public InfluxFieldSet(IEnumerable<InfluxField> fields) {
            if (fields == null) { throw new ArgumentNullException(nameof(fields), "Fields cannot be null."); }
            foreach (var field in fields) {
                Add(field);
            }
        }


        /// <summary>
        /// Gets number of fields in the set.
        /// </summary>
        public int Count => Base.Count;

        /// <summary>
        /// Gets field by index.
        /// </summary>
        /// <param name="index">Index of field.</param>
        public InfluxField this[int index] {
            get {
                lock (SyncLock) {
                    SortIfNeedBe();
                    return Base[index];
                }
            }
        }

        /// <summary>
        /// Adds a new field to the set.
        /// </summary>
        /// <param name="field">Field.</param>
        /// <exception cref="ArgumentNullException">Field cannot be null.</exception>
        /// <exception cref="ArgumentException">Cannot add duplicate field.</exception>
        public void Add(InfluxField field) {
            if (field == null) { throw new ArgumentNullException(nameof(field), "Field cannot be null."); }
            lock (SyncLock) {
                foreach (var existing in Base) {
                    if (field.Key.Equals(existing.Key, StringComparison.InvariantCulture)) {
                        throw new ArgumentException("Cannot add duplicate field.", nameof(field));
                    }
                }
                Base.Add(field);
                if (Base.Count > 1) { IsBaseSorted = false; }
            }
        }


        #region Shortcuts

        /// <summary>
        /// Adds a new field to the set.
        /// </summary>
        /// <param name="key">Field key.</param>
        /// <param name="value">Field value.</param>
        /// <exception cref="ArgumentNullException">Key cannot be null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Key cannot be empty. -or- Key cannot start with underscore. -or- Key must not contain any control characters.</exception>
        /// <exception cref="ArgumentException">Cannot add duplicate field.</exception>
        public InfluxFieldSet Add(string key, double value) {
            Add(new InfluxField(key, value));
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
        public InfluxFieldSet Add(string key, long value) {
            Add(new InfluxField(key, value));
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
        public InfluxFieldSet Add(string key, ulong value) {
            Add(new InfluxField(key, value));
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
        public InfluxFieldSet Add(string key, string value) {
            Add(new InfluxField(key, value));
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
        public InfluxFieldSet Add(string key, bool value) {
            Add(new InfluxField(key, value));
            return this;
        }

        #endregion Shortcuts


        #region IEnumerable

        /// <summary>
        /// Return enumerator.
        /// </summary>
        public IEnumerator<InfluxField> GetEnumerator() {
            IList<InfluxField> list;
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
        private readonly List<InfluxField> Base = new();
        private bool IsBaseSorted = true;

        internal void SortIfNeedBe() {
            if (!IsBaseSorted) {
                Base.Sort(delegate (InfluxField item1, InfluxField item2) {
                    return item1.Key.CompareTo(item2.Key);
                });
                IsBaseSorted = true;
            }
        }

        #endregion Internal

    }
}
