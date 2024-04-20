// Copyright (c) AI Downloader. All rights reserved.

using System.Net;
using System.Text;
using System.Text.Json;
using AIDownloader.Aria.Exceptions;

namespace AIDownloader.Aria.Apis;

internal class Requests : IDisposable
{
    private readonly Store _store;
    private HttpClient _httpClient;
    private bool _disposedValue;

    public Requests(HttpClient httpClient, Store store)
    {
        _httpClient = httpClient;
        _store = store;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async Task GetRequestAsync(string url, CancellationToken cancellationToken)
        => await RequestAsync(url, _store.Secret, cancellationToken);

    public async Task GetRequestAsync(string url, CancellationToken cancellationToken, params object[] parameters)
        => await RequestAsync(url, _store.Secret, cancellationToken, parameters);

    public async Task<T> GetRequestAsync<T>(string url, CancellationToken cancellationToken)
    {
        var aria2Result = await RequestAsync<RequestResult<T>>(url, cancellationToken);
        return aria2Result.Result == null ? throw new Exception("No response data received") : aria2Result.Result;
    }

    public async Task<T> GetRequestAsync<T>(string url, CancellationToken cancellationToken, params object?[] parameters)
    {
        var aria2Result = await RequestAsync<RequestResult<T>>(url, cancellationToken, parameters);
        return aria2Result.Result == null ? throw new Exception("No response data received") : aria2Result.Result;
    }

    public async Task<List<object>> MultiRequestAsync(CancellationToken cancellationToken, params object?[] parameters)
    {
        var parameterList = new List<MulticallRequest>();

        foreach (var param in parameters)
        {
            if (param is not object[] methodCall)
            {
                throw new Exception($"Parameter must be of type object[]");
            }

            var actualParameters = new List<object>
            {
                $"token:{_store.Secret}",
            };
            actualParameters.AddRange(methodCall.Skip(1));

            if (methodCall[0] is not string methodName)
            {
                throw new Exception("Invalid method name in the first object");
            }

            var multicallRequest = new MulticallRequest
            {
                MethodName = methodName,
                Parameters = actualParameters.ToList(),
            };

            parameterList.Add(multicallRequest);
        }

        var requestResult = await RequestAsync("system.multicall", null, cancellationToken, parameterList);

        try
        {
            var result = JsonSerializer.Deserialize<RequestResult<List<List<object>>>>(requestResult) ?? throw new Exception();

            return result.Result == null ? throw new Exception() : result.Result.Select(m => m.First()).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Unable to deserialize Aria2 API response. Response was: {requestResult}", ex);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _httpClient?.Dispose();
            }

            _httpClient = default;
            _disposedValue = true;
        }
    }

    private static Aria2Exception? ParseAria2Exception(string? text)
    {
        try
        {
            if (text == null)
            {
                return null;
            }

            var requestError = JsonSerializer.Deserialize<RequestResult<RequestError>>(text);

            if (requestError?.Error != null)
            {
                return new Aria2Exception(requestError.Error.Code, requestError.Error.Message);
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    private async Task<string> RequestAsync(string method, string? secret, CancellationToken cancellationToken, params object?[]? parameters)
    {
        var requestUrl = $"{_store.Aria2Url}";

        var request = new Request
        {
            Id = "aria2net",
            Jsonrpc = "2.0",
            Method = method,
            Parameters = new List<object?>(),
        };

        if (!string.IsNullOrWhiteSpace(secret))
        {
            request.Parameters.Add($"token:{secret}");
        }

        if (parameters != null && parameters.Length > 0)
        {
            foreach (var parameter in parameters.Where(m => m != null))
            {
                request.Parameters.Add(parameter);
            }
        }

        var jsonRequest = JsonSerializer.Serialize(request);

        var content = new StringContent(jsonRequest);

        var retryCount = 0;
        while (true)
        {
            try
            {
                var response = await _httpClient.PostAsync(requestUrl, content, cancellationToken);

                var buffer = await response.Content.ReadAsByteArrayAsync();
                var text = Encoding.UTF8.GetString(buffer);

                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    text = null;
                }

                if (!response.IsSuccessStatusCode)
                {
                    var aria2Exception = ParseAria2Exception(text);

                    if (aria2Exception != null)
                    {
                        throw aria2Exception;
                    }

                    throw new Exception(text);
                }

                if (string.IsNullOrEmpty(text))
                {
                    throw new Exception("No response received");
                }

                return text!;
            }
            catch (Aria2Exception)
            {
                throw;
            }
            catch
            {
                if (retryCount < _store.RetryCount)
                {
                    retryCount++;

                    await Task.Delay(1000 * retryCount, cancellationToken);
                }
                else
                {
                    throw;
                }
            }
        }
    }

    private async Task<T> RequestAsync<T>(string url, CancellationToken cancellationToken, params object?[] parameters)
        where T : class, new()
    {
        var result = await RequestAsync(url, _store.Secret, cancellationToken, parameters);

        try
        {
            return JsonSerializer.Deserialize<T>(result) ?? throw new Exception();
        }
        catch (Exception ex)
        {
            throw new Exception($"Unable to deserialize Aria2 API response to {typeof(T).Name}. Response was: {result}", ex);
        }
    }
}
