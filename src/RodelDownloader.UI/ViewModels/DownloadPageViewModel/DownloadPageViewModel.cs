// Copyright (c) AI Downloader. All rights reserved.

using Microsoft.UI.Dispatching;
using RodelDownloader.Core;
using RodelDownloader.Core.Models;
using RodelDownloader.UI.Models.Constants;
using RodelDownloader.UI.Toolkits;
using Windows.ApplicationModel;

namespace RodelDownloader.UI.ViewModels;

/// <summary>
/// 下载页面视图模型.
/// </summary>
public sealed partial class DownloadPageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DownloadPageViewModel"/> class.
    /// </summary>
    private DownloadPageViewModel()
    {
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        _timer = new DispatcherTimer();
        CheckDownloadProgress();
        CheckDownloadEnabled();
        CheckPauseAllStatus();
        DownloadSpeed = 0;
        HasSpeed = false;
        SelectedSource = SettingsToolkit.ReadLocalSetting(SettingNames.LastDonwloadType, DownloadSource.HuggingFace);

        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += OnTimerTickAsync;

        AttachIsRunningToAsyncCommand(p => IsPreparingDownload = p, AddDownloadItemsCommand);
    }

    /// <summary>
    /// 获取下载器.
    /// </summary>
    /// <returns><see cref="Downloader"/>.</returns>
    public Downloader GetDownloader()
    {
        Initialize();
        return _downloader;
    }

    [RelayCommand]
    private void Initialize()
    {
        if (_downloader != null)
        {
            return;
        }

        _downloader = new Downloader();
        var ariaPath = Path.Combine(Package.Current.InstalledPath, "aria2c.exe");
        var configPath = Path.Combine(Package.Current.InstalledPath, "aria2.conf");
        _ariaClient = new Aria.AriaClient(ariaPath, configPath, 9600, retryCount: 5);
    }

    [RelayCommand]
    private void Clean()
    {
        _downloader?.Dispose();
        _ariaClient?.Dispose();
    }

    [RelayCommand]
    private void CheckDownloadEnabled()
    {
        IsDownloadEnabled = SelectedSource == DownloadSource.HuggingFace
            ? !string.IsNullOrEmpty(SettingsToolkit.ReadLocalSetting(SettingNames.HuggingFaceToken, string.Empty))
            : true;
    }

    [RelayCommand]
    private async Task AddDownloadItemsAsync(List<DownloadItem> items)
    {
        var availableItems = items.Where(item => !DownloadingItems.Any(p => p.SavePath == Path.Combine(item.TargetFolder, item.FileName))).ToList();
        var tasks = new List<Task>();
        var ariaConfigPath = Path.Combine(Package.Current.InstalledPath, "aria2.conf");
        var token = SelectedSource == DownloadSource.HuggingFace
            ? SettingsToolkit.ReadLocalSetting(SettingNames.HuggingFaceToken, string.Empty)
            : SettingsToolkit.ReadLocalSetting(SettingNames.CivitaiToken, string.Empty);
        foreach (var item in availableItems)
        {
            var task = Task.Run(async () =>
            {
                var vm = new DownloadItemViewModel(item, RemoveItem, PauseItem, ResumeItem);
                var fileName = item.FileName;
                var targetFolder = item.TargetFolder;
                var link = item.Link;
                var options = new Dictionary<string, object>
                    {
                        { "out", fileName },
                        { "dir", targetFolder },
                        { "config-path", $"\"{ariaConfigPath}\"" },
                    };

                if (!string.IsNullOrEmpty(token))
                {
                    if (SelectedSource == DownloadSource.HuggingFace || SelectedSource == DownloadSource.ModelScope)
                    {
                        options.Add("header", $"Authorization: Bearer {token}");
                    }
                    else if (SelectedSource == DownloadSource.Civitai)
                    {
                        link = link += $"?token={token}";
                    }
                }

                var id = await _ariaClient.AddUriAsync([link], options);
                _dispatcherQueue.TryEnqueue(() =>
                {
                    vm.Id = id;
                    DownloadingItems.Add(vm);
                });
            });
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
        CheckDownloadingItems();
        CheckDownloadedItems();
        _timer.Start();
    }

    [RelayCommand]
    private async Task ResumeAllAsync()
    {
        await _ariaClient.UnpauseAllAsync();
        foreach (var item in DownloadingItems.Where(p => p.IsPaused || p.IsError))
        {
            item.Status = DownloadStatus.Fetching;
        }
    }

    [RelayCommand]
    private async Task PauseAllAsync()
    {
        await _ariaClient.PauseAllAsync();
        foreach (var item in DownloadingItems.Where(p => p.IsDownloading))
        {
            item.Status = DownloadStatus.Paused;
        }
    }

    private void CheckPauseAllStatus()
    {
        CanPauseAll = DownloadingItems.Any(p => p.IsDownloading);
        CanResumeAll = DownloadingItems.Any(p => p.IsPaused || p.IsError);
    }

    private async void OnTimerTickAsync(object sender, object e)
    {
        var isAllCompleted = DownloadingItems.All(p => p.IsCompleted);
        if (isAllCompleted)
        {
            _timer.Stop();
            DownloadSpeed = 0;
            return;
        }

        var tasks = new List<Task>();
        foreach (var item in DownloadingItems)
        {
            if (item.IsDownloading || item.IsFetching)
            {
                var task = Task.Run(async () =>
                {
                    var i = item;
                    var status = await _ariaClient.TellStatusAsync(i.Id);
                    if (status == null)
                    {
                        return;
                    }

                    var maxValue = long.Parse(status.TotalLength);
                    var downloadedValue = long.Parse(status.CompletedLength);
                    var speed = long.Parse(status.DownloadSpeed);
                    var s = status.Status switch
                    {
                        "active" => DownloadStatus.Downloading,
                        "paused" => DownloadStatus.Paused,
                        "complete" => DownloadStatus.Completed,
                        "error" => DownloadStatus.Error,
                        _ => DownloadStatus.Fetching,
                    };

                    _dispatcherQueue.TryEnqueue(() =>
                    {
                        i.DownloadedLength = downloadedValue;
                        i.MaxLength = maxValue;
                        i.Status = s;
                        i.Speed = speed;

                        if (i.IsCompleted || i.IsError)
                        {
                            DownloadingItems.Remove(i);
                            DownloadedItems.Insert(0, i);
                        }

                        if (i.IsError)
                        {
                            var message = $"下载 {i.Name} 时发生错误: {status.ErrorMessage}";
                            Logger.Error(message);
                            i.ErrorMessage = status.ErrorMessage;
                        }
                    });
                });

                tasks.Add(task);
            }
        }

        var totalTask = Task.Run(async () =>
        {
            var status = await _ariaClient.GetGlobalStatAsync();
            if (status == null)
            {
                return;
            }

            var speed = long.Parse(status.DownloadSpeed);
            _dispatcherQueue.TryEnqueue(() =>
            {
                DownloadSpeed = speed;
                HasSpeed = speed > 0;
            });
        });

        await Task.WhenAll(tasks);
        CheckDownloadProgress();
        CheckDownloadedItems();
        CheckPauseAllStatus();
    }

    private void CheckDownloadProgress()
    {
        if (HasDownloadItems)
        {
            DownloadProgress = $"{DownloadedItems.Count}/{DownloadingItems.Count + DownloadedItems.Count}";
            DownloadProgressTip = string.Format(ResourceToolkit.GetLocalizedString(StringNames.DownloadProgressTipTemplate), DownloadingItems.Count, DownloadingItems.Count(p => p.IsCompleted));
        }
        else
        {
            DownloadProgress = "-/-";
            DownloadProgressTip = string.Empty;
        }
    }

    private void CheckDownloadingItems()
        => HasDownloadItems = DownloadingItems.Count > 0 || DownloadedItems.Count > 0;

    private void CheckDownloadedItems()
        => HasDownloadedItems = DownloadedItems.Count > 0;

    private void PauseItem(DownloadItemViewModel item)
    {
        Task.Run(async () =>
        {
            await _ariaClient.PauseAsync(item.Id);
        });
    }

    private void ResumeItem(DownloadItemViewModel item)
    {
        if (DownloadedItems.Contains(item))
        {
            DownloadedItems.Remove(item);
            DownloadingItems.Add(item);
        }

        Task.Run(async () =>
        {
            await _ariaClient.UnpauseAsync(item.Id);
        });
    }

    private void RemoveItem(DownloadItemViewModel item)
    {
        if (!item.IsCompleted)
        {
            Task.Run(async () =>
            {
                await _ariaClient.RemoveAsync(item.Id);
            });
        }

        DownloadingItems.Remove(item);
        DownloadedItems.Remove(item);
        CheckDownloadingItems();
        CheckDownloadedItems();
        CheckDownloadProgress();
        CheckPauseAllStatus();
    }

    partial void OnSelectedSourceChanged(DownloadSource value)
    {
        CheckDownloadEnabled();
        SettingsToolkit.WriteLocalSetting(SettingNames.LastDonwloadType, value);
    }
}
