// Copyright (c) AI Downloader. All rights reserved.

using AIDownloader.UI.Pages;

namespace AIDownloader.UI.Forms;

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
