// Copyright (c) AI Downloader. All rights reserved.

namespace AIDownloader.UI.Models;

/// <summary>
/// 文件夹条目.
/// </summary>
public sealed class FolderItem
{
    /// <summary>
    /// 名称.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 路径.
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// 是否可用.
    /// </summary>
    /// <returns>是否存在.</returns>
    public bool IsAvailable()
        => !string.IsNullOrEmpty(Path) && Directory.Exists(Path);

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is FolderItem item && Path == item.Path;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Path);
}
