﻿// Copyright (c) AI Downloader. All rights reserved.
// <auto-generated />

using System.Text.Json.Serialization;

namespace AIDownloader.Aria;

public class RequestError
{
    [JsonPropertyName("code")]
    public long Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }
}