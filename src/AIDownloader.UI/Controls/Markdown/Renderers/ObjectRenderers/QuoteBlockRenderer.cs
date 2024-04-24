﻿// Copyright (c) AI Downloader. All rights reserved.
// <auto-generated />

using Markdig.Syntax;
using AIDownloader.UI.Controls.Markdown.TextElements;

namespace AIDownloader.UI.Controls.Markdown.Renderers.ObjectRenderers;

internal class QuoteBlockRenderer : WinUIObjectRenderer<QuoteBlock>
{
    protected override void Write(WinUIRenderer renderer, QuoteBlock obj)
    {
        if (renderer == null)
        {
            throw new ArgumentNullException(nameof(renderer));
        }

        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        var quote = new MyQuote(obj);

        renderer.Push(quote);
        renderer.WriteChildren(obj);
        renderer.Pop();
    }
}
