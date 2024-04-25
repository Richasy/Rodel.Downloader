// Copyright (c) AI Downloader. All rights reserved.

using Windows.System;

namespace RodelDownloader.UI.Controls.Settings;

/// <summary>
/// 关于此应用.
/// </summary>
public sealed partial class AboutSettingSection : SettingSectionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AboutSettingSection"/> class.
    /// </summary>
    public AboutSettingSection() => InitializeComponent();

    private void OnOtherProjectsClick(object sender, RoutedEventArgs e)
    {
        var link = $"ms-windows-store://publisher/?name=云之幻";
        _ = Launcher.LaunchUriAsync(new Uri(link));
    }
}
