﻿// Copyright (c) AI Downloader. All rights reserved.
// <auto-generated />

using HtmlAgilityPack;
using AIDownloader.UI.Controls.Markdown.Renderers;
using AIDownloader.UI.Controls.Markdown.TextElements;
using AIDownloader.UI.Controls.Markdown.TextElements.Html;

namespace AIDownloader.UI.Controls.Markdown;

internal class HtmlWriter
{
    public static void WriteHtml(WinUIRenderer renderer, HtmlNodeCollection nodes)
    {
        if (nodes == null || nodes.Count == 0)
        {
            return;
        }

        foreach (var node in nodes)
        {
            if (node.NodeType == HtmlNodeType.Text)
            {
                renderer.WriteText(node.InnerText);
            }
            else if (node.NodeType == HtmlNodeType.Element && node.Name.TagToType() == TextElements.HtmlElementType.Inline)
            {
                // detect br here
                var inlineTagName = node.Name.ToLower();
                if (inlineTagName == "br")
                {
                    renderer.WriteInline(new MyLineBreak());
                }
                else if (inlineTagName == "a")
                {
                    IAddChild hyperLink = node.ChildNodes.Any(n => n.Name != "#text")
                        ? new MyHyperlinkButton(node, renderer.Config.BaseUrl)
                        : (IAddChild)new MyHyperlink(node, renderer.Config.BaseUrl);
                    renderer.Push(hyperLink);
                    WriteHtml(renderer, node.ChildNodes);
                    renderer.Pop();
                }
                else if (inlineTagName == "img")
                {
                    var image = new MyImage(node, renderer.Config);
                    renderer.WriteInline(image);
                }
                else
                {
                    var inline = new MyInline(node);
                    renderer.Push(inline);
                    WriteHtml(renderer, node.ChildNodes);
                    renderer.Pop();
                }
            }
            else if (node.NodeType == HtmlNodeType.Element && node.Name.TagToType() == TextElements.HtmlElementType.Block)
            {
                IAddChild block;
                var tag = node.Name.ToLower();
                if (tag == "details")
                {
                    block = new MyDetails(node);
                    node.ChildNodes.Remove(node.ChildNodes.FirstOrDefault(x => x.Name is "summary" or "header"));
                    renderer.Push(block);
                    WriteHtml(renderer, node.ChildNodes);
                }
                else if (tag.IsHeading())
                {
                    var heading = new MyHeading(node, renderer.Config);
                    renderer.Push(heading);
                    WriteHtml(renderer, node.ChildNodes);
                }
                else
                {
                    block = new MyBlock(node);
                    renderer.Push(block);
                    WriteHtml(renderer, node.ChildNodes);
                }

                renderer.Pop();
            }
        }
    }
}
