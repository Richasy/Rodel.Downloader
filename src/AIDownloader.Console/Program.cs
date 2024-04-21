// Copyright (c) AI Downloader. All rights reserved.

ConfigureConsole();

var ignoreConfig = args.Contains("--ignore-config");
var shouldCleanUp = args.Contains("--clean-up");

if (shouldCleanUp)
{
    KillAllAria2Process();
}

await RunHuggingFaceDownloadAsync(ignoreConfig);
