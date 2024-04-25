// Copyright (c) AI Downloader. All rights reserved.

using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Text;
using System.Text.Json;
using RodelDownloader.Aria;
using RodelDownloader.Aria.Models;
using RodelDownloader.Core;
using RodelDownloader.Core.Models;
using Spectre.Console;

/// <summary>
/// 入口.
/// </summary>
public partial class Program
{
    private static AriaClient _ariaClient;
    private static Downloader _downloader;
    private static List<LinkItem> _currentLinks;
    private static Dictionary<string, ProgressTask> _currentProgressTasks;
    private static CultureInfo _currentCulture;
    private static ResourceManager _resourceManager;

    private static void ConfigureConsole()
    {
        OutputEncoding = Encoding.UTF8;
        var culture = CultureInfo.CurrentCulture;
        _currentCulture = culture.TwoLetterISOLanguageName == "zh"
           ? new CultureInfo("zh-CN")
           : new CultureInfo("en-US");

        AppDomain.CurrentDomain.ProcessExit += (s, e) =>
        {
            ReleaseResources();
        };

        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            ReleaseResources();
        };

        CancelKeyPress += (s, e) =>
        {
            ReleaseResources();
            Environment.Exit(0);
        };
    }

    private static string GetString(string name)
    {
        _resourceManager ??= new ResourceManager("RodelDownloader.Console.Properties.Resources", typeof(Program).Assembly);
        return _resourceManager.GetString(name, _currentCulture) ?? name;
    }

    private static void KillAllAria2Process()
    {
        foreach (var process in Process.GetProcesses())
        {
            if (process.ProcessName == "aria2c")
            {
                try
                {
                    process.Kill();
                    AnsiConsole.WriteLine($"[green]{GetString("Aria2cCleaned")}[/]");
                }
                catch (Exception)
                {
                    AnsiConsole.WriteLine($"[red]{GetString("Aria2cNotClean")}[/]");
                }
            }
        }
    }

    private static void ReleaseResources()
    {
        _ariaClient?.Dispose();
        _downloader?.Dispose();
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

    private static AIType ConvertAITypeFromString(string type)
    {
        var t = type.ToLower() switch
        {
            "hf" => AIType.HuggingFace,
            "civitai" => AIType.Civitai,
            "ms" => AIType.ModelScope,
            _ => throw new Exception("Invalid AI type."),
        };

        return t;
    }

    private static string ChooseSaveFolders(string preferFolder, Dictionary<string, string> backupFolders, bool canAsk = true)
    {
        if (!string.IsNullOrEmpty(preferFolder) && Directory.Exists(preferFolder))
        {
            return preferFolder;
        }

        var selectedFolder = string.Empty;
        if (backupFolders != null && backupFolders.Count > 0)
        {
            if (backupFolders.Count > 1)
            {
                if (canAsk)
                {
                    var folderIndex = AnsiConsole.Prompt(
                       new SelectionPrompt<int>()
                          .Title(GetString("SelectSaveFolder"))
                          .PageSize(10)
                          .MoreChoicesText(GetString("MoreFolderTip"))
                          .UseConverter(ConvertFolderToString)
                          .AddChoices(Enumerable.Range(0, backupFolders.Count)));
                    selectedFolder = backupFolders.ElementAt(folderIndex).Value;
                }
                else
                {
                    selectedFolder = string.Empty;
                }
            }
            else
            {
                selectedFolder = backupFolders.First().Value;
            }

            if (!Directory.Exists(selectedFolder))
            {
                AnsiConsole.MarkupLine($"[red]{GetString("FolderNotExist")}[/]");
                selectedFolder = string.Empty;
            }
        }

        if (string.IsNullOrEmpty(selectedFolder) && canAsk)
        {
            selectedFolder = AskSaveFolderPath();
        }

        if (!string.IsNullOrEmpty(selectedFolder) && !Directory.Exists(selectedFolder))
        {
            AnsiConsole.MarkupLine($"[red]{GetString("FolderNotExist")}[/]");
            selectedFolder = string.Empty;
        }

        return selectedFolder;

        string ConvertFolderToString(int index)
        {
            var folder = backupFolders.ElementAt(index);
            var sb = new StringBuilder();
            sb.AppendLine($"[bold]{folder.Key}[/]");
            sb.AppendLine(folder.Value);
            return sb.ToString();
        }
    }

    private static void RenderFileList(List<DownloadItem> fileList)
    {
        var table = new Table();
        table.AddColumn("文件名");
        table.AddColumn("下载链接");

        foreach (var item in fileList)
        {
            table.AddRow(item.FileName, $"[grey]{item.Link}[/]");
        }

        table.Expand();
        AnsiConsole.Write(table);
    }

    private static List<DownloadItem> SelectionFiles(List<DownloadItem> fileList)
    {
        var prompt = new MultiSelectionPrompt<DownloadItem>()
                           .Title(GetString("ChooseDownloadFiles"))
                           .PageSize(10)
                           .AddChoices(fileList)
                           .MoreChoicesText($"[grey]{GetString("MoreFileTip")}[/]")
                           .InstructionsText(
                                $"[grey]({GetString("MoreFileOperationTip")})[/]");
        foreach (var item in fileList)
        {
            prompt.Select(item);
        }

        var selectedFiles = AnsiConsole.Prompt(prompt);
        return selectedFiles;
    }

    private static async Task TryDownloadFilesAsync(List<DownloadItem> items, string accessToken = "")
    {
        var processFolder = AppDomain.CurrentDomain.BaseDirectory;
        if (_ariaClient == null)
        {
            var ariaFilePath = Path.Combine(processFolder, "aria2c.exe");
            var ariaConfigPath = Path.Combine(processFolder, "aria2.conf");
            _ariaClient = new AriaClient(ariaFilePath, ariaConfigPath, 9600, retryCount: 3);
            AnsiConsole.MarkupLine($"[gray]{GetString("Aria2cInitialized")}[yellow]9600[/][/]");
        }

        _currentLinks ??= new List<LinkItem>();
        AnsiConsole.MarkupLine($"[bold]{GetString("BeginAddFiles")}[/]");
        var table = new Table();
        table.AddColumn(GetString("FileName"));
        table.AddColumn("GID");
        await AnsiConsole.Live(table)
            .AutoClear(false)
            .Overflow(VerticalOverflow.Ellipsis)
            .Cropping(VerticalOverflowCropping.Top)
            .StartAsync(async ctx =>
            {
                var ariaConfigPath = Path.Combine(processFolder, "aria2.conf");
                foreach (var item in items)
                {
                    var options = new Dictionary<string, object>
                    {
                        { "out", item.FileName },
                        { "dir", item.TargetFolder },
                        { "config-path", $"\"{ariaConfigPath}\"" },
                    };

                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        options.Add("header", $"Authorization: Bearer {accessToken}");
                    }

                    var gid = await _ariaClient.AddUriAsync([item.Link], options);
                    _currentLinks.Add(new LinkItem { Link = item.Link, Name = item.FileName, Gid = gid });
                    table.AddRow(item.FileName, $"[grey]{gid}[/]");
                }
            });

        AnsiConsole.MarkupLine($"[bold]{string.Format(GetString("FileAdded"), items.Count)}[/]");
        AnsiConsole.MarkupLine($"[bold]{GetString("BeginDownload")}[/]");

        await AnsiConsole.Progress()
            .Columns(
            [
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new DownloadedColumn(),
                new SpinnerColumn(),
            ])
            .StartAsync(async ctx =>
            {
                _currentProgressTasks ??= new Dictionary<string, ProgressTask>();
                foreach (var item in _currentLinks)
                {
                    var status = await _ariaClient.TellStatusAsync(item.Gid);
                    var maxLength = status?.TotalLength ?? "100";
                    var t = ctx.AddTask(item.Name, new ProgressTaskSettings
                    {
                        MaxValue = long.Parse(maxLength),
                    });
                    _currentProgressTasks.Add(item.Gid, t);
                }

                var finishTasks = new List<string>();
                while (true)
                {
                    if (finishTasks.Count == _currentProgressTasks.Count)
                    {
                        AnsiConsole.MarkupLine($"[bold green]{GetString("DownloadCompleted")}[/]");
                        break;
                    }

                    foreach (var item in _currentProgressTasks)
                    {
                        if (finishTasks.Contains(item.Key))
                        {
                            continue;
                        }

                        var status = await _ariaClient.TellStatusAsync(item.Key);
                        var t = item.Value;
                        if (status == null)
                        {
                            continue;
                        }

                        t.MaxValue = long.Parse(status.TotalLength);
                        t.Value = long.Parse(status.CompletedLength);

                        var linkItem = _currentLinks.FirstOrDefault(m => m.Gid == item.Key);

                        if ((status.Status == "complete" || status.Status == "error") && !finishTasks.Contains(item.Key))
                        {
                            if (status.Status == "error")
                            {
                                AnsiConsole.MarkupLine($"[red]{string.Format(GetString("DownloadError"), item.Value.Description)}\n{status.ErrorCode}: {status.ErrorMessage}[/]");
                                item.Value.StopTask();
                            }

                            finishTasks.Add(item.Key);
                        }
                    }

                    await Task.Delay(1000);
                }
            });
    }

    private static string GetConfigPath()
        => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

    private static void EditOrCreateConfig()
    {
        var configPath = GetConfigPath();
        if (!File.Exists(configPath))
        {
            File.Create(configPath).Close();
            var config = new DownloaderConfig
            {
                HuggingFaceToken = string.Empty,
                HuggingFaceSaveFolder = string.Empty,
                HuggingFaceBackupFolders = new Dictionary<string, string>(),
                HuggingFaceUriType = "official",
                CivitaiToken = string.Empty,
                CivitaiSaveFolder = string.Empty,
                CivitaiBackupFolders = new Dictionary<string, string>(),
                ModelScopeToken = string.Empty,
                ModelScopeSaveFolder = string.Empty,
                ModelScopeBackupFolders = new Dictionary<string, string>(),
            };
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(configPath, json);
        }

        Process.Start(new ProcessStartInfo(configPath) { UseShellExecute = true });
    }
}
