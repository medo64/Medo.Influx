/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo.Net.Influx;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Client interface for InfluxDB.
/// This class is thread safe.
/// </summary>
public sealed class InfluxClient : IDisposable {

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="serverUrl">Base URL of InfluxDB server, e.g. http://localhost:8086.</param>
    /// <param name="organization">Organization.</param>
    /// <param name="bucket">Bucket.</param>
    /// <param name="token">Token.</param>
    /// <param name="version">Protocol version.</param>
    /// <param name="resolution">Timestamp resolution.</param>
    /// <exception cref="ArgumentNullException">Server URL cannot be null. -or- Bucket cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Server URL must be either HTTP or HTTPS.</exception>
    public InfluxClient(Uri serverUrl, string? organization, string bucket, string? token, InfluxProtocolVersion version, InfluxTimestampResolution resolution) {
        if (serverUrl == null) { throw new ArgumentNullException(nameof(serverUrl), "Server URL cannot be null."); }
        if ((serverUrl.Scheme != "http") && (serverUrl.Scheme != "https")) { throw new ArgumentOutOfRangeException(nameof(serverUrl), "Server URL must be either HTTP or HTTPS."); }
        if (bucket == null) { throw new ArgumentNullException(nameof(bucket), "Bucket cannot be null."); }

        ServerUrl = serverUrl;
        Organization = organization;
        Bucket = bucket;
        Token = token;
        Version = version;
        Resolution = resolution;

        MaxBatchSize = 5000;
        MaxBatchInterval = 10;
        BatchRetryInterval = 1;

        var apiUriFormat = "/api/v2/write?org={0}&bucket={1}";
        switch (Resolution) {
            case InfluxTimestampResolution.Nanoseconds:
                apiUriFormat += "&precision=ns";
                break;
            case InfluxTimestampResolution.Microseconds:
                apiUriFormat += "&precision=us";
                break;
            case InfluxTimestampResolution.Miliseconds:
                apiUriFormat += "&precision=ms";
                break;
            case InfluxTimestampResolution.Seconds:
                apiUriFormat += "&precision=s";
                break;
        }
        ApiUrl = new Uri(ServerUrl, string.Format(CultureInfo.InvariantCulture, apiUriFormat, Organization, Bucket));

        BatchTimer = new(HttpBatchTimerCallback, null, 1000, 1000);
    }


    #region V1

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="serverUrl">Base URL of InfluxDB server, e.g. http://localhost:8086.</param>
    /// <param name="bucket">Bucket.</param>
    /// <exception cref="ArgumentNullException">Server URL cannot be null. -or- Bucket cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Server URL must be either HTTP or HTTPS.</exception>
    public static InfluxClient V1(Uri serverUrl, string bucket) {
        return new InfluxClient(serverUrl, null, bucket, null, InfluxProtocolVersion.V1, InfluxTimestampResolution.Nanoseconds);
    }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="serverUrl">Base URL of InfluxDB server, e.g. http://localhost:8086.</param>
    /// <param name="bucket">Bucket.</param>
    /// <param name="token">Token.</param>
    /// <exception cref="ArgumentNullException">Server URL cannot be null. -or- Bucket cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Server URL must be either HTTP or HTTPS.</exception>
    public static InfluxClient V1(Uri serverUrl, string bucket, string? token) {
        return new InfluxClient(serverUrl, null, bucket, token, InfluxProtocolVersion.V1, InfluxTimestampResolution.Nanoseconds);
    }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="serverUrl">Base URL of InfluxDB server, e.g. http://localhost:8086.</param>
    /// <param name="bucket">Bucket.</param>
    /// <param name="token">Token.</param>
    /// <param name="resolution">Timestamp resolution.</param>
    /// <exception cref="ArgumentNullException">Server URL cannot be null. -or- Bucket cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Server URL must be either HTTP or HTTPS.</exception>
    public static InfluxClient V1(Uri serverUrl, string bucket, string? token, InfluxTimestampResolution resolution) {
        return new InfluxClient(serverUrl, null, bucket, token, InfluxProtocolVersion.V1, resolution);
    }

    #endregion V1

