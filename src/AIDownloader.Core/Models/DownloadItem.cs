// Copyright (c) AI Downloader. All rights reserved.

namespace AIDownloader.Core.Models;

/// <summary>
/// 下载链接.
/// </summary>
public class DownloadItem(string fileName, string link)
{
    /// <summary>
    /// 文件名.
    /// </summary>
    public string FileName { get; } = fileName;

    /// <summary>
    /// 链接.
    /// </summary>
    public string Link { get; } = link;

    /// <summary>
    /// 目标文件夹.
    /// </summary>
    public string TargetFolder { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is DownloadItem item && Link == item.Link;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Link);

    /// <inheritdoc/>
    public override string ToString() => FileName;
}
