// Copyright (c) AI Downloader. All rights reserved.

namespace RodelDownloader.Aria.Models;

/// <summary>
/// Download link item.
/// </summary>
public sealed class LinkItem
{
    /// <summary>
    /// Gets or sets the download link.
    /// </summary>
    public string Link { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the gid.
    /// </summary>
    public string Gid { get; set; }
}
