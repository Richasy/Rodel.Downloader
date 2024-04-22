﻿// Copyright (c) AI Downloader. All rights reserved.
// <auto-generated />

using HtmlAgilityPack;
using Markdig.Syntax;
using Microsoft.UI.Xaml.Documents;
using Block = Microsoft.UI.Xaml.Documents.Block;
using Inline = Microsoft.UI.Xaml.Documents.Inline;

namespace AI.Downloader.UI.Controls.Markdown.TextElements;

public class MyFlowDocument : IAddChild
{
    private HtmlNode _htmlNode;
    private readonly MarkdownObject _markdownObject;

    // useless property

    /// <inheritdoc/>
    public TextElement TextElement { get; set; } = new Run();

    public RichTextBlock RichTextBlock { get; set; } = new RichTextBlock()
    {
        TextWrapping = TextWrapping.Wrap,
        IsTextSelectionEnabled = true,
    };

    public bool IsHtml => _htmlNode != null;

    public MyFlowDocument()
    {
    }

    public MyFlowDocument(MarkdownObject markdownObject) => _markdownObject = markdownObject;

    public MyFlowDocument(HtmlNode node) => _htmlNode = node;

    /// <inheritdoc/>
    public void AddChild(IAddChild child)
    {
        var element = child.TextElement;
        if (element != null)
        {
            if (element is Block block)
            {
                RichTextBlock.Blocks.Add(block);
            }
            else if (element is Inline inline)
            {
                var paragraph = new Paragraph();
                paragraph.Inlines.Add(inline);
                RichTextBlock.Blocks.Add(paragraph);
            }
        }
    }
}
