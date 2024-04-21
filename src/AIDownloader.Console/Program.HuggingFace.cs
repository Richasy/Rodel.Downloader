// Copyright (c) AI Downloader. All rights reserved.

using System.Text.Json;
using AIDownloader.Core;
using Spectre.Console;

/// <summary>
/// 入口.
/// </summary>
public partial class Program
{
    private static async Task RunHuggingFaceDownloadAsync(bool ignoreConfig = false)
    {
        var configFile = Path.Combine(Environment.CurrentDirectory, "config.json");
        var isConfigFileExist = File.Exists(configFile);

        var modelId = AnsiConsole.Ask<string>(GetString("ModelIdInput"));
        if (string.IsNullOrEmpty(modelId) || !modelId.Contains('/'))
        {
            AnsiConsole.MarkupLine($"[red]{GetString("ModelIdInputFailed")}[/]");
            return;
        }

        string token;
        string folderPath;
        HuggingFaceUriType hfUriType;
        if (!isConfigFileExist || ignoreConfig)
        {
            token = AskHfToken();
            if (string.IsNullOrEmpty(token))
            {
                return;
            }

            folderPath = AskSaveFolderPath();
            if (string.IsNullOrEmpty(folderPath))
            {
                return;
            }

            hfUriType = AskHfUriType();
        }
        else
        {
            var config = JsonSerializer.Deserialize<Config>(File.ReadAllText(configFile));
            token = string.IsNullOrEmpty(config.Token) ? AskHfToken() : config.Token;
            if (string.IsNullOrEmpty(token))
            {
                return;
            }

            folderPath = string.IsNullOrEmpty(config.SaveFolder) ? AskSaveFolderPath() : config.SaveFolder;
            if (string.IsNullOrEmpty(folderPath))
            {
                return;
            }

            hfUriType = string.IsNullOrEmpty(config.UriType)
                ? AskHfUriType()
                : config.UriType.ToLower() switch
                {
                    "mirror" => HuggingFaceUriType.Mirror,
                    _ => HuggingFaceUriType.Official
                };
        }

        var savePath = Path.Combine(folderPath, modelId.Split('/').Last());
        _downloader = new Downloader();
        _downloader.InitializeHuggingFace(hfUriType, token, savePath);

        try
        {
            var links = await _downloader.GetHuggingFaceModelAsync(modelId);
            if (links == null || links.Count == 0)
            {
                AnsiConsole.MarkupLine($"[red]{GetString("NoAvailableDownloadLink")}[/]");
                return;
            }

            AnsiConsole.MarkupLine($"[green]{string.Format(GetString("GetFileList"), modelId)}[/]");
            var downloadList = SelectionFiles(links);
            RenderFileList(downloadList);
            await TryDownloadFilesAsync(downloadList, token);
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
        }
    }

    private static string AskHfToken()
    {
        var token = AnsiConsole.Ask<string>(GetString("HuggingFaceToken"));
        if (string.IsNullOrEmpty(token) || !token.StartsWith("hf"))
        {
            AnsiConsole.MarkupLine($"[red]{GetString("InvalidToken")}[/]");
            return string.Empty;
        }

        return token;
    }

    private static string AskSaveFolderPath()
    {
        var folderPath = AnsiConsole.Ask<string>(GetString("FolderPickerTip"));
        if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
        {
            AnsiConsole.MarkupLine($"[red]{GetString("NoSelectedFolder")}[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"{GetString("SelectedFolder")}[green]{folderPath}[/]");
        }

        return folderPath;
    }

    private static HuggingFaceUriType AskHfUriType()
    {
        var hfUriType = AnsiConsole.Prompt(
               new SelectionPrompt<HuggingFaceUriType>()
                      .Title(GetString("HuggingFaceSource"))
                      .PageSize(10)
                      .AddChoices([HuggingFaceUriType.Official, HuggingFaceUriType.Mirror]));
        return hfUriType;
    }
}
