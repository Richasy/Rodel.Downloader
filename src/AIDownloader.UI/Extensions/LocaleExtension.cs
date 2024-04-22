// Copyright (c) AI Downloader. All rights reserved.

using AIDownloader.UI.Models.Constants;
using AIDownloader.UI.Toolkits;
using Microsoft.UI.Xaml.Markup;

namespace AIDownloader.UI.Extensions;

/// <summary>
/// Localized text extension.
/// </summary>
[MarkupExtensionReturnType(ReturnType = typeof(string))]
public sealed class LocaleExtension : MarkupExtension
{
    /// <summary>
    /// Language name.
    /// </summary>
    public StringNames Name { get; set; }

    /// <inheritdoc/>
    protected override object ProvideValue()
        => ResourceToolkit.GetLocalizedString(Name);
}
