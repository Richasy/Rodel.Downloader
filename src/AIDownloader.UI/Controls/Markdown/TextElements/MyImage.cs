﻿// Copyright (c) AI Downloader. All rights reserved.
// <auto-generated />

using System;
using System.Globalization;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using HtmlAgilityPack;
using Markdig.Syntax.Inlines;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;

namespace AI.Downloader.UI.Controls.Markdown.TextElements;

internal class MyImage : IAddChild
{
    private InlineUIContainer _container = new InlineUIContainer();
    private readonly LinkInline _linkInline;
    private Image _image = new();
    private readonly Uri _uri;
    private HtmlNode _htmlNode;
    private readonly IImageProvider _imageProvider;
    private readonly ISVGRenderer _svgRenderer;
    private readonly double _precedentWidth;
    private readonly double _precedentHeight;
    private bool _loaded;

    public TextElement TextElement => _container;

    public MyImage(LinkInline linkInline, Uri uri, MarkdownConfig config)
    {
        _linkInline = linkInline;
        _uri = uri;
        _imageProvider = config.ImageProvider;
        _svgRenderer = config.SVGRenderer ?? new DefaultSVGRenderer();
        Init();
        var size = Extensions.GetMarkdownImageSize(linkInline);
        if (size.Width != 0)
        {
            _precedentWidth = size.Width;
        }

        if (size.Height != 0)
        {
            _precedentHeight = size.Height;
        }
    }

    public MyImage(HtmlNode htmlNode, MarkdownConfig config)
    {
        Uri.TryCreate(htmlNode.GetAttributeValue("src", "#"), UriKind.RelativeOrAbsolute, out _uri);
        _htmlNode = htmlNode;
        _imageProvider = config?.ImageProvider;
        _svgRenderer = config?.SVGRenderer == null ? new DefaultSVGRenderer() : config.SVGRenderer;
        Init();
        int.TryParse(
            htmlNode.GetAttributeValue("width", "0"),
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out var width);
        int.TryParse(
            htmlNode.GetAttributeValue("height", "0"),
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out var height);
        if (width > 0)
        {
            _precedentWidth = width;
        }

        if (height > 0)
        {
            _precedentHeight = height;
        }
    }

    private void Init()
    {
        _image.Loaded += LoadImage;
        _container.Child = _image;
    }

    private async void LoadImage(object sender, RoutedEventArgs e)
    {
        if (_loaded)
        {
            return;
        }

        try
        {
            if (_imageProvider != null && _imageProvider.ShouldUseThisProvider(_uri.AbsoluteUri))
            {
                _image = await _imageProvider.GetImage(_uri.AbsoluteUri);
                _container.Child = _image;
            }
            else
            {
                var client = new HttpClient();

                // Download data from URL
                HttpResponseMessage response = await client.GetAsync(_uri);

                // Get the Content-Type header
                string contentType = response.Content.Headers.ContentType.MediaType;

                if (contentType == "image/svg+xml")
                {
                    var svgString = await response.Content.ReadAsStringAsync();
                    var resImage = await _svgRenderer.SvgToImage(svgString);
                    if (resImage != null)
                    {
                        _image = resImage;
                        _container.Child = _image;
                    }
                }
                else
                {
                    byte[] data = await response.Content.ReadAsByteArrayAsync();

                    // Create a BitmapImage for other supported formats
                    var bitmap = new BitmapImage();
                    using (var stream = new InMemoryRandomAccessStream())
                    {
                        // Write the data to the stream
                        await stream.WriteAsync(data.AsBuffer());
                        stream.Seek(0);

                        // Set the source of the BitmapImage
                        await bitmap.SetSourceAsync(stream);
                    }

                    _image.Source = bitmap;
                    _image.Width = bitmap.PixelWidth == 0 ? bitmap.DecodePixelWidth : bitmap.PixelWidth;
                    _image.Height = bitmap.PixelHeight == 0 ? bitmap.DecodePixelHeight : bitmap.PixelHeight;
                }

                _loaded = true;
            }

            if (_precedentWidth != 0)
            {
                _image.Width = _precedentWidth;
            }

            if (_precedentHeight != 0)
            {
                _image.Height = _precedentHeight;
            }
        }
        catch (Exception)
        {
        }
    }

    public void AddChild(IAddChild child)
    {
    }
}
