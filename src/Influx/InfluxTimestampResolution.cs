/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo.Net.Influx {

    /// <summary>
    /// Influx timestamp resolution.
    /// </summary>
    public enum InfluxTimestampResolution {

        /// <summary>
        /// No timestamp is to be used.
        /// </summary>
        None = 0,

        /// <summary>
        /// Nanosecond timestamp resolution.
        /// Note that .NET restricts this to 100 ns minimum.
        /// </summary>
        Nanoseconds = 1,

        /// <summary>
        /// Microsecond timestamp resolution.
        /// </summary>
        Microseconds = 2,

        /// <summary>
        /// Milisecond timestamp resolution.
        /// </summary>
        Miliseconds = 3,

        /// <summary>
        /// Second timestamp resolution.
        /// </summary>
        Seconds = 4,

    }
}
