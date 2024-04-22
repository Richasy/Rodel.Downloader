﻿// Copyright (c) AI Downloader. All rights reserved.
// <auto-generated />

using Markdig.Syntax.Inlines;

namespace AI.Downloader.UI.Controls.Markdown.Renderers.ObjectRenderers.Inlines;

internal class LiteralInlineRenderer : WinUIObjectRenderer<LiteralInline>
{
    protected override void Write(WinUIRenderer renderer, LiteralInline obj)
    {
        if (renderer == null)
        {
            throw new ArgumentNullException(nameof(renderer));
        }

        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        if (obj.Content.IsEmpty)
        {
            return;
        }

        renderer.WriteText(ref obj.Content);
    }
}
