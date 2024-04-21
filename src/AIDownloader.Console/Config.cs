// Copyright (c) AI Downloader. All rights reserved.

using System.Text.Json.Serialization;

internal class Config
{
    [JsonPropertyName("hf_token")]
    public string HuggingFaceToken { get; set; }

    [JsonPropertyName("hf_save_folder")]
    public string HuggingFaceSaveFolder { get; set; }

    [JsonPropertyName("hf_backup_folders")]
    public Dictionary<string, string> HuggingFaceBackupFolders { get; set; }

    [JsonPropertyName("hf_uri_type")]
    public string HuggingFaceUriType { get; set; }

    [JsonPropertyName("civitai_token")]
    public string CivitaiToken { get; set; }

    [JsonPropertyName("civitai_save_folder")]
    public string CivitaiSaveFolder { get; set; }

    [JsonPropertyName("civitai_backup_folders")]
    public Dictionary<string, string> CivitaiBackupFolders { get; set; }
}
