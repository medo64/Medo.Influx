/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo.Net.Influx {
    using System;

    /// <summary>
    /// Batch event arguments.
    /// </summary>
    public sealed class InfluxBatchEventArgs : EventArgs {

        internal InfluxBatchEventArgs(int batchSize, InfluxResult result) {
            BatchSize = batchSize;
            Result = result;
        }


        /// <summary>
        /// Gets number of items sent in given batch.
        /// </summary>
        public int BatchSize { get; }

        /// <summary>
        /// Gets result of HTTP send operation.
        /// </summary>
        public InfluxResult Result { get; }

    }
}
