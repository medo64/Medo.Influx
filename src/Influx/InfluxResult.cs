/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo.Net.Influx;

using System;

/// <summary>
/// Result of sending operation
/// </summary>
public record InfluxResult {

    private InfluxResult(bool isSuccess, string? errorText) {
        IsSuccess = isSuccess;
        ErrorText = errorText;
    }

    private static readonly InfluxResult SuccessCache = new(isSuccess: true, errorText: null);
    internal static InfluxResult Success() {
        return SuccessCache;
    }

    internal static InfluxResult Failure(Exception exception) {
        return new InfluxResult(isSuccess: false, errorText: exception.Message);
    }

    internal static InfluxResult Failure(string errorText) {
        return new InfluxResult(isSuccess: false, errorText: errorText);
    }


    /// <summary>
    /// Gets if send operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets error text or null if operation was successful.
    /// </summary>
    public string? ErrorText { get; }


    #region Coversion

    /// <summary>
    /// Returns success status.
    /// </summary>
    public bool ToBoolean() {
        return IsSuccess;
    }

    /// <summary>
    /// Gets success status.
    /// </summary>
    /// <param name="result">Result.</param>
    public static implicit operator bool(InfluxResult result) {
        return result?.IsSuccess ?? false;
    }

    /// <summary>
    /// Gets error text.
    /// </summary>
    /// <param name="result">Result.</param>
    public static implicit operator string?(InfluxResult result) {
        return result?.ErrorText;
    }

    #endregion Conversion

}
