// Copyright (c) AI Downloader. All rights reserved.

using System.Text.Json.Serialization;

internal class Config
{
    [JsonPropertyName("token")]
    public string Token { get; set; }

    [JsonPropertyName("save_folder")]
    public string SaveFolder { get; set; }
}
