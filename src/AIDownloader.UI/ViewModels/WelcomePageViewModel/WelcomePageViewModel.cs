// Copyright (c) AI Downloader. All rights reserved.

using System.Text.Json;
using AIDownloader.UI.Models.Constants;
using AIDownloader.UI.Toolkits;
using Microsoft.Windows.AppLifecycle;

namespace AIDownloader.UI.ViewModels;

/// <summary>
/// 欢迎页视图模型.
/// </summary>
public sealed partial class WelcomePageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WelcomePageViewModel"/> class.
    /// </summary>
    public WelcomePageViewModel()
    {
        StepCount = 5;
        CurrentStep = 0;
        CheckStep();
        CheckHuggingFaceSaveFolderEmpty();
        CheckCivitaiSaveFolderEmpty();
        CheckModelScopeSaveFoldersEmpty();
    }

    [RelayCommand]
    private void Restart()
    {
        try
        {
            if (CurrentStep >= 1)
            {
                WriteSettings();
            }

            SettingsToolkit.WriteLocalSetting(SettingNames.SkipWelcome, true);
            AppInstance.GetCurrent().UnregisterKey();
            _ = AppInstance.Restart(default);
        }
        catch (Exception ex)
        {
            AppViewModel.Instance.ShowTip(ex.Message, InfoType.Error);
            LogException(ex);
        }
    }

    [RelayCommand]
    private void GoNext()
    {
        if (CurrentStep < StepCount - 1)
        {
            CurrentStep++;
        }
        else
        {
            Restart();
        }
    }

    [RelayCommand]
    private void GoPrev()
        => CurrentStep--;

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

        CheckModelScopeSaveFoldersEmpty();
    }

    [RelayCommand]
    private async Task ImportConfigurationAsync()
    {
        var isSuccess = await AppViewModel.Instance.ImportConfigurationAsync();
        if (isSuccess)
        {
            Restart();
        }
    }

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
        CheckModelScopeSaveFoldersEmpty();
    }

    private void WriteSettings()
    {
        SettingsToolkit.WriteLocalSetting(SettingNames.HuggingFaceToken, HuggingFaceToken ?? string.Empty);
        SettingsToolkit.WriteLocalSetting(SettingNames.CivitaiToken, CivitaiToken ?? string.Empty);
        SettingsToolkit.WriteLocalSetting(SettingNames.ModelScopeToken, ModelScopeToken ?? string.Empty);

        var hfFolderList = HuggingFaceSaveFolders.Select(p => p.GetData()).ToList();
        SettingsToolkit.WriteLocalSetting(SettingNames.HuggingFaceSaveFolders, JsonSerializer.Serialize(hfFolderList));
        var cvFolderList = CivitaiSaveFolders.Select(p => p.GetData()).ToList();
        SettingsToolkit.WriteLocalSetting(SettingNames.CivitaiSaveFolders, JsonSerializer.Serialize(cvFolderList));
        var msFolderList = ModelScopeSaveFolders.Select(p => p.GetData()).ToList();
        SettingsToolkit.WriteLocalSetting(SettingNames.ModelScopeSaveFolders, JsonSerializer.Serialize(msFolderList));
    }

    private void CheckHuggingFaceSaveFolderEmpty()
        => IsHuggingFaceFoldersEmpty = HuggingFaceSaveFolders.Count == 0;

    private void CheckCivitaiSaveFolderEmpty()
        => IsCivitaiFoldersEmpty = CivitaiSaveFolders.Count == 0;

    private void CheckModelScopeSaveFoldersEmpty()
        => IsModelScopeFoldersEmpty = ModelScopeSaveFolders.Count == 0;

    private void CheckStep()
    {
        IsFREStep = CurrentStep == 0;
        IsHuggingFaceStep = CurrentStep == 1;
        IsCivitaiStep = CurrentStep == 2;
        IsModelScopeStep = CurrentStep == 3;
        IsLastStep = CurrentStep == StepCount - 1;
        IsPreviousStepShown = CurrentStep > 1 && !IsLastStep;
    }

    partial void OnCurrentStepChanged(int value)
        => CheckStep();
}
