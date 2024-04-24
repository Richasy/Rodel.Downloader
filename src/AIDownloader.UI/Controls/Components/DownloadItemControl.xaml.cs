// Copyright (c) AI Downloader. All rights reserved.

using AIDownloader.UI.ViewModels;

namespace AIDownloader.UI.Controls.Components;

/// <summary>
/// 下载项控件.
/// </summary>
public sealed partial class DownloadItemControl : DownloadItemControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DownloadItemControl"/> class.
    /// </summary>
    public DownloadItemControl()
    {
        InitializeComponent();
    }
}

/// <summary>
/// <see cref="DownloadItemControl"/> 的基类.
/// </summary>
public abstract class DownloadItemControlBase : ReactiveUserControl<DownloadItemViewModel>
{
}
