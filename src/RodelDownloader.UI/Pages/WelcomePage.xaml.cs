// Copyright (c) AI Downloader. All rights reserved.

using RodelDownloader.UI.ViewModels;

namespace RodelDownloader.UI.Pages;

/// <summary>
/// 欢迎页.
/// </summary>
public sealed partial class WelcomePage : WelcomePageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WelcomePage"/> class.
    /// </summary>
    public WelcomePage()
    {
        InitializeComponent();
        ViewModel = WelcomePageViewModel.Instance;
    }
}

/// <summary>
/// <see cref="WelcomePage"/> 的基类.
/// </summary>
public abstract class WelcomePageBase : PageBase<WelcomePageViewModel>
{
}
