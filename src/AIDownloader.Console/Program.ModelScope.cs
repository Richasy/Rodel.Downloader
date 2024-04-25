// Copyright (c) AI Downloader. All rights reserved.

using System.Text.Json;
using AIDownloader.Core;
using AIDownloader.Core.Models;
using Spectre.Console;

/// <summary>
/// 入口.
/// </summary>
public partial class Program
{
    private static async Task RunModelScopeDownloadAsync(bool ignoreConfig = false)
    {
        var configFile = GetConfigPath();
        var isConfigFileExist = File.Exists(configFile);

        var modelId = AnsiConsole.Ask<string>(GetString("ModelIdInput"));
        if (string.IsNullOrEmpty(modelId) || !modelId.Contains('/'))
        {
            AnsiConsole.MarkupLine($"[red]{GetString("ModelIdInputFailed")}[/]");
            return;
        }

        string token;
        string folderPath;
        if (!isConfigFileExist || ignoreConfig)
        {
            token = AskModelScopeToken();
            folderPath = AskSaveFolderPath();
            if (string.IsNullOrEmpty(folderPath))
            {
                return;
            }
        }
        else
        {
            var config = JsonSerializer.Deserialize<DownloaderConfig>(File.ReadAllText(configFile));
            token = config.ModelScopeToken ?? string.Empty;

            folderPath = ChooseSaveFolders(config.ModelScopeSaveFolder, config.ModelScopeBackupFolders);
            if (string.IsNullOrEmpty(folderPath))
            {
                return;
            }
        }

        var savePath = Path.Combine(folderPath, modelId.Split('/').Last());
        _downloader ??= new Downloader();
        _downloader.InitializeModelScope(token, savePath);

        try
        {
            var links = await _downloader.GetModelScopeModelAsync(modelId);
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

    private static string AskModelScopeToken()
    {
        var optionalStr = GetString("Optional");
        var token = AnsiConsole.Ask<string>(GetString("ModelScopeToken"), optionalStr);
        if (token == optionalStr)
        {
            token = string.Empty;
        }

        return token;
    }
}
