// Copyright (c) AI Downloader. All rights reserved.

using AIDownloader.Core.Models;

namespace AIDownloader.Core;

/// <summary>
/// 下载客户端.
/// </summary>
public sealed class Downloader
{
    private HuggingFaceUtils _hfUtils;
    private string _hfSaveFolder;

    /// <summary>
    /// 初始化 Hugging Face.
    /// </summary>
    /// <param name="uriType">使用的 BaseUri 类型.</param>
    /// <param name="accessToken">访问令牌.</param>
    /// <param name="saveFolder">保存文件夹的路径.</param>
    /// <exception cref="ArgumentOutOfRangeException">Uri 类型不在所属范围内.</exception>
    /// <exception cref="ArgumentNullException">访问令牌不能为空.</exception>
    public void InitializeHuggingFace(HuggingFaceUriType uriType, string accessToken, string saveFolder)
    {
        var baseUri = uriType switch
        {
            HuggingFaceUriType.Official => "https://huggingface.co",
            HuggingFaceUriType.Mirror => "https://hf-mirror.com",
            _ => throw new ArgumentOutOfRangeException(nameof(uriType)),
        };

        if (string.IsNullOrEmpty(accessToken))
        {
            throw new ArgumentNullException(nameof(accessToken));
        }

        if (_hfUtils != null)
        {
            _hfUtils.SetBaseUri(baseUri);
            _hfUtils.SetAccessToken(accessToken);
        }
        else
        {
            _hfUtils = new HuggingFaceUtils(baseUri, accessToken);
        }

        _hfSaveFolder = saveFolder;
    }

    /// <summary>
    /// 获取 Hugging Face 模型仓库下载列表.
    /// </summary>
    /// <param name="modelId">模型标识符，类似 <c>author/model</c>.</param>
    /// <returns>下载列表.</returns>
    public async Task<List<DownloadItem>> GetHuggingFaceModelAsync(string modelId)
    {
        var results = await _hfUtils.GetDownloadUrlsAsync(modelId);
        foreach (var item in results)
        {
            item.TargetFolder = _hfSaveFolder;
        }

        return results;
    }
}
