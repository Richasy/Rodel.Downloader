// Copyright (c) AI Downloader. All rights reserved.

using AIDownloader.Core;
using AIDownloader.UI.Models.Constants;
using AIDownloader.UI.Toolkits;
using Windows.ApplicationModel;

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

    [RelayCommand]
    private void Initialize()
    {
        if (_downloader != null)
        {
            return;
        }

        _downloader = new Downloader();
        var ariaPath = Path.Combine(Package.Current.InstalledPath, "lib", "aria2c.exe");
        var configPath = Path.Combine(Package.Current.InstalledPath, "lib", "aria2.conf");
        _ariaClient = new Aria.AriaClient(ariaPath, configPath, 9600, retryCount: 5);
    }

    [RelayCommand]
    private void Clean()
    {
        _downloader?.Dispose();
        _ariaClient?.Dispose();
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
