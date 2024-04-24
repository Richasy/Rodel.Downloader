// Copyright (c) AI Downloader. All rights reserved.

using AIDownloader.UI.ViewModels;

namespace AIDownloader.UI.Pages;

/// <summary>
/// 设置页面.
/// </summary>
public sealed partial class SettingsPage : SettingsPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsPage"/> class.
    /// </summary>
    public SettingsPage()
    {
        InitializeComponent();
        ViewModel = SettingsPageViewModel.Instance;
    }

    /// <inheritdoc/>
    protected override void OnNavigatedTo(NavigationEventArgs e)
        => CoreViewModel.IsBackButtonShown = true;

    /// <inheritdoc/>
    protected override void OnNavigatedFrom(NavigationEventArgs e)
        => CoreViewModel.IsBackButtonShown = false;

    /// <inheritdoc/>
    protected override void OnPageUnloaded()
        => ViewModel.BeforeExitCommand.Execute(default);
}

/// <summary>
/// <see cref="SettingsPage"/> 的基类.
/// </summary>
public abstract class SettingsPageBase : PageBase<SettingsPageViewModel>
{
}
