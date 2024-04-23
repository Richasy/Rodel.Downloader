// Copyright (c) AI Downloader. All rights reserved.

using AIDownloader.UI.Forms;
using AIDownloader.UI.Models.Args;
using AIDownloader.UI.Models.Constants;
using AIDownloader.UI.Toolkits;

namespace AIDownloader.UI.ViewModels;

/// <summary>
/// 应用视图模型.
/// </summary>
public sealed partial class AppViewModel : ViewModelBase
{
    /// <summary>
    /// 在应用退出前执行.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public Task BeforeExitAsync()
    {
        if (ActivatedWindow is MainWindow)
        {
            DownloadPageViewModel.Instance.CleanCommand.Execute(default);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// 显示提示.
    /// </summary>
    /// <param name="message">提示内容.</param>
    /// <param name="type">提示类型.</param>
    public void ShowTip(string message, InfoType type = InfoType.Information)
        => RequestShowTip?.Invoke(this, new AppTipNotification(message, type, ActivatedWindow));

    /// <summary>
    /// 显示提示.
    /// </summary>
    /// <param name="messageName">提示内容.</param>
    /// <param name="type">提示类型.</param>
    public void ShowTip(StringNames messageName, InfoType type = InfoType.Information)
        => RequestShowTip?.Invoke(this, new AppTipNotification(ResourceToolkit.GetLocalizedString(messageName), type, ActivatedWindow));

    /// <summary>
    /// 修改主题.
    /// </summary>
    /// <param name="theme">主题类型.</param>
    public void ChangeTheme(ElementTheme theme)
    {
        if (ActivatedWindow == null)
        {
            return;
        }

        (ActivatedWindow.Content as FrameworkElement).RequestedTheme = theme;
        if (theme == ElementTheme.Dark)
        {
            ActivatedWindow.AppWindow.TitleBar.ButtonForegroundColor = Colors.White;
        }
        else if (theme == ElementTheme.Light)
        {
            ActivatedWindow.AppWindow.TitleBar.ButtonForegroundColor = Colors.Black;
        }
        else
        {
            ActivatedWindow.AppWindow.TitleBar.ButtonForegroundColor = default;
        }
    }
}