    #region V2

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="serverUrl">Base URL of InfluxDB server, e.g. http://localhost:8086.</param>
    /// <param name="organization">Organization.</param>
    /// <param name="bucket">Bucket.</param>
    /// <exception cref="ArgumentNullException">Server URL cannot be null. -or- Bucket cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Server URL must be either HTTP or HTTPS.</exception>
    public static InfluxClient V2(Uri serverUrl, string? organization, string bucket) {
        return new InfluxClient(serverUrl, organization, bucket, null, InfluxProtocolVersion.V2, InfluxTimestampResolution.Nanoseconds);
    }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="serverUrl">Base URL of InfluxDB server, e.g. http://localhost:8086.</param>
    /// <param name="organization">Organization.</param>
    /// <param name="bucket">Bucket.</param>
    /// <param name="token">Token.</param>
    /// <exception cref="ArgumentNullException">Server URL cannot be null. -or- Bucket cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Server URL must be either HTTP or HTTPS.</exception>
    public static InfluxClient V2(Uri serverUrl, string? organization, string bucket, string? token) {
        return new InfluxClient(serverUrl, organization, bucket, token, InfluxProtocolVersion.V2, InfluxTimestampResolution.Nanoseconds);
    }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="serverUrl">Base URL of InfluxDB server, e.g. http://localhost:8086.</param>
    /// <param name="organization">Organization.</param>
    /// <param name="bucket">Bucket.</param>
    /// <param name="token">Token.</param>
    /// <param name="resolution">Timestamp resolution.</param>
    /// <exception cref="ArgumentNullException">Server URL cannot be null. -or- Bucket cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Server URL must be either HTTP or HTTPS.</exception>
    public static InfluxClient V2(Uri serverUrl, string? organization, string bucket, string? token, InfluxTimestampResolution resolution) {
        return new InfluxClient(serverUrl, organization, bucket, token, InfluxProtocolVersion.V2, resolution);
    }

    #endregion V2


    /// <summary>
    /// Gets base URL of InfluxDB server.
    /// e.g. http://localhost:8086
    /// </summary>
    public Uri ServerUrl { get; }

    /// <summary>
    /// Gets protocol version to use.
    /// </summary>
    public InfluxProtocolVersion Version { get; }

    /// <summary>
    /// Gets timestamp resolution.
    /// </summary>
    public InfluxTimestampResolution Resolution { get; }

    /// <summary>
    /// Gets organization.
    /// </summary>
    public string? Organization { get; }

    /// <summary>
    /// Gets bucket.
    /// </summary>
    public string Bucket { get; }

    /// <summary>
    /// Gets token.
    /// </summary>
    public string? Token { get; }


    private int _maxBatchSize;
    /// <summary>
    /// Gets/sets batch size.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Cannot have maximum batch size smaller than 1 -or- Cannot have maximum batch size larger than 100,000..</exception>
    public int MaxBatchSize {
        get { return _maxBatchSize; }
        set {
            if (value < 1) { throw new ArgumentOutOfRangeException(nameof(value), "Cannot have maximum batch size smaller than 1."); }
            if (value > 100000) { throw new ArgumentOutOfRangeException(nameof(value), "Cannot have maximum batch size larger than 100,000."); }
            lock (BatchLock) {
                _maxBatchSize = value;
            }
        }
    }

    private int _maxBatchInterval;
    /// <summary>
    /// Gets/sets batch interval in seconds.
    /// Any batched measurements will be sent at that time.
    /// If value is Timeout.Infinite, no interval-based batching will be used.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Batch interval must be either Timeout.Infinite or between 1 second and 1 hour.</exception>
    public int MaxBatchInterval {
        get { return _maxBatchInterval; }
        set {
            if ((value != Timeout.Infinite) && ((value < 1) && (value > 3600))) { throw new ArgumentOutOfRangeException(nameof(value), "Batch interval must be either Timeout.Infinite or between 1 second and 1 hour."); }
            lock (BatchLock) {
                _maxBatchInterval = value;
            }
        }
    }

    private int _batchRetryInterval;
    /// <summary>
    /// Gets/sets batch retry interval in seconds.
    /// There will be only 1 retry for any failed batch request.
    /// If value is Timeout.Infinite, there will be no retry.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Batch retry interval must be either Timeout.Infinite or between 1 second and 1 minute.</exception>
    public int BatchRetryInterval {
        get { return _batchRetryInterval; }
        set {
            if ((value != Timeout.Infinite) && ((value < 1) && (value > 3600))) { throw new ArgumentOutOfRangeException(nameof(value), "Batch retry interval must be either Timeout.Infinite or between 1 second and 1 minute."); }
            lock (BatchLock) {
                _batchRetryInterval = value;
            }
        }
    }


    /// <summary>
    /// Immediately sends measurement to InfluxDB and returns if operation was successful.
    /// </summary>
    /// <param name="measurement">Measurement to send.</param>
    /// <exception cref="ArgumentNullException">Measurement cannot be null.</exception>
    public InfluxResult Send(InfluxMeasurement measurement) {
        if (Disposed) { throw new ObjectDisposedException(nameof(InfluxClient), "Object has been disposed."); }
        if (measurement == null) { throw new ArgumentNullException(nameof(measurement), "Measurement cannot be null."); }
        return HttpSend(measurement.ToString(Version, Resolution) + "\n");
    }

