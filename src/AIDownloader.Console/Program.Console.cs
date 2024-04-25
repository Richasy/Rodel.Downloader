// Copyright (c) AI Downloader. All rights reserved.

using System.Text.Json;
using AIDownloader.Core;
using AIDownloader.Core.Models;
using Spectre.Console;

/// <summary>
/// 控制台入口.
/// </summary>
public partial class Program
{
    private static async Task RunConsoleDownloadAsync(Options options)
    {
        var configFile = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), "config.json");
        var isConfigFileExist = File.Exists(configFile);
        var config = isConfigFileExist ? JsonSerializer.Deserialize<DownloaderConfig>(File.ReadAllText(configFile)) : default;

        var service = options.Service;
        if (string.IsNullOrEmpty(service))
        {
            AnsiConsole.MarkupLine($"[red]{GetString("ServiceInputFailed")}[/]");
            return;
        }

        var aiType = ConvertAITypeFromString(service);
        var modelId = options.ModelId;
        if (string.IsNullOrEmpty(modelId))
        {
            AnsiConsole.MarkupLine($"[red]{GetString("ModelIdInputFailed")}[/]");
            return;
        }

        var saveDir = options.SaveDir;
        if (string.IsNullOrEmpty(saveDir))
        {
            if (config != null && !options.IgnoreConfig)
            {
                var preferFolder = aiType switch
                {
                    AIType.HuggingFace => config.HuggingFaceSaveFolder,
                    AIType.Civitai => config.CivitaiSaveFolder,
                    AIType.ModelScope => config.ModelScopeSaveFolder,
                    _ => string.Empty
                };

                var backupFolders = aiType switch
                {
                    AIType.HuggingFace => config.HuggingFaceBackupFolders,
                    AIType.Civitai => config.CivitaiBackupFolders,
                    AIType.ModelScope => config.ModelScopeBackupFolders,
                    _ => default,
                };

                saveDir = ChooseSaveFolders(preferFolder, backupFolders);
            }

            if (string.IsNullOrEmpty(saveDir))
            {
                return;
            }
        }

        var token = options.Token;
        if (string.IsNullOrEmpty(token))
        {
            if (config != null && !options.IgnoreConfig)
            {
                token = aiType switch
                {
                    AIType.HuggingFace => config.HuggingFaceToken,
                    AIType.Civitai => config.CivitaiToken,
                    AIType.ModelScope => config.ModelScopeToken,
                    _ => string.Empty
                };
            }

            if (string.IsNullOrEmpty(token) && aiType == AIType.HuggingFace)
            {
                AnsiConsole.MarkupLine($"[red]{GetString("HuggingFaceNeedToken")}[/]");
                return;
            }
        }

        var include = options.Include;
        var exclude = options.Exclude;

        List<DownloadItem> items = default;
        try
        {
            _downloader ??= new Downloader();

            if (aiType == AIType.HuggingFace)
            {
                var isMirror = options.UseHuggingFaceMirror;
                var savePath = Path.Combine(saveDir, modelId.Split('/').Last());
                var hfUriType = isMirror ? HuggingFaceUriType.Mirror : HuggingFaceUriType.Official;
                _downloader.InitializeHuggingFace(hfUriType, token, savePath);
                var links = await _downloader.GetHuggingFaceModelAsync(modelId);
                if (links == null || links.Count == 0)
                {
                    AnsiConsole.MarkupLine($"[red]{GetString("NoAvailableDownloadLink")}[/]");
                    return;
                }

                items = links;
            }
            else if (aiType == AIType.Civitai)
            {
                var modelVersions = await _downloader.GetCivitaiModelAsync(modelId);
                if (modelVersions == null || modelVersions.Count == 0)
                {
                    AnsiConsole.MarkupLine($"[red]{GetString("ModelNotFound")}[/]");
                    return;
                }

                var selectedVersion = modelVersions.FirstOrDefault();
                if (selectedVersion == null)
                {
                    return;
                }

                var downloadItems = _downloader.GetCivitaiModelDownloadItems(selectedVersion);
                if (downloadItems == null || downloadItems.Count == 0)
                {
                    AnsiConsole.MarkupLine($"[red]{GetString("NoAvailableDownloadLink")}[/]");
                    return;
                }

                items = downloadItems;
            }
            else if (aiType == AIType.ModelScope)
            {
                var savePath = Path.Combine(saveDir, modelId.Split('/').Last());
                _downloader.InitializeModelScope(token, savePath);

                var links = await _downloader.GetModelScopeModelAsync(modelId);
                if (links == null || links.Count == 0)
                {
                    AnsiConsole.MarkupLine($"[red]{GetString("NoAvailableDownloadLink")}[/]");
                    return;
                }

                items = links;
            }

            if (!string.IsNullOrEmpty(include) || !string.IsNullOrEmpty(exclude))
            {
                var filterItems = FilterFiles(include, exclude, items.Select(x => x.FileName).ToList());
                items = items.Where(x => filterItems.Contains(x.FileName)).ToList();
            }

            AnsiConsole.MarkupLine($"[green]{string.Format(GetString("GetFileList"), modelId)}[/]");
            RenderFileList(items);
            await TryDownloadFilesAsync(items, token);
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
        }

        List<string> FilterFiles(string include, string exclude, List<string> files)
        {
            var result = new List<string>();

            // Split the include and exclude parameters by space
            var includePatterns = include.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var excludePatterns = exclude.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);

                var isIncluded = includePatterns.Length == 0 || includePatterns.Any(pattern => IsMatch(pattern, fileName));
                var isExcluded = excludePatterns.Length > 0 && excludePatterns.Any(pattern => IsMatch(pattern, fileName));

                if (isIncluded && !isExcluded)
                {
                    result.Add(file);
                }
            }

            return result;
        }

        bool IsMatch(string pattern, string input)
        {
            var regexPattern = "^" + System.Text.RegularExpressions.Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";
            return System.Text.RegularExpressions.Regex.IsMatch(input, regexPattern);
        }
    }
}
