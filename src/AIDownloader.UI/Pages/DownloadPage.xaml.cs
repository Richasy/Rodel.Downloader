// Copyright (c) AI Downloader. All rights reserved.

using AIDownloader.UI.Models.Constants;
using AIDownloader.UI.ViewModels;

namespace AIDownloader.UI.Pages;

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

    private void OnDownloadButtonClick(object sender, RoutedEventArgs e)
    {
        ViewModel.InitializeCommand.Execute(default);
        if (ViewModel.SelectedSource == DownloadSource.HuggingFace)
        {
            // Show hugging face download dialog.
        }
        else
        {
            // Show civitai download dialog.
        }
    }
}

/// <summary>
/// <see cref="DownloadPage"/> 的基类.
/// </summary>
public abstract class DownloadPageBase : PageBase<DownloadPageViewModel>
{
}
