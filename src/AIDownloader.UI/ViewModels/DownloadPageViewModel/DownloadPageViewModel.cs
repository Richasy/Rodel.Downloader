// Copyright (c) AI Downloader. All rights reserved.

using AIDownloader.UI.Models.Constants;
using AIDownloader.UI.Toolkits;

namespace AIDownloader.UI.ViewModels;

/// <summary>
/// 下载页面视图模型.
/// </summary>
public sealed partial class DownloadPageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DownloadPageViewModel"/> class.
    /// </summary>
    public DownloadPageViewModel()
    {
        CheckDownloadProgress();
        CheckDownloadSpeed();
        SelectedSource = SettingsToolkit.ReadLocalSetting(SettingNames.LastDonwloadType, DownloadSource.HuggingFace);
    }

    private void CheckDownloadProgress()
    {
        if (HasDownloadItems)
        {
            // Check file counts.
        }
        else
        {
            DownloadProgress = "-/-";
            DownloadProgressTip = string.Empty;
        }
    }

    private void CheckDownloadSpeed()
    {
        if (HasDownloadItems)
        {
            // Check download speed.
        }
        else
        {
            DownloadSpeed = 0;
            HasSpeed = false;
        }
    }

    partial void OnSelectedSourceChanged(DownloadSource value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.LastDonwloadType, value);
}
