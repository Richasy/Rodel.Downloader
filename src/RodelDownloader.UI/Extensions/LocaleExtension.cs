// Copyright (c) AI Downloader. All rights reserved.

using Microsoft.UI.Xaml.Markup;
using RodelDownloader.UI.Models.Constants;
using RodelDownloader.UI.Toolkits;

namespace RodelDownloader.UI.Extensions;

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
