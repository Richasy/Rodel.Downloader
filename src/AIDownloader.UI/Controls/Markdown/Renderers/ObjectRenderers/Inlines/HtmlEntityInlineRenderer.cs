﻿// Copyright (c) AI Downloader. All rights reserved.
// <auto-generated />

using Markdig.Syntax.Inlines;

namespace AIDownloader.UI.Controls.Markdown.Renderers.ObjectRenderers.Inlines;

internal class HtmlEntityInlineRenderer : WinUIObjectRenderer<HtmlEntityInline>
{
    protected override void Write(WinUIRenderer renderer, HtmlEntityInline obj)
    {
        if (renderer == null)
        {
            throw new ArgumentNullException(nameof(renderer));
        }

        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        var transcoded = obj.Transcoded;
        renderer.WriteText(ref transcoded);

        // todo: wtf is this?
    }
}
