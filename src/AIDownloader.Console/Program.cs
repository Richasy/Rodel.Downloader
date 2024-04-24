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
         .UseConverter(ConvertAITypeToString)
         .AddChoices(AIType.HuggingFace, AIType.Civitai, AIType.ModelScope));

if (type == AIType.HuggingFace)
{
    await RunHuggingFaceDownloadAsync(ignoreConfig);
}
else if (type == AIType.Civitai)
{
    await RunCivitaiDownloadAsync(ignoreConfig);
}
else if (type == AIType.ModelScope)
{
    await RunModelScopeDownloadAsync(ignoreConfig);
}

string ConvertAITypeToString(AIType type)
{
    return type switch
    {
        AIType.HuggingFace => "Hugging Face",
        AIType.Civitai => "Civitai",
        AIType.ModelScope => GetString("ModelScope"),
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };
}
