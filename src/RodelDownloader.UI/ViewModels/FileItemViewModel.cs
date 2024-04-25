// Copyright (c) AI Downloader. All rights reserved.

using RodelDownloader.Core.Models;

namespace RodelDownloader.UI.ViewModels;

/// <summary>
/// 文件项视图模型.
/// </summary>
public sealed partial class FileItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private bool _isSelected;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileItemViewModel"/> class.
    /// </summary>
    /// <param name="item">下载条目.</param>
    public FileItemViewModel(DownloadItem item)
    {
        Data = item;
        Name = item.FileName;
        IsSelected = true;
    }

    /// <summary>
    /// 数据.
    /// </summary>
    public DownloadItem Data { get; }
}
