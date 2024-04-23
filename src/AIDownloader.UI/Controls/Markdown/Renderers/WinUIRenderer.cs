﻿// Copyright (c) AI Downloader. All rights reserved.
// <auto-generated />

using Markdig.Helpers;
using Markdig.Renderers;
using Markdig.Syntax;
using AI.Downloader.UI.Controls.Markdown.Renderers.ObjectRenderers;
using AI.Downloader.UI.Controls.Markdown.Renderers.ObjectRenderers.Extensions;
using AI.Downloader.UI.Controls.Markdown.Renderers.ObjectRenderers.Inlines;
using AI.Downloader.UI.Controls.Markdown.TextElements;

namespace AI.Downloader.UI.Controls.Markdown.Renderers;

public class WinUIRenderer : RendererBase
{
    private readonly Stack<IAddChild> _stack = new();
    private char[] _buffer;

    public MyFlowDocument FlowDocument { get; private set; }
    public MarkdownConfig Config { get; set; } = MarkdownConfig.Default;

    public WinUIRenderer(MyFlowDocument document, MarkdownConfig config)
    {
        _buffer = new char[1024];
        Config = config;
        FlowDocument = document;

        // set style
        _stack.Push(FlowDocument);
        LoadOverridenRenderers();
    }

    private void LoadOverridenRenderers() => LoadRenderers();

    /// <inheritdoc/>
    public override object Render(MarkdownObject markdownObject)
    {
        Write(markdownObject);
        return FlowDocument ?? new();
    }

    public void ReloadDocument()
    {
        _stack.Clear();
        FlowDocument.RichTextBlock.Blocks.Clear();
        _stack.Push(FlowDocument);
        LoadOverridenRenderers();
    }

    public void WriteLeafInline(LeafBlock leafBlock)
    {
        if (leafBlock == null || leafBlock.Inline == null)
        {
            throw new ArgumentNullException(nameof(leafBlock));
        }

        var inline = (Markdig.Syntax.Inlines.Inline)leafBlock.Inline;
        while (inline != null)
        {
            Write(inline);
            inline = inline.NextSibling;
        }
    }

    public void WriteLeafRawLines(LeafBlock leafBlock)
    {
        if (leafBlock == null)
        {
            throw new ArgumentNullException(nameof(leafBlock));
        }

        if (leafBlock.Lines.Lines != null)
        {
            var lines = leafBlock.Lines;
            var slices = lines.Lines;
            for (var i = 0; i < lines.Count; i++)
            {
                if (i != 0)
                {
                    WriteInline(new MyLineBreak());
                }

                WriteText(ref slices[i].Slice);
            }
        }
    }

    public void Push(IAddChild child) => _stack.Push(child);

    public void Pop()
    {
        var popped = _stack.Pop();
        _stack.Peek().AddChild(popped);
    }

    public void WriteBlock(IAddChild obj) => _stack.Peek().AddChild(obj);

    public void WriteInline(IAddChild inline) => AddInline(_stack.Peek(), inline);

    public void WriteText(ref StringSlice slice)
    {
        if (slice.Start > slice.End)
        {
            return;
        }

        WriteText(slice.Text, slice.Start, slice.Length);
    }

    public void WriteText(string text) => WriteInline(new MyInlineText(text ?? string.Empty));

    public void WriteText(string text, int offset, int length)
    {
        if (text == null)
        {
            return;
        }

        if (offset == 0 && text.Length == length)
        {
            WriteText(text);
        }
        else
        {
            if (length > _buffer.Length)
            {
                _buffer = text.ToCharArray();
                WriteText(new string(_buffer, offset, length));
            }
            else
            {
                text.CopyTo(offset, _buffer, 0, length);
                WriteText(new string(_buffer, 0, length));
            }
        }
    }

    private static void AddInline(IAddChild parent, IAddChild inline) => parent.AddChild(inline);

    protected virtual void LoadRenderers()
    {
        // Default block renderers
        ObjectRenderers.Add(new CodeBlockRenderer());
        ObjectRenderers.Add(new ListRenderer());
        ObjectRenderers.Add(new HeadingRenderer());
        ObjectRenderers.Add(new ParagraphRenderer());
        ObjectRenderers.Add(new QuoteBlockRenderer());
        ObjectRenderers.Add(new ThematicBreakRenderer());
        ObjectRenderers.Add(new HtmlBlockRenderer());

        // Default inline renderers
        ObjectRenderers.Add(new AutoLinkInlineRenderer());
        ObjectRenderers.Add(new CodeInlineRenderer());
        ObjectRenderers.Add(new DelimiterInlineRenderer());
        ObjectRenderers.Add(new EmphasisInlineRenderer());
        ObjectRenderers.Add(new HtmlEntityInlineRenderer());
        ObjectRenderers.Add(new LineBreakInlineRenderer());
        ObjectRenderers.Add(new LinkInlineRenderer());
        ObjectRenderers.Add(new LiteralInlineRenderer());
        ObjectRenderers.Add(new ContainerInlineRenderer());

        // Extension renderers
        ObjectRenderers.Add(new TableRenderer());
        ObjectRenderers.Add(new TaskListRenderer());
        ObjectRenderers.Add(new HtmlInlineRenderer());
    }
}