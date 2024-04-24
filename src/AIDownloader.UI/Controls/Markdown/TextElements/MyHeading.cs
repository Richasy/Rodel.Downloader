﻿// Copyright (c) AI Downloader. All rights reserved.
// <auto-generated />

using HtmlAgilityPack;
using Markdig.Syntax;
using Microsoft.UI.Xaml.Documents;

namespace AIDownloader.UI.Controls.Markdown.TextElements;

internal class MyHeading : IAddChild
{
    private Paragraph _paragraph;
    private readonly HeadingBlock _headingBlock;
    private HtmlNode _htmlNode;
    private readonly MarkdownConfig _config;

    public bool IsHtml => _htmlNode != null;

    public TextElement TextElement => _paragraph;

    public MyHeading(HeadingBlock headingBlock, MarkdownConfig config)
    {
        _headingBlock = headingBlock;
        _paragraph = new Paragraph();
        _config = config;

        var level = headingBlock.Level;
        _paragraph.FontSize = level switch
        {
            1 => _config.Themes.H1FontSize,
            2 => _config.Themes.H2FontSize,
            3 => _config.Themes.H3FontSize,
            4 => _config.Themes.H4FontSize,
            5 => _config.Themes.H5FontSize,
            _ => _config.Themes.H6FontSize,
        };

        //_paragraph.Foreground = _config.Themes.HeadingForeground;
        _paragraph.FontWeight = level switch
        {
            1 => _config.Themes.H1FontWeight,
            2 => _config.Themes.H2FontWeight,
            3 => _config.Themes.H3FontWeight,
            4 => _config.Themes.H4FontWeight,
            5 => _config.Themes.H5FontWeight,
            _ => _config.Themes.H6FontWeight,
        };
        var padding = level switch
        {
            1 => 0.5 * _config.Themes.H1FontSize,
            2 => 0.5 * _config.Themes.H2FontSize,
            3 => 0.5 * _config.Themes.H3FontSize,
            4 => 0.5 * _config.Themes.H4FontSize,
            5 => 0.5 * _config.Themes.H5FontSize,
            _ => 0.5 * _config.Themes.H6FontSize,
        };

        _paragraph.Margin = new Thickness(0, padding, 0, padding);
    }

    public MyHeading(HtmlNode htmlNode, MarkdownConfig config)
    {
        _htmlNode = htmlNode;
        _paragraph = new Paragraph();
        _config = config;

        var align = _htmlNode.GetAttributeValue("align", "left");
        _paragraph.TextAlignment = align switch
        {
            "left" => TextAlignment.Left,
            "right" => TextAlignment.Right,
            "center" => TextAlignment.Center,
            "justify" => TextAlignment.Justify,
            _ => TextAlignment.Left,
        };

        var level = int.Parse(htmlNode.Name.Substring(1));
        _paragraph.FontSize = level switch
        {
            1 => _config.Themes.H1FontSize,
            2 => _config.Themes.H2FontSize,
            3 => _config.Themes.H3FontSize,
            4 => _config.Themes.H4FontSize,
            5 => _config.Themes.H5FontSize,
            _ => _config.Themes.H6FontSize,
        };

        // _paragraph.Foreground = _config.Themes.HeadingForeground;
        _paragraph.FontWeight = level switch
        {
            1 => _config.Themes.H1FontWeight,
            2 => _config.Themes.H2FontWeight,
            3 => _config.Themes.H3FontWeight,
            4 => _config.Themes.H4FontWeight,
            5 => _config.Themes.H5FontWeight,
            _ => _config.Themes.H6FontWeight,
        };
        var padding = level switch
        {
            1 => 0.5 * _config.Themes.H1FontSize,
            2 => 0.5 * _config.Themes.H2FontSize,
            3 => 0.5 * _config.Themes.H3FontSize,
            4 => 0.5 * _config.Themes.H4FontSize,
            5 => 0.5 * _config.Themes.H5FontSize,
            _ => 0.5 * _config.Themes.H6FontSize,
        };

        _paragraph.Margin = new Thickness(0, padding, 0, padding);
    }

    public void AddChild(IAddChild child)
    {
        if (child.TextElement is Inline inlineChild)
        {
            _paragraph.Inlines.Add(inlineChild);
        }
    }
}
