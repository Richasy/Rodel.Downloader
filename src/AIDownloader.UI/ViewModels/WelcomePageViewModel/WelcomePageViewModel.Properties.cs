// Copyright (c) AI Downloader. All rights reserved.

namespace AIDownloader.UI.ViewModels;

/// <summary>
/// 欢迎页视图模型.
/// </summary>
public sealed partial class WelcomePageViewModel
{
    [ObservableProperty]
    private int _currentStep;

    [ObservableProperty]
    private int _stepCount;

    [ObservableProperty]
    private bool _isFREStep;

    [ObservableProperty]
    private bool _isHuggingFaceStep;

    [ObservableProperty]
    private bool _isCivitaiStep;

    [ObservableProperty]
    private bool _isModelScopeStep;

    [ObservableProperty]
    private bool _isLastStep;

    [ObservableProperty]
    private bool _isPreviousStepShown;

    [ObservableProperty]
    private string _huggingFaceToken;

    [ObservableProperty]
    private string _civitaiToken;

    [ObservableProperty]
    private string _modelScopeToken;

    [ObservableProperty]
    private bool _isHuggingFaceFoldersEmpty;

    [ObservableProperty]
    private bool _isCivitaiFoldersEmpty;

    [ObservableProperty]
    private bool _isModelScopeFoldersEmpty;

    /// <summary>
    /// 实例.
    /// </summary>
    public static WelcomePageViewModel Instance { get; } = new();

    /// <summary>
    /// Hugging Face 保存文件夹.
    /// </summary>
    public ObservableCollection<FolderItemViewModel> HuggingFaceSaveFolders { get; } = new();

    /// <summary>
    /// Civitai 保存文件夹.
    /// </summary>
    public ObservableCollection<FolderItemViewModel> CivitaiSaveFolders { get; } = new();

    /// <summary>
    /// 魔搭保存文件夹.
    /// </summary>
    public ObservableCollection<FolderItemViewModel> ModelScopeSaveFolders { get; } = new();
}
