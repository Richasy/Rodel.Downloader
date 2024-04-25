// Copyright (c) AI Downloader. All rights reserved.

using System.Net.Http.Json;
using RodelDownloader.Core.Models;

namespace RodelDownloader.Core;

internal sealed class HuggingFaceUtils : IDisposable
{
    private HttpClient _httpClient;
    private string _baseUrl;
    private string _accessToken;
    private bool _disposedValue;
    private CancellationTokenSource _cancellationTokenSource;

    public HuggingFaceUtils(string baseUri, string accessToken)
    {
        _baseUrl = baseUri;
        _accessToken = accessToken;
        _httpClient = new HttpClient();
    }

    public void SetBaseUri(string baseUri)
        => _baseUrl = baseUri;

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
        var models = await response.Content.ReadFromJsonAsync<HuggingFaceModels>();
        if (cancelSource.IsCancellationRequested)
        {
            return default;
        }

        var fileNames = models.siblings.Select(item => item.rfilename).ToList();
        var results = new List<DownloadItem>();
        foreach (var item in fileNames)
        {
            results.Add(new DownloadItem(item, $"{_baseUrl}/{modelId}/resolve/main/{item}?download=true"));
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
        var url = $"{_baseUrl}/api/models/{modelId}";
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
