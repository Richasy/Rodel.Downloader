// Copyright (c) AI Downloader. All rights reserved.

namespace AIDownloader.UI.Models.Constants;

/// <summary>
/// 下载状态.
/// </summary>
public enum DownloadStatus
{
    /// <summary>
    /// 正在获取.
    /// </summary>
    Fetching,

    /// <summary>
    /// 正在下载.
    /// </summary>
    Downloading,

    /// <summary>
    /// 正在暂停.
    /// </summary>
    Paused,

    /// <summary>
    /// 出现错误.
    /// </summary>
    Error,

    /// <summary>
    /// 已完成.
    /// </summary>
    Completed,
}
