// Copyright (c) AI Downloader. All rights reserved.

using System.Diagnostics;
using System.Net.NetworkInformation;
using RodelDownloader.UI.Models.Constants;
using Windows.ApplicationModel;
using Windows.Graphics;

namespace RodelDownloader.UI.Toolkits;

/// <summary>
/// 应用工具箱.
/// </summary>
public static class AppToolkit
{
    /// <summary>
    /// 获取应用包版本.
    /// </summary>
    /// <returns>包版本.</returns>
    public static string GetPackageVersion()
    {
        var appVersion = Package.Current.Id.Version;
        return $"{appVersion.Major}.{appVersion.Minor}.{appVersion.Build}.{appVersion.Revision}";
    }

    /// <summary>
    /// 重置控件主题.
    /// </summary>
    /// <param name="element">控件.</param>
    public static void ResetControlTheme(FrameworkElement element)
    {
        var localTheme = SettingsToolkit.ReadLocalSetting(SettingNames.AppTheme, ElementTheme.Default);
        element.RequestedTheme = localTheme;
    }

    /// <summary>
    /// 将 <see cref="Rect"/> 转换为 <see cref="RectInt32"/>.
    /// </summary>
    /// <param name="rect">矩形对象.</param>
    /// <param name="scaleFactor">缩放比例.</param>
    /// <returns><see cref="RectInt32"/>.</returns>
    public static RectInt32 GetRectInt32(Rect rect, double scaleFactor)
        => new(
              Convert.ToInt32(rect.X * scaleFactor),
              Convert.ToInt32(rect.Y * scaleFactor),
              Convert.ToInt32(rect.Width * scaleFactor),
              Convert.ToInt32(rect.Height * scaleFactor));

    /// <summary>
    /// 杀掉占用当前端口的进程.
    /// </summary>
    /// <param name="port">端口号.</param>
    /// <returns><see cref="Task"/>.</returns>
    public static async Task KillProcessIfUsingPortAsync(int port)
    {
        var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
        var listeners = ipGlobalProperties.GetActiveTcpListeners();
        var hasProcess = listeners.Any(p => p.Port == port);
        if (hasProcess)
        {
            await Task.Run(() =>
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        Arguments = $"-Command \"Get-NetTCPConnection -LocalPort {port} | Select-Object -ExpandProperty OwningProcess | ForEach-Object {{ Stop-Process -Id $_ -Force }}\"",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    },
                };

                _ = process.Start();
                process.WaitForExit();
            });
        }
    }
}
