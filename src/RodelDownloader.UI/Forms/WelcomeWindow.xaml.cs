// Copyright (c) AI Downloader. All rights reserved.

using RodelDownloader.UI.Pages;
using RodelDownloader.UI.ViewModels;

namespace RodelDownloader.UI.Forms;

/// <summary>
/// 初始化欢迎窗口.
/// </summary>
public sealed partial class WelcomeWindow : WindowBase, ITipWindow
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WelcomeWindow"/> class.
    /// </summary>
    public WelcomeWindow()
    {
        InitializeComponent();

        AppWindow.SetPresenter(Microsoft.UI.Windowing.AppWindowPresenterKind.CompactOverlay);

        Width = 700;
        Height = 460;

        this.CenterOnScreen();
        _ = MainFrame.Navigate(typeof(WelcomePage));
    }

    /// <summary>
    /// 视图模型.
    /// </summary>
#pragma warning disable CA1822 // 将成员标记为 static
    public WelcomePageViewModel ViewModel => WelcomePageViewModel.Instance;
#pragma warning restore CA1822 // 将成员标记为 static

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

    internal static Visibility IsNavButtonShown(bool isLastStep)
        => isLastStep ? Visibility.Collapsed : Visibility.Visible;
}