    /// <summary>
    /// Immediately sends measurement to InfluxDB and returns if operation was successful.
    /// </summary>
    /// <param name="measurements">Measurements to send.</param>
    /// <exception cref="ArgumentNullException">Measurements cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Missing measurements.</exception>
    public InfluxResult Send(params InfluxMeasurement[] measurements) {
        if (Disposed) { throw new ObjectDisposedException(nameof(InfluxClient), "Object has been disposed."); }
        if (measurements == null) { throw new ArgumentNullException(nameof(measurements), "Measurements cannot be null."); }
        if (measurements.Length == 0) { throw new ArgumentOutOfRangeException(nameof(measurements), "Missing measurements."); }

        var content = new StringBuilder(measurements.Length * 1024);
        foreach (var measurement in measurements) {
            content.Append(measurement.ToString(Version, Resolution));
            content.Append('\n');
        }
        return HttpSend(content.ToString());
    }


    /// <summary>
    /// Immediately sends measurement to InfluxDB.
    /// </summary>
    /// <param name="measurement">Measurement to send.</param>
    /// <exception cref="ArgumentNullException">Measurement cannot be null.</exception>
    public async Task<InfluxResult> SendAsync(InfluxMeasurement measurement) {
        if (Disposed) { throw new ObjectDisposedException(nameof(InfluxClient), "Object has been disposed."); }
        if (measurement == null) { throw new ArgumentNullException(nameof(measurement), "Measurement cannot be null."); }
        return await HttpSendAsync(measurement.ToString(Version, Resolution) + "\n").ConfigureAwait(continueOnCapturedContext: false);
    }

    /// <summary>
    /// Immediately sends measurements to InfluxDB.
    /// </summary>
    /// <param name="measurements">Measurements to send.</param>
    /// <exception cref="ArgumentNullException">Measurements cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Missing measurements.</exception>
    public async Task<InfluxResult> SendAsync(params InfluxMeasurement[] measurements) {
        if (Disposed) { throw new ObjectDisposedException(nameof(InfluxClient), "Object has been disposed."); }
        if (measurements == null) { throw new ArgumentNullException(nameof(measurements), "Measurements cannot be null."); }
        if (measurements.Length == 0) { throw new ArgumentOutOfRangeException(nameof(measurements), "Missing measurements."); }

        var content = new StringBuilder(measurements.Length * 1024);
        foreach (var measurement in measurements) {
            content.Append(measurement.ToString(Version, Resolution));
            content.Append('\n');
        }
        return await HttpSendAsync(content.ToString()).ConfigureAwait(continueOnCapturedContext: false); ;
    }


    /// <summary>
    /// Queues measurement and sends it later based on MaxBatchSize and MaxBatchInterval.
    /// </summary>
    /// <param name="measurement">Measurement to queue.</param>
    public void Queue(InfluxMeasurement measurement) {
        Queue(new InfluxMeasurement[] { measurement });
    }

    /// <summary>
    /// Queues measurement and sends it later based on MaxBatchSize and MaxBatchInterval.
    /// </summary>
    /// <param name="measurements">Measurements to queue.</param>
    /// <exception cref="ArgumentNullException">Measurements cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Missing measurements.</exception>
    public void Queue(params InfluxMeasurement[] measurements) {
        if (Disposed) { throw new ObjectDisposedException(nameof(InfluxClient), "Object has been disposed."); }
        if (measurements == null) { throw new ArgumentNullException(nameof(measurements), "Measurements cannot be null."); }
        if (measurements.Length == 0) { throw new ArgumentOutOfRangeException(nameof(measurements), "Missing measurements."); }

        lock (BatchLock) {
            foreach (var measurement in measurements) {
                BatchQueue.Enqueue(measurement.ToString(Version, Resolution));
            }
        }

        Task.Run(delegate { HttpBatchSend(); });  // check queue in background
    }


    /// <summary>
    /// Flushes the queue.
    /// </summary>
    public void Flush() {
        if (Disposed) { return; }
        HttpBatchSend(force: true);
    }


    #region Events

    /// <summary>
    /// Event raised when batch was successfully sent.
    /// </summary>
    public event EventHandler<InfluxBatchEventArgs>? BatchSucceeded;

    private void OnBatchSucceeded(InfluxBatchEventArgs e) {
        BatchSucceeded?.Invoke(this, e);
    }

