/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo.Net.Influx {

    /// <summary>
    /// Influx protocol version.
    /// </summary>
    public enum InfluxProtocolVersion {

        /// <summary>
        /// InfluxDB Line protocol version 1.
        /// Any field or tag value not part of V1 (e.g. unsigned integer) will be adjusted in the background.
        /// </summary>
        V1 = 0,

        /// <summary>
        /// InfluxDB Line protocol version 1.
        /// Any field not part of V1 will cause an exception.
        /// </summary>
        V1Strict = 1,

        /// <summary>
        /// InfluxDB Line protocol version 2.
        /// </summary>
        V2 = 2,

    }
}
