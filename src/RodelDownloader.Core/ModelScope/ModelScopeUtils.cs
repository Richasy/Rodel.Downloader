// Copyright (c) AI Downloader. All rights reserved.

using System.Net.Http.Json;
using RodelDownloader.Core.Models;

namespace RodelDownloader.Core.ModelScope;

internal sealed class ModelScopeUtils : IDisposable
{
    private const string _baseUrl = "https://modelscope.cn/api/v1/models";
    private bool _disposedValue;
    private HttpClient _httpClient;
    private string _accessToken;
    private CancellationTokenSource _cancellationTokenSource;

    public ModelScopeUtils(string accessToken)
    {
        _accessToken = accessToken;
        _httpClient = new HttpClient();
    }

    public void SetAccessToken(string accessToken)
        => _accessToken = accessToken;

    public async Task<List<DownloadItem>> GetDownloadUrlsAsync(string modelId)
    {
        var request = GetRequest(modelId);

        if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
        {
            await _cancellationTokenSource.CancelAsync();
        }

        var cancelSource = new CancellationTokenSource();
        _cancellationTokenSource = cancelSource;
        var response = await _httpClient.SendAsync(request, cancelSource.Token);
        response.EnsureSuccessStatusCode();
        var models = await response.Content.ReadFromJsonAsync<ModelScopeResponse>();
        if (cancelSource.IsCancellationRequested)
        {
            return default;
        }

        var fileNames = models.Data.Files.Select(item => item.Path).ToList();
        var results = new List<DownloadItem>();
        foreach (var item in fileNames)
        {
            results.Add(new DownloadItem(item, $"{_baseUrl}/{modelId}/repo?FilePath={Uri.EscapeDataString(item)}"));
        }

        return results;
    }

    public void Cancel()
        => _cancellationTokenSource?.Cancel();

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private HttpRequestMessage GetRequest(string modelId)
    {
        var url = $"{_baseUrl}/{modelId}/repo/files";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        if (!string.IsNullOrEmpty(_accessToken))
        {
            request.Headers.Add("Authorization", "Bearer " + _accessToken);
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
