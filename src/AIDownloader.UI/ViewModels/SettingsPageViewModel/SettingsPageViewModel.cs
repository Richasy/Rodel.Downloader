// Copyright (c) AI Downloader. All rights reserved.

using System.Text.Json;
using AIDownloader.UI.Models;
using AIDownloader.UI.Models.Constants;
using AIDownloader.UI.Toolkits;
using Microsoft.Windows.AppLifecycle;

namespace AIDownloader.UI.ViewModels;

/// <summary>
/// 设置页面视图模型.
/// </summary>
public sealed partial class SettingsPageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsPageViewModel"/> class.
    /// </summary>
    private SettingsPageViewModel()
        => Initialize();

    [RelayCommand]
    private void Initialize()
    {
        AppTheme = SettingsToolkit.ReadLocalSetting(SettingNames.AppTheme, ElementTheme.Default);
        AppVersion = AppToolkit.GetPackageVersion();
        var copyrightTemplate = ResourceToolkit.GetLocalizedString(StringNames.CopyrightTemplate);
        Copyright = string.Format(copyrightTemplate, BuildYear);
        HuggingFaceToken = SettingsToolkit.ReadLocalSetting(SettingNames.HuggingFaceToken, string.Empty);
        CivitaiToken = SettingsToolkit.ReadLocalSetting(SettingNames.CivitaiToken, string.Empty);
        ModelScopeToken = SettingsToolkit.ReadLocalSetting(SettingNames.ModelScopeToken, string.Empty);
        LoadHuggingFaceSaveFolders();
        LoadCivitaiSaveFolders();
        LoadModelScopeSaveFolders();
    }

    [RelayCommand]
    private void BeforeExit()
    {
        var hfFolderList = HuggingFaceSaveFolders.Select(p => p.GetData()).ToList();
        SettingsToolkit.WriteLocalSetting(SettingNames.HuggingFaceSaveFolders, JsonSerializer.Serialize(hfFolderList));
        var cvFolderList = CivitaiSaveFolders.Select(p => p.GetData()).ToList();
        SettingsToolkit.WriteLocalSetting(SettingNames.CivitaiSaveFolders, JsonSerializer.Serialize(cvFolderList));
        var msFolderList = ModelScopeSaveFolders.Select(p => p.GetData()).ToList();
        SettingsToolkit.WriteLocalSetting(SettingNames.ModelScopeSaveFolders, JsonSerializer.Serialize(msFolderList));
    }

    [RelayCommand]
    private async Task AddHuggingFaceSaveFolderAsync()
    {
        var folder = await FileToolkit.PickFolderAsync(AppViewModel.Instance.ActivatedWindow);
        if (folder != null && !HuggingFaceSaveFolders.Any(p => p.Path == folder.Path))
        {
            HuggingFaceSaveFolders.Add(new FolderItemViewModel(folder.Path, RemoveHuggingFaceSaveFolder));
        }

        CheckHuggingFaceSaveFolderEmpty();
    }

    [RelayCommand]
    private async Task AddCivitaiSaveFolderAsync()
    {
        var folder = await FileToolkit.PickFolderAsync(AppViewModel.Instance.ActivatedWindow);
        if (folder != null && !CivitaiSaveFolders.Any(p => p.Path == folder.Path))
        {
            CivitaiSaveFolders.Add(new FolderItemViewModel(folder.Path, RemoveCivitaiSaveFolder));
        }

        CheckCivitaiSaveFolderEmpty();
    }

    [RelayCommand]
    private async Task AddModelScopeSaveFolderAsync()
    {
        var folder = await FileToolkit.PickFolderAsync(AppViewModel.Instance.ActivatedWindow);
        if (folder != null && !ModelScopeSaveFolders.Any(p => p.Path == folder.Path))
        {
            ModelScopeSaveFolders.Add(new FolderItemViewModel(folder.Path, RemoveModelScopeSaveFolder));
        }

        CheckModelScopeSaveFolderEmpty();
    }

    [RelayCommand]
    private async Task ImportConfigurationAsync()
    {
        var isSuccess = await AppViewModel.Instance.ImportConfigurationAsync();
        if (isSuccess)
        {
            AppInstance.GetCurrent().UnregisterKey();
            _ = AppInstance.Restart(default);
        }
    }

    [RelayCommand]
    private Task ExportConfigurationAsync()
        => AppViewModel.Instance.ExportConfigurationAsync();

    private void LoadHuggingFaceSaveFolders()
    {
        HuggingFaceSaveFolders.Clear();
        var folders = JsonSerializer.Deserialize<List<FolderItem>>(SettingsToolkit.ReadLocalSetting(SettingNames.HuggingFaceSaveFolders, "[]"));
        foreach (var folder in folders)
        {
            HuggingFaceSaveFolders.Add(new FolderItemViewModel(folder, RemoveHuggingFaceSaveFolder));
        }

        CheckHuggingFaceSaveFolderEmpty();
    }

    private void LoadCivitaiSaveFolders()
    {
        CivitaiSaveFolders.Clear();
        var folders = JsonSerializer.Deserialize<List<FolderItem>>(SettingsToolkit.ReadLocalSetting(SettingNames.CivitaiSaveFolders, "[]"));
        foreach (var folder in folders)
        {
            CivitaiSaveFolders.Add(new FolderItemViewModel(folder, RemoveCivitaiSaveFolder));
        }

        CheckCivitaiSaveFolderEmpty();
    }

    private void LoadModelScopeSaveFolders()
    {
        ModelScopeSaveFolders.Clear();
        var folders = JsonSerializer.Deserialize<List<FolderItem>>(SettingsToolkit.ReadLocalSetting(SettingNames.ModelScopeSaveFolders, "[]"));
        foreach (var folder in folders)
        {
            ModelScopeSaveFolders.Add(new FolderItemViewModel(folder, RemoveModelScopeSaveFolder));
        }

        CheckModelScopeSaveFolderEmpty();
    }

    private void CheckHuggingFaceSaveFolderEmpty()
        => IsHuggingFaceFoldersEmpty = HuggingFaceSaveFolders.Count == 0;

    private void CheckCivitaiSaveFolderEmpty()
        => IsCivitaiFoldersEmpty = CivitaiSaveFolders.Count == 0;

    private void CheckModelScopeSaveFolderEmpty()
        => IsModelScopeFoldersEmpty = ModelScopeSaveFolders.Count == 0;

    private void RemoveHuggingFaceSaveFolder(FolderItemViewModel folder)
    {
        HuggingFaceSaveFolders.Remove(folder);
        CheckHuggingFaceSaveFolderEmpty();
    }

    private void RemoveCivitaiSaveFolder(FolderItemViewModel folder)
    {
        CivitaiSaveFolders.Remove(folder);
        CheckCivitaiSaveFolderEmpty();
    }

    private void RemoveModelScopeSaveFolder(FolderItemViewModel folder)
    {
        ModelScopeSaveFolders.Remove(folder);
        CheckModelScopeSaveFolderEmpty();
    }

    partial void OnHuggingFaceTokenChanged(string value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.HuggingFaceToken, value);

    partial void OnCivitaiTokenChanged(string value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.CivitaiToken, value);

    partial void OnModelScopeTokenChanged(string value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.ModelScopeToken, value);

    partial void OnAppThemeChanged(ElementTheme value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.AppTheme, value);
}