    /// <summary>
    /// Event raised when batch has failed.
    /// </summary>
    public event EventHandler<InfluxBatchEventArgs>? BatchFailed;

    private void OnBatchFailed(InfluxBatchEventArgs e) {
        BatchFailed?.Invoke(this, e);
    }

    #endregion

    #region HttpClient

    private readonly HttpClient HttpClient = new();
    private readonly Uri ApiUrl;

    private HttpRequestMessage GetRequest(string content) {
        var request = new HttpRequestMessage(HttpMethod.Post, ApiUrl);
        if (Token != null) { request.Headers.Add("Authorization", "Token " + Token); }
        request.Content = new StringContent(content, Encoding.UTF8, "text/plain");

        return request;
    }

    private static InfluxResult GetResult(HttpResponseMessage response) {
        if (response.IsSuccessStatusCode) { return InfluxResult.Success(); }

        var content = response.Content.ReadAsStringAsync().Result;
        var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(content));
        while (reader.Read()) {
            if (reader.TokenType == JsonTokenType.PropertyName) {
                if ("message".Equals(reader.GetString(), StringComparison.Ordinal)) {
                    if (reader.Read()) {
                        var message = reader.GetString();
                        return InfluxResult.Failure(message ?? response.StatusCode.ToString());
                    }
                }
            }
        }

        return InfluxResult.Failure(response.StatusCode.ToString());
    }

    private InfluxResult HttpSend(string content) {
        using var request = GetRequest(content);
        try {
            var res = HttpClient.SendAsync(request).Result;  // have to use SendAsync() as Send() doesn't work with HTTP/2
            return GetResult(res);
        } catch (AggregateException ex) {
            return InfluxResult.Failure(ex.GetBaseException());
        }
    }

    private async Task<InfluxResult> HttpSendAsync(string content) {
        using var request = GetRequest(content);
        try {
            var res = await HttpClient.SendAsync(request).ConfigureAwait(continueOnCapturedContext: false); ;
            return GetResult(res);
        } catch (HttpRequestException ex) {
            return InfluxResult.Failure(ex);
        }
    }

    #endregion HttpClient

    #region Batch

    private readonly object BatchLock = new();
    private readonly Queue<string> BatchQueue = new();

    private void HttpBatchSend(bool force = false) {
        while (true) {
            var batchContentBuilder = new StringBuilder();
            var batchSize = 0;

            int batchRetryInterval;
            lock (BatchLock) {
                if (BatchQueue.Count == 0) { return; }  // no items to queue
                if (!force && (BatchQueue.Count < MaxBatchSize)) { return; }  // not enough items in queue

                for (var i = 0; i < MaxBatchSize; i++) {
                    if (BatchQueue.Count == 0) { break; }  // done with all items - can happen if forced
                    batchContentBuilder.Append(BatchQueue.Dequeue());
                    batchContentBuilder.Append('\n');
                    batchSize += 1;
                }
                batchRetryInterval = BatchRetryInterval;
                BatchStopwatch.Restart();  // delay batch sending
            }

            var batchContent = batchContentBuilder.ToString();
            Task.Run(delegate {
                var res = HttpSend(batchContent);
                if (!res) {  // retry a bit later
                    if (batchRetryInterval > 0) {
                        Task.Delay(batchRetryInterval * 1000);
                        res = HttpSend(batchContent);
                    }
                }
                if (res) {
                    OnBatchSucceeded(new InfluxBatchEventArgs(batchSize, res));
                } else {
                    OnBatchFailed(new InfluxBatchEventArgs(batchSize, res));
                }
            });

            lock (BatchLock) {
                if (BatchQueue.Count < MaxBatchSize) { break; }  // done if queue is low enough
            }
        }
    }

    private readonly Timer BatchTimer;
    private readonly Stopwatch BatchStopwatch = Stopwatch.StartNew();

    private void HttpBatchTimerCallback(object? state) {
        lock (BatchLock) {
            if (BatchQueue.Count == 0) {
                BatchStopwatch.Restart();  // don't count seconds without anything in queue
                return;
            }
        }

        if (BatchStopwatch.ElapsedMilliseconds > MaxBatchInterval * 1000) {  // enough time has elapsed, send what you have
            HttpBatchSend(force: true);  // this will reset stopwatch
        }
    }

    #endregion Batch


    #region IDisposable

    private bool Disposed;

    /// <summary>
    /// Disposes class.
    /// </summary>
    public void Dispose() {
        lock (BatchLock) {  // since we're gonna empty queue
            Flush();
            HttpClient.Dispose();
            BatchTimer.Dispose();
            Disposed = true;
        }
    }

    #endregion IDisposable

}
