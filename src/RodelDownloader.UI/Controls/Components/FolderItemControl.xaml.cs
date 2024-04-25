// Copyright (c) AI Downloader. All rights reserved.

using RodelDownloader.UI.ViewModels;

namespace RodelDownloader.UI.Controls.Components;

/// <summary>
/// 文件夹条目.
/// </summary>
public sealed partial class FolderItemControl : FolderItemControlBase
{
    private readonly long _nameBoxVisEventToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="FolderItemControl"/> class.
    /// </summary>
    public FolderItemControl()
    {
        InitializeComponent();
        Unloaded += OnUnloaded;
        _nameBoxVisEventToken = NameBox.RegisterPropertyChangedCallback(TextBox.VisibilityProperty, OnNameBoxVisibilityChangedAsync);
    }

    private async void OnNameBoxVisibilityChangedAsync(DependencyObject sender, DependencyProperty dp)
    {
        if (NameBox.Visibility == Visibility.Visible)
        {
            await Task.Delay(300);
            NameBox.Focus(FocusState.Programmatic);
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (_nameBoxVisEventToken > 0)
        {
            NameBox.UnregisterPropertyChangedCallback(TextBox.VisibilityProperty, _nameBoxVisEventToken);
        }
    }

    private void OnNameBoxLostFocus(object sender, RoutedEventArgs e)
    {
        var v = (sender as TextBox).Text;
        if (!string.IsNullOrEmpty(v))
        {
            ViewModel.Name = v;
            ViewModel.ExitEditCommand.Execute(default);
        }
    }
}

/// <summary>
/// <see cref="FolderItemControl"/> 的基类.
/// </summary>
public abstract class FolderItemControlBase : ReactiveUserControl<FolderItemViewModel>
{
}
