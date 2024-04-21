// Copyright (c) AI Downloader. All rights reserved.

using Spectre.Console;

ConfigureConsole();

var ignoreConfig = args.Contains("--ignore-config");
var shouldCleanUp = args.Contains("--clean-up");

if (shouldCleanUp)
{
    KillAllAria2Process();
}

var type = AnsiConsole.Prompt(
     new SelectionPrompt<AIType>()
         .Title(GetString("AITypeSelection"))
         .PageSize(10)
         .AddChoices(AIType.HuggingFace, AIType.Civitai));

if (type == AIType.HuggingFace)
{
    await RunHuggingFaceDownloadAsync(ignoreConfig);
}
else
{
    await RunCivitaiDownloadAsync(ignoreConfig);
}
