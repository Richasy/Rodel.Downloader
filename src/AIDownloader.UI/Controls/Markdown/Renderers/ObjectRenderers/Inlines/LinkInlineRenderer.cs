﻿// Copyright (c) AI Downloader. All rights reserved.
// <auto-generated />

using Markdig.Syntax.Inlines;
using AI.Downloader.UI.Controls.Markdown.TextElements;

namespace AI.Downloader.UI.Controls.Markdown.Renderers.ObjectRenderers.Inlines;

internal class LinkInlineRenderer : WinUIObjectRenderer<LinkInline>
{
    protected override void Write(WinUIRenderer renderer, LinkInline link)
    {
        if (renderer == null)
        {
            throw new ArgumentNullException(nameof(renderer));
        }

        if (link == null)
        {
            throw new ArgumentNullException(nameof(link));
        }

        var url = link.GetDynamicUrl != null ? link.GetDynamicUrl() ?? link.Url : link.Url;

        if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
        {
            url = "#";
        }

        if (link.IsImage)
        {
            var image = new MyImage(link, AI.Downloader.UI.Controls.Markdown.Extensions.GetUri(url, renderer.Config.BaseUrl), renderer.Config);
            renderer.WriteInline(image);
        }
        else
        {
            if (link.FirstChild is LinkInline linkInlineChild && linkInlineChild.IsImage)
            {
                renderer.Push(new MyHyperlinkButton(link, renderer.Config.BaseUrl));
            }
            else
            {
                var hyperlink = new MyHyperlink(link, renderer.Config.BaseUrl);

                renderer.Push(hyperlink);
            }

            renderer.WriteChildren(link);
            renderer.Pop();
        }
    }
}
