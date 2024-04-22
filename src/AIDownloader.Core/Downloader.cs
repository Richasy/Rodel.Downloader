// Copyright (c) AI Downloader. All rights reserved.

using AIDownloader.Core.Civitai;
using AIDownloader.Core.Models;

namespace AIDownloader.Core;

/// <summary>
/// 下载客户端.
/// </summary>
public sealed class Downloader : IDisposable
{
    private HuggingFaceUtils _hfUtils;
    private CivitaiUtils _civitaiUtils;
    private string _hfSaveFolder;
    private string _civitaiSaveFolder;
    private bool _disposedValue;

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
    /// 初始化 Civitai.
    /// </summary>
    /// <param name="accessToken">访问令牌（可选）.</param>
    /// <param name="saveFolder">保存模型的文件夹.</param>
    public void InitializeCivitai(string accessToken, string saveFolder)
    {
        if (_civitaiUtils != null)
        {
            _civitaiUtils.SetAccessToken(accessToken);
        }
        else
        {
            _civitaiUtils = new CivitaiUtils(accessToken);
        }

        _civitaiSaveFolder = saveFolder;
    }

    /// <summary>
    /// 获取 Hugging Face 模型仓库下载列表.
    /// </summary>
    /// <param name="modelId">模型标识符，类似 <c>author/model</c>.</param>
    /// <returns>下载列表.</returns>
    public async Task<List<DownloadItem>> GetHuggingFaceModelAsync(string modelId)
    {
        if (_hfUtils == null)
        {
            throw new InvalidOperationException("Hugging Face 未初始化.");
        }

        var results = await _hfUtils.GetDownloadUrlsAsync(modelId);
        foreach (var item in results)
        {
            item.TargetFolder = _hfSaveFolder;
        }

        return results;
    }

    /// <summary>
    /// 获取 Civitai 模型下载列表.
    /// </summary>
    /// <param name="id">模型 Id.</param>
    /// <returns>模型不同版本的列表.</returns>
    /// <exception cref="InvalidOperationException">在没有初始化 Civitai 的情况下进行了调用.</exception>
    public async Task<List<ModelItem>> GetCivitaiModelAsync(string id)
    {
        return _civitaiUtils == null
            ? throw new InvalidOperationException("Civitai 未初始化.")
            : await _civitaiUtils.GetModelAsync(id);
    }

    /// <summary>
    /// 获取 Civitai 模型下载项.
    /// </summary>
    /// <param name="model">选中的模型.</param>
    /// <returns>下载项列表.</returns>
    public List<DownloadItem> GetCivitaiModelDownloadItems(ModelItem model)
    {
        var results = _civitaiUtils.GetDownloads(model);
        foreach (var item in results)
        {
            item.TargetFolder = _civitaiSaveFolder;
        }

        return results;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _hfUtils?.Dispose();
                _civitaiUtils?.Dispose();
            }

            _hfUtils = null;
            _civitaiUtils = null;
            _disposedValue = true;
        }
    }
}
