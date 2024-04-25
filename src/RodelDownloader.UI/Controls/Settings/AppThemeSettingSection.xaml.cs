// Copyright (c) AI Downloader. All rights reserved.

using RodelDownloader.UI.ViewModels;

namespace RodelDownloader.UI.Controls.Settings;

/// <summary>
/// 应用主题设置部分.
/// </summary>
public sealed partial class AppThemeSettingSection : SettingSectionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppThemeSettingSection"/> class.
    /// </summary>
    public AppThemeSettingSection()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var index = ViewModel.AppTheme switch
        {
            ElementTheme.Light => 0,
            ElementTheme.Dark => 1,
            _ => 2,
        };

        ThemePicker.SelectedIndex = index;
    }

    private void OnThemeSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var theme = ThemePicker.SelectedIndex switch
        {
            0 => ElementTheme.Light,
            1 => ElementTheme.Dark,
            _ => ElementTheme.Default,
        };

        ViewModel.AppTheme = theme;
        AppViewModel.Instance.ChangeTheme(theme);
    }
}
