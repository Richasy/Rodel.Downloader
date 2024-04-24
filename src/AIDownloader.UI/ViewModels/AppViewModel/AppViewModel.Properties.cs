// Copyright (c) AI Downloader. All rights reserved.

using AIDownloader.UI.Models.Args;

namespace AIDownloader.UI.ViewModels;

/// <summary>
/// 应用视图模型.
/// </summary>
public sealed partial class AppViewModel
{
    [ObservableProperty]
    private Window _activatedWindow;

    [ObservableProperty]
    private bool _isBackButtonShown;

    /// <summary>
    /// 在有新的提示请求时触发.
    /// </summary>
    public event EventHandler<AppTipNotification> RequestShowTip;

    /// <summary>
    /// 在有导航请求时触发.
    /// </summary>
    public event EventHandler<AppNavigateEventArgs> RequestNavigate;

    /// <summary>
    /// 实例.
    /// </summary>
    public static AppViewModel Instance { get; } = new();
}
