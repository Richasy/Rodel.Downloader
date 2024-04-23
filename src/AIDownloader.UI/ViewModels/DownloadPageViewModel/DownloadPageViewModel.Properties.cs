// Copyright (c) AI Downloader. All rights reserved.

using AIDownloader.UI.Models.Constants;

namespace AIDownloader.UI.ViewModels;

/// <summary>
/// 下载页面视图模型.
/// </summary>
public sealed partial class DownloadPageViewModel
{
    [ObservableProperty]
    private DownloadSource _selectedSource;

    [ObservableProperty]
    private string _downloadProgress;

    [ObservableProperty]
    private string _downloadProgressTip;

    [ObservableProperty]
    private bool _hasDownloadItems;

    [ObservableProperty]
    private long _downloadSpeed;

    [ObservableProperty]
    private bool _hasSpeed;

    /// <summary>
    /// 实例.
    /// </summary>
    public static DownloadPageViewModel Instance { get; } = new DownloadPageViewModel();
}
