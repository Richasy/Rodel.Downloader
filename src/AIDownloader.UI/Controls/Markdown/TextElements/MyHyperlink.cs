﻿// Copyright (c) AI Downloader. All rights reserved.
// <auto-generated />

using HtmlAgilityPack;
using Markdig.Syntax.Inlines;
using Microsoft.UI.Xaml.Documents;

namespace AIDownloader.UI.Controls.Markdown.TextElements;

internal class MyHyperlink : IAddChild
{
    private Hyperlink _hyperlink;
    private readonly LinkInline _linkInline;
    private HtmlNode _htmlNode;
    private readonly string _baseUrl;

    public bool IsHtml => _htmlNode != null;

    public TextElement TextElement => _hyperlink;

    public MyHyperlink(LinkInline linkInline, string baseUrl)
    {
        _baseUrl = baseUrl;
        var url = linkInline.GetDynamicUrl != null ? linkInline.GetDynamicUrl() ?? linkInline.Url : linkInline.Url;
        _linkInline = linkInline;
        _hyperlink = new Hyperlink()
        {
            NavigateUri = Extensions.GetUri(url, baseUrl),
        };
    }

    public MyHyperlink(HtmlNode htmlNode, string baseUrl)
    {
        _baseUrl = baseUrl;
        var url = htmlNode.GetAttributeValue("href", "#");
        _htmlNode = htmlNode;
        _hyperlink = new Hyperlink()
        {
            NavigateUri = Extensions.GetUri(url, baseUrl),
        };
    }

    public void AddChild(IAddChild child)
    {
        if (child.TextElement is Microsoft.UI.Xaml.Documents.Inline inlineChild)
        {
            try
            {
                _hyperlink.Inlines.Add(inlineChild);
                // TODO: Add support for click handler
            }
            catch { }
        }
    }
}
