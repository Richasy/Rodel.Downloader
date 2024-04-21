﻿// Copyright (c) AI Downloader. All rights reserved.

using System.Text;
using System.Text.Json;
using AIDownloader.Core;
using AIDownloader.Core.Models;
using Spectre.Console;

/// <summary>
/// 入口.
/// </summary>
public partial class Program
{
    private static async Task RunCivitaiDownloadAsync(bool ignoreConfig = false)
    {
        var configFile = Path.Combine(Environment.CurrentDirectory, "config.json");
        var isConfigFileExist = File.Exists(configFile);

        var modelId = AnsiConsole.Ask<string>(GetString("ModelIdOrLinkInput"));
        if (string.IsNullOrEmpty(modelId))
        {
            AnsiConsole.MarkupLine($"[red]{GetString("ModelIdInputFailed")}[/]");
            return;
        }

        if (modelId.Contains('/'))
        {
            var split = modelId.Split('/');
            foreach (var s in split)
            {
                if (long.TryParse(s, out var id))
                {
                    modelId = id.ToString();
                    break;
                }
            }
        }

        if (!long.TryParse(modelId, out _))
        {
            AnsiConsole.MarkupLine($"[red]{GetString("ModelIdInputFailed")}[/]");
            return;
        }

        string token;
        string folderPath;
        if (!isConfigFileExist || ignoreConfig)
        {
            token = AskCivitaiToken();
            folderPath = AskSaveFolderPath();
            if (string.IsNullOrEmpty(folderPath))
            {
                return;
            }
        }
        else
        {
            var config = JsonSerializer.Deserialize<Config>(File.ReadAllText(configFile));
            token = string.IsNullOrEmpty(config.CivitaiToken) ? AskCivitaiToken() : config.CivitaiToken;
            folderPath = string.IsNullOrEmpty(config.CivitaiSaveFolder) ? AskSaveFolderPath() : config.CivitaiSaveFolder;
            if (string.IsNullOrEmpty(folderPath))
            {
                return;
            }
        }

        _downloader = new Downloader();
        _downloader.InitializeCivitai(token, folderPath);

        try
        {
            var modelVersions = await _downloader.GetCivitaiModelAsync(modelId);
            if (modelVersions == null || modelVersions.Count == 0)
            {
                AnsiConsole.MarkupLine($"[red]{GetString("ModelNotFound")}[/]");
                return;
            }

            var selectedVersion = modelVersions.Count > 1
                ? AnsiConsole.Prompt(
                      new SelectionPrompt<ModelItem>()
                             .Title(GetString("SelectModelVersion"))
                             .PageSize(10)
                             .MoreChoicesText(GetString("MoreFileTip"))
                             .UseConverter(ConvertModelItemToString)
                             .AddChoices(modelVersions))
                : modelVersions[0];

            if (selectedVersion == null)
            {
                return;
            }

            var downloadItems = Downloader.GetCivitaiModelDownloadItems(selectedVersion);
            if (downloadItems == null || downloadItems.Count == 0)
            {
                AnsiConsole.MarkupLine($"[red]{GetString("NoAvailableDownloadLink")}[/]");
                return;
            }

            if (downloadItems.Count > 1)
            {
                AnsiConsole.MarkupLine($"[green]{string.Format(GetString("GetFileList"), selectedVersion.Name)}[/]");
                downloadItems = SelectionFiles(downloadItems);
            }

            RenderFileList(downloadItems);
            await TryDownloadFilesAsync(downloadItems, string.Empty);
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
        }
    }

    private static string AskCivitaiToken()
    {
        var optionalStr = GetString("Optional");
        var token = AnsiConsole.Ask<string>(GetString("CivitaiToken"), optionalStr);
        if (token == optionalStr)
        {
            token = string.Empty;
        }

        return token;
    }

    private static string ConvertModelItemToString(ModelItem item)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"[bold]{item.Name}[/]");
        sb.AppendLine($"[grey]{item.Description}[/]");
        return sb.ToString();
    }
}
