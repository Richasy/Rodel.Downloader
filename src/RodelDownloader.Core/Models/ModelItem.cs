// Copyright (c) AI Downloader. All rights reserved.

namespace RodelDownloader.Core.Models;

/// <summary>
/// 模型条目.
/// </summary>
public sealed class ModelItem
{
    /// <summary>
    /// 标识符.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 名称.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 描述.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 附加对象.
    /// </summary>
    public object AttachObject { get; set; }
}
