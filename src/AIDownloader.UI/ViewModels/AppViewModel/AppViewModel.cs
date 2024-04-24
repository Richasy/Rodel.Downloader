// Copyright (c) AI Downloader. All rights reserved.

using System.Text.Json;
using AIDownloader.Core;
using AIDownloader.Core.Models;
using AIDownloader.UI.Forms;
using AIDownloader.UI.Models;
using AIDownloader.UI.Models.Args;
using AIDownloader.UI.Models.Constants;
using AIDownloader.UI.Toolkits;
using Windows.Storage;

namespace AIDownloader.UI.ViewModels;

/// <summary>
/// 应用视图模型.
/// </summary>
public sealed partial class AppViewModel : ViewModelBase
{
    /// <summary>
    /// 在应用退出前执行.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public Task BeforeExitAsync()
    {
        if (ActivatedWindow is MainWindow)
        {
            DownloadPageViewModel.Instance.CleanCommand.Execute(default);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// 显示提示.
    /// </summary>
    /// <param name="message">提示内容.</param>
    /// <param name="type">提示类型.</param>
    public void ShowTip(string message, InfoType type = InfoType.Information)
        => RequestShowTip?.Invoke(this, new AppTipNotification(message, type, ActivatedWindow));

    /// <summary>
    /// 显示提示.
    /// </summary>
    /// <param name="messageName">提示内容.</param>
    /// <param name="type">提示类型.</param>
    public void ShowTip(StringNames messageName, InfoType type = InfoType.Information)
        => RequestShowTip?.Invoke(this, new AppTipNotification(ResourceToolkit.GetLocalizedString(messageName), type, ActivatedWindow));

    /// <summary>
    /// 修改主题.
    /// </summary>
    /// <param name="theme">主题类型.</param>
    public void ChangeTheme(ElementTheme theme)
    {
        if (ActivatedWindow == null)
        {
            return;
        }

        (ActivatedWindow.Content as FrameworkElement).RequestedTheme = theme;
        if (theme == ElementTheme.Dark)
        {
            ActivatedWindow.AppWindow.TitleBar.ButtonForegroundColor = Colors.White;
        }
        else if (theme == ElementTheme.Light)
        {
            ActivatedWindow.AppWindow.TitleBar.ButtonForegroundColor = Colors.Black;
        }
        else
        {
            ActivatedWindow.AppWindow.TitleBar.ButtonForegroundColor = default;
        }
    }

    /// <summary>
    /// 导航到指定页面.
    /// </summary>
    /// <param name="page">页面类型.</param>
    /// <param name="parameter">参数.</param>
    public void Navigate(Type page, object parameter = null)
        => RequestNavigate?.Invoke(this, new AppNavigateEventArgs(page, parameter));

    /// <summary>
    /// 导入配置.
    /// </summary>
    /// <returns>是否成功.</returns>
    public async Task<bool> ImportConfigurationAsync()
    {
        var file = await FileToolkit.PickFileAsync(".json", ActivatedWindow);
        if (file == null)
        {
            return false;
        }

        var content = await FileIO.ReadTextAsync(file);
        try
        {
            var config = JsonSerializer.Deserialize<DownloaderConfig>(content);
            if (config == null)
            {
                ShowTip(StringNames.ImportConfigFailed, InfoType.Error);
                return false;
            }

            SettingsToolkit.WriteLocalSetting(SettingNames.CivitaiToken, config.CivitaiToken);
            SettingsToolkit.WriteLocalSetting(SettingNames.HuggingFaceToken, config.HuggingFaceToken);
            SettingsToolkit.WriteLocalSetting(SettingNames.ModelScopeToken, config.ModelScopeToken);

            var hfFolders = GetFolders(config.HuggingFaceSaveFolder, config.HuggingFaceBackupFolders);
            var msFolders = GetFolders(config.ModelScopeSaveFolder, config.ModelScopeBackupFolders);
            var civitaiFolders = GetFolders(config.CivitaiSaveFolder, config.CivitaiBackupFolders);

            SettingsToolkit.WriteLocalSetting(SettingNames.HuggingFaceSaveFolders, JsonSerializer.Serialize(hfFolders));
            SettingsToolkit.WriteLocalSetting(SettingNames.ModelScopeSaveFolders, JsonSerializer.Serialize(msFolders));
            SettingsToolkit.WriteLocalSetting(SettingNames.CivitaiSaveFolders, JsonSerializer.Serialize(civitaiFolders));

            var hfUri = (config.HuggingFaceUriType ?? string.Empty).Equals("mirror", StringComparison.InvariantCultureIgnoreCase) ? HuggingFaceUriType.Mirror : HuggingFaceUriType.Official;
            SettingsToolkit.WriteLocalSetting(SettingNames.HuggingFaceUriType, hfUri);
            ShowTip(StringNames.ImportCompleted, InfoType.Success);
            return true;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Import configuration failed.");
            return false;
        }

        List<FolderItem> GetFolders(string preferFolder, Dictionary<string, string> folders)
        {
            var folderList = new List<FolderItem>();
            if (!string.IsNullOrEmpty(preferFolder))
            {
                folderList.Add(new FolderItem
                {
                    Name = Path.GetFileName(preferFolder),
                    Path = preferFolder,
                });
            }

            if (folders is Dictionary<string, string> hfConfigFolders && hfConfigFolders.Count > 0)
            {
                foreach (var item in hfConfigFolders)
                {
                    if (folderList.Any(p => p.Path.Equals(item.Value)))
                    {
                        continue;
                    }

                    folderList.Add(new FolderItem
                    {
                        Name = item.Key,
                        Path = item.Value,
                    });
                }
            }

            return folderList;
        }
    }

    /// <summary>
    /// 导出配置.
    /// </summary>
    /// <returns>是否成功.</returns>
    public async Task<bool> ExportConfigurationAsync()
    {
        var file = await FileToolkit.SaveFileAsync(".json", ActivatedWindow);
        if (file == null)
        {
            return false;
        }

        var config = new DownloaderConfig();
        config.CivitaiToken = SettingsToolkit.ReadLocalSetting(SettingNames.CivitaiToken, string.Empty);
        config.HuggingFaceToken = SettingsToolkit.ReadLocalSetting(SettingNames.HuggingFaceToken, string.Empty);
        config.ModelScopeToken = SettingsToolkit.ReadLocalSetting(SettingNames.ModelScopeToken, string.Empty);
        config.HuggingFaceUriType = SettingsToolkit.ReadLocalSetting(SettingNames.HuggingFaceUriType, HuggingFaceUriType.Official).ToString().ToLower();

        var hfFolders = JsonSerializer.Deserialize<List<FolderItem>>(SettingsToolkit.ReadLocalSetting(SettingNames.HuggingFaceSaveFolders, string.Empty));
        var msFolders = JsonSerializer.Deserialize<List<FolderItem>>(SettingsToolkit.ReadLocalSetting(SettingNames.ModelScopeSaveFolders, string.Empty));
        var civitaiFolders = JsonSerializer.Deserialize<List<FolderItem>>(SettingsToolkit.ReadLocalSetting(SettingNames.CivitaiSaveFolders, string.Empty));

        if (hfFolders.Count == 1)
        {
            config.HuggingFaceSaveFolder = hfFolders[0].Path;
        }
        else if (hfFolders.Count > 1)
        {
            config.HuggingFaceBackupFolders = hfFolders.ToDictionary(p => p.Name, p => p.Path);
        }

        if (msFolders.Count == 1)
        {
            config.ModelScopeSaveFolder = msFolders[0].Path;
        }
        else if (msFolders.Count > 1)
        {
            config.ModelScopeBackupFolders = msFolders.ToDictionary(p => p.Name, p => p.Path);
        }

        if (civitaiFolders.Count == 1)
        {
            config.CivitaiSaveFolder = civitaiFolders[0].Path;
        }
        else if (civitaiFolders.Count > 1)
        {
            config.CivitaiBackupFolders = civitaiFolders.ToDictionary(p => p.Name, p => p.Path);
        }

        var content = JsonSerializer.Serialize(config);
        await FileIO.WriteTextAsync(file, content);
        ShowTip(StringNames.ExportCompleted, InfoType.Success);

        return true;
    }
}
