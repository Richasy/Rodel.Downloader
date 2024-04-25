// Copyright (c) AI Downloader. All rights reserved.

using System.Windows.Input;
using RodelDownloader.UI.ViewModels;

namespace RodelDownloader.UI.Controls.Components;

/// <summary>
/// 文件夹列表面板.
/// </summary>
public sealed partial class FolderListPanel : UserControl
{
    /// <summary>
    /// <see cref="ItemsSource"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource), typeof(ObservableCollection<FolderItemViewModel>), typeof(FolderListPanel), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="AddCommand"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty AddCommandProperty =
        DependencyProperty.Register(nameof(AddCommand), typeof(ICommand), typeof(FolderListPanel), new PropertyMetadata(default));

    /// <summary>
    /// <see cref="IsEmptyShown"/> 依赖属性.
    /// </summary>
    public static readonly DependencyProperty IsEmptyShownProperty =
        DependencyProperty.Register(nameof(IsEmptyShown), typeof(bool), typeof(FolderListPanel), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="FolderListPanel"/> class.
    /// </summary>
    public FolderListPanel() => InitializeComponent();

    /// <summary>
    /// 文件夹列表.
    /// </summary>
    public ObservableCollection<FolderItemViewModel> ItemsSource
    {
        get => (ObservableCollection<FolderItemViewModel>)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// 添加命令.
    /// </summary>
    public ICommand AddCommand
    {
        get => (ICommand)GetValue(AddCommandProperty);
        set => SetValue(AddCommandProperty, value);
    }

    /// <summary>
    /// 是否显示占位组件.
    /// </summary>
    public bool IsEmptyShown
    {
        get => (bool)GetValue(IsEmptyShownProperty);
        set => SetValue(IsEmptyShownProperty, value);
    }
}
