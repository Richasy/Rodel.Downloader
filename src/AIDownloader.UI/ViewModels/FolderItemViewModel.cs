// Copyright (c) AI Downloader. All rights reserved.

using AIDownloader.UI.Models;
using AIDownloader.UI.Models.Constants;
using AIDownloader.UI.Toolkits;
using Windows.System;

namespace AIDownloader.UI.ViewModels;

/// <summary>
/// 文件夹项视图模型.
/// </summary>
public sealed partial class FolderItemViewModel : ViewModelBase
{
    private readonly Action<FolderItemViewModel> _removeAction;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _path;

    [ObservableProperty]
    private bool _isEditing;

    /// <summary>
    /// Initializes a new instance of the <see cref="FolderItemViewModel"/> class.
    /// </summary>
    public FolderItemViewModel(string path, Action<FolderItemViewModel> removeAction)
    {
        Path = path;
        Name = System.IO.Path.GetFileName(path);
        _removeAction = removeAction;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FolderItemViewModel"/> class.
    /// </summary>
    public FolderItemViewModel(FolderItem item, Action<FolderItemViewModel> removeAction)
    {
        Path = item.Path;
        Name = item.Name;
        _removeAction = removeAction;
    }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is FolderItemViewModel model && Path == model.Path;

    /// <summary>
    /// 获取数据.
    /// </summary>
    /// <returns>数据.</returns>
    public FolderItem GetData()
        => new()
        {
            Name = Name,
            Path = Path,
        };

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Path);

    [RelayCommand]
    private void Edit() => IsEditing = true;

    [RelayCommand]
    private void ExitEdit() => IsEditing = false;

    [RelayCommand]
    private void Remove()
        => _removeAction?.Invoke(this);

    [RelayCommand]
    private async Task OpenFolderAsync()
    {
        if (IsEditing)
        {
            return;
        }

        try
        {
            await Launcher.LaunchFolderPathAsync(Path);
        }
        catch (Exception)
        {
            AppViewModel.Instance.ShowTip(ResourceToolkit.GetLocalizedString(StringNames.FolderOpenFailed), InfoType.Error);
        }
    }
}
