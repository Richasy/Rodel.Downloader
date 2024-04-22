﻿// Copyright (c) AI Downloader. All rights reserved.
// <auto-generated />

namespace AIDownloader.Core.Models;

public class CivitaiModel
{
    public int id { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public bool poi { get; set; }
    public bool allowNoCredit { get; set; }
    public string[] allowCommercialUse { get; set; }
    public bool allowDerivatives { get; set; }
    public bool allowDifferentLicense { get; set; }
    public string type { get; set; }
    public bool nsfw { get; set; }
    public int nsfwLevel { get; set; }
    public CivitaiStats stats { get; set; }
    public CivitaiCreator creator { get; set; }
    public string[] tags { get; set; }
    public CivitaiModelVersion[] modelVersions { get; set; }
}

public class CivitaiStats
{
    public int downloadCount { get; set; }
    public int favoriteCount { get; set; }
    public int thumbsUpCount { get; set; }
    public int thumbsDownCount { get; set; }
    public int commentCount { get; set; }
    public int ratingCount { get; set; }
    public double rating { get; set; }
    public int tippedAmountCount { get; set; }
}

public class CivitaiCreator
{
    public string username { get; set; }
    public string image { get; set; }
}

public class CivitaiModelVersion
{
    public int id { get; set; }
    public string name { get; set; }
    public int index { get; set; }
    public string status { get; set; }
    public int modelId { get; set; }
    public string baseModel { get; set; }
    public DateTimeOffset createdAt { get; set; }
    public int nsfwLevel { get; set; }
    public DateTimeOffset publishedAt { get; set; }
    public string availability { get; set; }
    public string baseModelType { get; set; }
    public CivitaiStats stats { get; set; }
    public CivitaiFile[] files { get; set; }
    public CivitaiImage[] images { get; set; }
    public string downloadUrl { get; set; }
    public string description { get; set; }
}

public class CivitaiFile
{
    public int id { get; set; }
    public float sizeKB { get; set; }
    public string name { get; set; }
    public string type { get; set; }
    public string pickleScanResult { get; set; }
    public string pickleScanMessage { get; set; }
    public string virusScanResult { get; set; }
    public object virusScanMessage { get; set; }
    public DateTime scannedAt { get; set; }
    public CivitaiMetadata metadata { get; set; }
    public CivitaiHashes hashes { get; set; }
    public string downloadUrl { get; set; }
    public bool primary { get; set; }
}

public class CivitaiMetadata
{
    public string format { get; set; }
    public string size { get; set; }
    public string fp { get; set; }
}

public class CivitaiHashes
{
    public string AutoV1 { get; set; }
    public string AutoV2 { get; set; }
    public string SHA256 { get; set; }
    public string CRC32 { get; set; }
    public string BLAKE3 { get; set; }
    public string AutoV3 { get; set; }
}

public class CivitaiImage
{
    public string url { get; set; }
    public int nsfwLevel { get; set; }
    public int width { get; set; }
    public int height { get; set; }
    public string hash { get; set; }
    public string type { get; set; }
}
