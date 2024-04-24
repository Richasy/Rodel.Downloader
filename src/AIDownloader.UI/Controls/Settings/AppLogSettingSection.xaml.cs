// Copyright (c) AI Downloader. All rights reserved.

using Windows.Storage;
using Windows.System;

namespace AIDownloader.UI.Controls.Settings;

/// <summary>
/// 日志设置.
/// </summary>
public sealed partial class AppLogSettingSection : SettingSectionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppLogSettingSection"/> class.
    /// </summary>
    public AppLogSettingSection()
    {
        InitializeComponent();
    }

    private async void OnItemClickAsync(object sender, RoutedEventArgs e)
    {
        var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Logger", CreationCollisionOption.OpenIfExists).AsTask();
        _ = await Launcher.LaunchFolderAsync(folder);
    }
}
