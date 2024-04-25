// Copyright (c) AI Downloader. All rights reserved.

using Microsoft.UI.Windowing;
using RodelDownloader.UI.Controls;
using RodelDownloader.UI.Models.Args;
using RodelDownloader.UI.Models.Constants;
using RodelDownloader.UI.Pages;
using RodelDownloader.UI.Toolkits;
using RodelDownloader.UI.ViewModels;
using Windows.ApplicationModel.Activation;

namespace RodelDownloader.UI.Forms;

/// <summary>
/// 主窗口.
/// </summary>
public sealed partial class MainWindow : WindowBase, ITipWindow
{
    private readonly IActivatedEventArgs _launchArgs;
    private bool _isActivated;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow(IActivatedEventArgs args = default)
    {
        InitializeComponent();
        _launchArgs = args;
        Closed += OnClosedAsync;
        Activated += OnWindowActivated;

        MinWidth = 600;
        MinHeight = 400;

        var lastWidth = SettingsToolkit.ReadLocalSetting(SettingNames.LastWindowWidth, 800d);
        var lastHeight = SettingsToolkit.ReadLocalSetting(SettingNames.LastWindowHeight, 500d);
        Width = lastWidth;
        Height = lastHeight;

        this.CenterOnScreen();

        MainFrame.Navigate(typeof(DownloadPage));
        AppViewModel.Instance.RequestShowTip += OnAppViewModelRequestShowTip;
        AppViewModel.Instance.RequestNavigate += OnAppViewModelRequestNavigate;
    }

    /// <inheritdoc/>
    public async Task ShowTipAsync(UIElement element, double delaySeconds)
    {
        TipContainer.Visibility = Visibility.Visible;
        TipContainer.Children.Add(element);
        element.Visibility = Visibility.Visible;
        await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
        element.Visibility = Visibility.Collapsed;
        _ = TipContainer.Children.Remove(element);
        if (TipContainer.Children.Count == 0)
        {
            TipContainer.Visibility = Visibility.Collapsed;
        }
    }

    /// <summary>
    /// 激活参数.
    /// </summary>
    /// <param name="e">参数.</param>
    public void ActivateArguments(IActivatedEventArgs e = default)
    {
        e ??= _launchArgs;

        if (e.Kind == ActivationKind.StartupTask)
        {
            _ = this.Hide();
        }
    }

    private void OnAppViewModelRequestShowTip(object sender, AppTipNotification e)
    {
        if (e.TargetWindow is ITipWindow window)
        {
            new TipPopup(e.Message, window).ShowAsync(e.Type);
        }
    }

    private void OnAppViewModelRequestNavigate(object sender, AppNavigateEventArgs e)
    {
        if (MainFrame.Content is not null && MainFrame.Content.GetType() == e.PageType)
        {
            return;
        }

        MainFrame.Navigate(e.PageType, e.Parameter);
    }

    private async void OnClosedAsync(object sender, WindowEventArgs args)
    {
        await AppViewModel.Instance.BeforeExitAsync();
        var isMaximized = PInvoke.IsZoomed(new HWND(this.GetWindowHandle()));
        SettingsToolkit.WriteLocalSetting(SettingNames.IsWindowMaximized, (bool)isMaximized);

        if (!isMaximized)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.LastWindowWidth, Width);
            SettingsToolkit.WriteLocalSetting(SettingNames.LastWindowHeight, Height);
        }
    }

    private void OnWindowActivated(object sender, WindowActivatedEventArgs args)
    {
        if (_isActivated)
        {
            return;
        }

        var isMaximized = SettingsToolkit.ReadLocalSetting(SettingNames.IsWindowMaximized, false);
        if (isMaximized)
        {
            (AppWindow.Presenter as OverlappedPresenter).Maximize();
        }

        var localTheme = SettingsToolkit.ReadLocalSetting(SettingNames.AppTheme, ElementTheme.Default);
        AppViewModel.Instance.ChangeTheme(localTheme);
        _isActivated = true;
    }

    private void OnFrameLoaded(object sender, RoutedEventArgs e)
    {
        if (_launchArgs != null)
        {
            ActivateArguments(_launchArgs);
        }
    }
}
