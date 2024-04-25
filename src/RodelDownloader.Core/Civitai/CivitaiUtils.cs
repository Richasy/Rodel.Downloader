// Copyright (c) AI Downloader. All rights reserved.

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using RodelDownloader.Core.Models;

namespace RodelDownloader.Core.Civitai;

internal sealed class CivitaiUtils : IDisposable
{
    private bool _disposedValue;
    private HttpClient _httpClient;
    private string _accessToken;
    private CancellationTokenSource _cancellationTokenSource;

    public CivitaiUtils(string accessToken)
    {
        _accessToken = accessToken;
        _httpClient = new HttpClient();
    }

    public void SetAccessToken(string accessToken)
        => _accessToken = accessToken;

    public async Task<List<ModelItem>> GetModelAsync(string id)
    {
        var request = GetRequest(id);
        if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
        {
            await _cancellationTokenSource.CancelAsync();
        }

        var cancelSource = new CancellationTokenSource();
        _cancellationTokenSource = cancelSource;
        var response = await _httpClient.SendAsync(request, cancelSource.Token);
        response.EnsureSuccessStatusCode();
        var model = await response.Content.ReadFromJsonAsync<CivitaiModel>();
        if (cancelSource.IsCancellationRequested)
        {
            return default;
        }

        var modelList = new List<ModelItem>();
        foreach (var version in model.modelVersions)
        {
            var item = new ModelItem();
            item.Id = version.id.ToString();
            item.Name = version.name;
            item.Description = TrimHtmlTag(version.description ?? version.baseModel ?? version.publishedAt.ToString("yyyy/MM/dd"));
            item.AttachObject = version;
            modelList.Add(item);
        }

        return modelList;
    }

    /// <summary>
    /// 从模型中获取下载项.
    /// </summary>
    /// <param name="model">选中的模型.</param>
    /// <returns>下载列表.</returns>
    public List<DownloadItem> GetDownloads(ModelItem model)
    {
        var version = model.AttachObject as CivitaiModelVersion;
        if (version == null)
        {
            return default;
        }

        var results = new List<DownloadItem>();
        foreach (var file in version.files)
        {
            var url = string.IsNullOrEmpty(_accessToken) ? file.downloadUrl : file.downloadUrl + $"?token={_accessToken}";
            results.Add(new DownloadItem(file.name, url));
        }

        return results;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private static string TrimHtmlTag(string html)
    {
        if (string.IsNullOrEmpty(html))
        {
            return string.Empty;
        }

        var htmlWithLineBreaks = Regex.Replace(html, @"</p>", "\n", RegexOptions.IgnoreCase);
        var textOnly = Regex.Replace(htmlWithLineBreaks, @"<[^>]*>", string.Empty);

        return textOnly;
    }

    private HttpRequestMessage GetRequest(string modelId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://civitai.com/api/v1/models/{modelId}");

        if (!string.IsNullOrEmpty(_accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        }

        return request;
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _httpClient?.Dispose();
                _cancellationTokenSource?.Dispose();
            }

            _httpClient = null;
            _cancellationTokenSource = null;
            _disposedValue = true;
        }
    }
}
