// Copyright (c) AI Downloader. All rights reserved.

using AIDownloader.Core.Models;
using AIDownloader.UI.Models.Constants;

namespace AIDownloader.UI.ViewModels;

/// <summary>
/// 下载条目视图模型.
/// </summary>
public sealed partial class DownloadItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _savePath;

    [ObservableProperty]
    private double _speed;

    [ObservableProperty]
    private double _maxLength;

    [ObservableProperty]
    private double _downloadedLength;

    [ObservableProperty]
    private bool _isFetching;

    [ObservableProperty]
    private bool _isDownloading;

    [ObservableProperty]
    private bool _isPaused;

    [ObservableProperty]
    private bool _isError;

    [ObservableProperty]
    private bool _isCompleted;

    [ObservableProperty]
    private DownloadStatus _status;

    /// <summary>
    /// Initializes a new instance of the <see cref="DownloadItemViewModel"/> class.
    /// </summary>
    public DownloadItemViewModel(DownloadItem item)
    {
        Name = item.FileName;
        SavePath = Path.Combine(item.TargetFolder, item.FileName);
        Status = DownloadStatus.Fetching;
        CheckStatus();
    }

    /// <summary>
    /// 下载标识符.
    /// </summary>
    public string Id { get; set; }

    private void CheckStatus()
    {
        IsFetching = Status == DownloadStatus.Fetching;
        IsDownloading = Status == DownloadStatus.Downloading;
        IsPaused = Status == DownloadStatus.Paused;
        IsError = Status == DownloadStatus.Error;
        IsCompleted = Status == DownloadStatus.Completed;
    }

    partial void OnStatusChanged(DownloadStatus value)
        => CheckStatus();
}
