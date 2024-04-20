// Copyright (c) AI Downloader. All rights reserved.

namespace AIDownloader.Aria.Exceptions;

/// <summary>
/// Represents an exception thrown by Aria2.
/// </summary>
public class Aria2Exception : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Aria2Exception"/> class.
    /// </summary>
    public Aria2Exception(long resultCode, string? resultMessage)
        : base($"Error {resultCode}: {resultMessage}")
    {
        ResultCode = resultCode;
        ResultMessage = resultMessage ?? "Unknown error";
    }

    /// <summary>
    /// Gets the result message.
    /// </summary>
    public string ResultMessage { get; }

    /// <summary>
    /// Gets the result code.
    /// </summary>
    public long ResultCode { get; }
}
