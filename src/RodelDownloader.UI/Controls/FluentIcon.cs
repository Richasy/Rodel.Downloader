﻿// Copyright (c) AI Downloader. All rights reserved.

using Microsoft.UI.Xaml.Media;
using RodelDownloader.UI.Models.Constants;

namespace RodelDownloader.UI.Controls;

/// <summary>
/// Fluent 图标.
/// </summary>
public sealed class FluentIcon : FontIcon
{
    /// <summary>
    /// Dependency property for <see cref="Symbol"/>.
    /// </summary>
    public static readonly DependencyProperty SymbolProperty =
        DependencyProperty.Register(nameof(Symbol), typeof(FluentSymbol), typeof(FluentIcon), new PropertyMetadata(default, OnSymbolChanged));

    /// <summary>
    /// Initializes a new instance of the <see cref="FluentIcon"/> class.
    /// </summary>
    public FluentIcon()
        => FontFamily = new FontFamily("ms-appx:///Assets/FluentIcon.ttf#FluentSystemIcons-Resizable");

    /// <summary>
    /// Icon.
    /// </summary>
    public FluentSymbol Symbol
    {
        get => (FluentSymbol)GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }

    private static void OnSymbolChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is FluentSymbol symbol)
        {
            var icon = d as FluentIcon;
            icon.Glyph = ((char)symbol).ToString();
        }
    }
}
