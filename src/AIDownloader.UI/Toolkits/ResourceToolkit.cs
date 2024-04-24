// Copyright (c) AI Downloader. All rights reserved.

using AIDownloader.UI.Models.Constants;
using Windows.ApplicationModel.Resources.Core;

namespace AIDownloader.UI.Toolkits;

/// <summary>
/// 资源管理工具.
/// </summary>
public static class ResourceToolkit
{
    /// <summary>
    /// Get localized text.
    /// </summary>
    /// <param name="stringName">Resource name corresponding to localized text.</param>
    /// <returns>Localized text.</returns>
    public static string GetLocalizedString(StringNames stringName)
        => ResourceManager.Current.MainResourceMap[$"Resources/{stringName}"].Candidates[0].ValueAsString;
}
