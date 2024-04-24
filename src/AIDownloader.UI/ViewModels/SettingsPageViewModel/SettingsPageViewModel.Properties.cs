// Copyright (c) AI Downloader. All rights reserved.

namespace AIDownloader.UI.ViewModels;

/// <summary>
/// 设置页面视图模型.
/// </summary>
public sealed partial class SettingsPageViewModel : ViewModelBase
{
    private const int BuildYear = 2024;

    [ObservableProperty]
    private ElementTheme _appTheme;

    [ObservableProperty]
    private string _appThemeText;

    [ObservableProperty]
    private string _appVersion;

    [ObservableProperty]
    private string _copyright;

    [ObservableProperty]
    private string _huggingFaceToken;

    [ObservableProperty]
    private string _civitaiToken;

    [ObservableProperty]
    private bool _isHuggingFaceFoldersEmpty;

    [ObservableProperty]
    private bool _isCivitaiFoldersEmpty;

    /// <summary>
    /// 实例.
    /// </summary>
    public static SettingsPageViewModel Instance { get; } = new();

    /// <summary>
    /// Hugging Face 保存文件夹.
    /// </summary>
    public ObservableCollection<FolderItemViewModel> HuggingFaceSaveFolders { get; } = new();

    /// <summary>
    /// Civitai 保存文件夹.
    /// </summary>
    public ObservableCollection<FolderItemViewModel> CivitaiSaveFolders { get; } = new();
}
