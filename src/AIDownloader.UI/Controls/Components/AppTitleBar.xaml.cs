// Copyright (c) AI Downloader. All rights reserved.

using System.ComponentModel;
using AIDownloader.UI.Pages;
using AIDownloader.UI.Toolkits;
using AIDownloader.UI.ViewModels;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;

namespace AIDownloader.UI.Controls.Components;

/// <summary>
/// App标题栏.
/// </summary>
public sealed partial class AppTitleBar : AppTitleBarBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppTitleBar"/> class.
    /// </summary>
    public AppTitleBar()
    {
        InitializeComponent();
        ViewModel = AppViewModel.Instance;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
        => ViewModel.PropertyChanged -= OnViewModelPropertyChangedAsync;

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ViewModel.PropertyChanged += OnViewModelPropertyChangedAsync;
        ResetDragArea();
    }

    private async void OnViewModelPropertyChangedAsync(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AppViewModel.IsBackButtonShown))
        {
            await Task.Delay(300);
            ResetDragArea();
        }
    }

    private void ResetDragArea()
    {
        var currentWindow = ViewModel.ActivatedWindow;
        if (currentWindow.AppWindow == null || !AppWindowTitleBar.IsCustomizationSupported())
        {
            return;
        }

        var scaleFactor = currentWindow.GetDpiForWindow() / 96d;
        var transform = BackButton.TransformToVisual(default);
        var backButtonBounds = transform.TransformBounds(new Rect(0, 0, BackButton.ActualWidth, BackButton.ActualHeight));
        var backButtonRect = AppToolkit.GetRectInt32(backButtonBounds, scaleFactor);

        var nonClientInputSrc = InputNonClientPointerSource.GetForWindowId(Win32Interop.GetWindowIdFromWindow(currentWindow.GetWindowHandle()));
        nonClientInputSrc.SetRegionRects(NonClientRegionKind.Passthrough, [backButtonRect]);
    }

    private void OnBackButtonClick(object sender, RoutedEventArgs e)
        => ViewModel.Navigate(typeof(DownloadPage));
}

/// <summary>
/// App标题栏基类.
/// </summary>
public abstract class AppTitleBarBase : ReactiveUserControl<AppViewModel>
{
}
