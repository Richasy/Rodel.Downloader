// Copyright (c) AI Downloader. All rights reserved.

using RodelDownloader.UI.Controls;
using RodelDownloader.UI.Models.Constants;
using RodelDownloader.UI.ViewModels;

namespace RodelDownloader.UI.Pages;

/// <summary>
/// 下载页面.
/// </summary>
public sealed partial class DownloadPage : DownloadPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DownloadPage"/> class.
    /// </summary>
    public DownloadPage()
    {
        InitializeComponent();
        ViewModel = DownloadPageViewModel.Instance;
    }

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        ViewModel.CheckDownloadEnabledCommand.Execute(default);
        DownloadTypeSelector.SelectedIndex = (int)ViewModel.SelectedSource;
    }

    private void OnDownloadTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var index = DownloadTypeSelector.SelectedIndex;
        if (index == -1)
        {
            return;
        }

        ViewModel.SelectedSource = (DownloadSource)index;
    }

    private async void OnDownloadButtonClickAsync(object sender, RoutedEventArgs e)
    {
        ViewModel.InitializeCommand.Execute(default);
        if (ViewModel.SelectedSource == DownloadSource.HuggingFace)
        {
            // Show hugging face download dialog.
            var dialog = new HuggingFaceDownloadDialog(ViewModel);
            dialog.XamlRoot = XamlRoot;
            await dialog.ShowAsync();
        }
        else if (ViewModel.SelectedSource == DownloadSource.Civitai)
        {
            // Show civitai download dialog.
            var dialog = new CivitaiDownloadDialog(ViewModel);
            dialog.XamlRoot = XamlRoot;
            await dialog.ShowAsync();
        }
        else if (ViewModel.SelectedSource == DownloadSource.ModelScope)
        {
            // Show civitai download dialog.
            var dialog = new ModelScopeDownloadDialog(ViewModel);
            dialog.XamlRoot = XamlRoot;
            await dialog.ShowAsync();
        }
    }

    private void OnSettingsButtonClick(object sender, RoutedEventArgs e)
        => CoreViewModel.Navigate(typeof(SettingsPage));
}

/// <summary>
/// <see cref="DownloadPage"/> 的基类.
/// </summary>
public abstract class DownloadPageBase : PageBase<DownloadPageViewModel>
{
}
