// Copyright (c) AI Downloader. All rights reserved.

using AIDownloader.Aria;
using AIDownloader.Core;
using AIDownloader.UI.Models.Constants;
using Microsoft.UI.Dispatching;

namespace AIDownloader.UI.ViewModels;

/// <summary>
/// 下载页面视图模型.
/// </summary>
public sealed partial class DownloadPageViewModel
{
    private readonly DispatcherQueue _dispatcherQueue;
    private readonly DispatcherTimer _timer;
    private AriaClient _ariaClient;
    private Downloader _downloader;

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

    [ObservableProperty]
    private bool _isDownloadEnabled;

    [ObservableProperty]
    private bool _isPreparingDownload;

    [ObservableProperty]
    private bool _canPauseAll;

    [ObservableProperty]
    private bool _canResumeAll;

    /// <summary>
    /// 实例.
    /// </summary>
    public static DownloadPageViewModel Instance { get; } = new DownloadPageViewModel();

    /// <summary>
    /// 下载列表.
    /// </summary>
    public ObservableCollection<DownloadItemViewModel> Items { get; } = new();
}
