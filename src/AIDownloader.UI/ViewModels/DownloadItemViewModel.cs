// Copyright (c) AI Downloader. All rights reserved.

using AIDownloader.Core.Models;
using AIDownloader.UI.Models.Constants;
using AIDownloader.UI.Toolkits;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.System;

namespace AIDownloader.UI.ViewModels;

/// <summary>
/// 下载条目视图模型.
/// </summary>
public sealed partial class DownloadItemViewModel : ViewModelBase
{
    private readonly Action<DownloadItemViewModel> _removeAction;
    private readonly Action<DownloadItemViewModel> _pauseAction;
    private readonly Action<DownloadItemViewModel> _resumeAction;

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
    private string _errorMessage;

    [ObservableProperty]
    private DownloadStatus _status;

    /// <summary>
    /// Initializes a new instance of the <see cref="DownloadItemViewModel"/> class.
    /// </summary>
    public DownloadItemViewModel(
        DownloadItem item,
        Action<DownloadItemViewModel> removeAction,
        Action<DownloadItemViewModel> pauseAction,
        Action<DownloadItemViewModel> resumeAction)
    {
        Name = item.FileName;
        Link = item.Link;
        SavePath = Path.Combine(item.TargetFolder, item.FileName);
        Status = DownloadStatus.Fetching;
        CheckStatus();
        _removeAction = removeAction;
        _pauseAction = pauseAction;
        _resumeAction = resumeAction;
    }

    /// <summary>
    /// 下载标识符.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 下载链接.
    /// </summary>
    public string Link { get; }

    [RelayCommand]
    private async Task OpenAsync()
    {
        if (Status == DownloadStatus.Completed)
        {
            var file = await StorageFile.GetFileFromPathAsync(SavePath);
            await Launcher.LaunchFileAsync(file);
        }
    }

    [RelayCommand]
    private void CopyLink()
    {
        var dp = new DataPackage();
        dp.SetText(Link);
        Clipboard.SetContent(dp);
        AppViewModel.Instance.ShowTip(ResourceToolkit.GetLocalizedString(StringNames.Copied), InfoType.Success);
    }

    [RelayCommand]
    private void Remove()
        => _removeAction?.Invoke(this);

    [RelayCommand]
    private void Pause()
    {
        Status = DownloadStatus.Paused;
        _pauseAction?.Invoke(this);
    }

    [RelayCommand]
    private void Resume()
    {
        Status = DownloadStatus.Fetching;
        _resumeAction?.Invoke(this);
    }

    [RelayCommand]
    private async Task OpenFolderAsync()
    {
        var folder = Path.GetDirectoryName(SavePath);
        await Launcher.LaunchFolderPathAsync(folder);
    }

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
