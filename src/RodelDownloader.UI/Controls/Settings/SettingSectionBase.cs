// Copyright (c) AI Downloader. All rights reserved.

using RodelDownloader.UI.ViewModels;

namespace RodelDownloader.UI.Controls.Settings;

/// <summary>
/// 设置页面控件的基类.
/// </summary>
public abstract class SettingSectionBase : ReactiveUserControl<SettingsPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingSectionBase"/> class.
    /// </summary>
    public SettingSectionBase()
    {
        ViewModel = SettingsPageViewModel.Instance;
        IsTabStop = false;
    }
}
