// Copyright (c) AI Downloader. All rights reserved.

using AIDownloader.UI.Models;

namespace AIDownloader.UI.ViewModels;

/// <summary>
/// 文件夹项视图模型.
/// </summary>
public sealed partial class FolderItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _path;

    /// <summary>
    /// Initializes a new instance of the <see cref="FolderItemViewModel"/> class.
    /// </summary>
    public FolderItemViewModel(string path)
    {
        Path = path;
        Name = System.IO.Path.GetFileName(path);
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
}
