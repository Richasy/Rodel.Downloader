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
        StepCount = 4;
        CurrentStep = 0;
        CheckStep();
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

    private void WriteSettings()
    {
        SettingsToolkit.WriteLocalSetting(SettingNames.HuggingFaceToken, HuggingFaceToken ?? string.Empty);
        SettingsToolkit.WriteLocalSetting(SettingNames.CivitaiToken, CivitaiToken ?? string.Empty);

        var hfFolderList = HuggingFaceSaveFolders.Select(p => p.GetData()).ToList();
        SettingsToolkit.WriteLocalSetting(SettingNames.HuggingFaceSaveFolders, JsonSerializer.Serialize(hfFolderList));
        var cvFolderList = CivitaiSaveFolders.Select(p => p.GetData()).ToList();
        SettingsToolkit.WriteLocalSetting(SettingNames.CivitaiSaveFolders, JsonSerializer.Serialize(cvFolderList));
    }

    private void CheckStep()
    {
        IsFREStep = CurrentStep == 0;
        IsHuggingFaceStep = CurrentStep == 1;
        IsCivitaiStep = CurrentStep == 2;
        IsLastStep = CurrentStep == StepCount - 1;
        IsPreviousStepShown = CurrentStep > 1 && !IsLastStep;
    }

    partial void OnCurrentStepChanged(int value)
        => CheckStep();
}
