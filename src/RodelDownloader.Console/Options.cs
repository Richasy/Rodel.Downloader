// Copyright (c) AI Downloader. All rights reserved.

using CommandLine;

internal class Options
{
    [Option("ignore-config", Required = false, Default = false, HelpText = "Ignore the local configuration and use the default process for interaction.")]
    public bool IgnoreConfig { get; set; }

    [Option("clean-up", Required = false, Default = false, HelpText = "Kill all aria2c processes before starting the download.")]
    public bool ShouldCleanUp { get; set; }

    [Option("edit-config", Required = false, Default = false, HelpText = "Edit the configuration file.")]
    public bool EditConfig { get; set; }

    [Option('n', "no-interaction", Required = false, Default = false, HelpText = "Disable interaction and use the command-line parameters.")]
    public bool DisabledInteraction { get; set; }

    [Option('m', "model-id", Required = false, HelpText = "The model ID to download.")]
    public string ModelId { get; set; }

    [Option('s', "service", Required = false, Default = "hf", HelpText = "The service to download the model from. Support hf | civitai | ms")]
    public string Service { get; set; }

    [Option("use-hf-mirror", Required = false, Default = false, HelpText = "Use the hf-mirror (https://hf-mirror.com/) for downloading. Only available when service is hf.")]
    public bool UseHuggingFaceMirror { get; set; }

    [Option('t', "token", Required = false, Default = "", HelpText = "The token to use for authentication.")]
    public string Token { get; set; }

    [Option('d', "save-dir", Required = false, Default = "", HelpText = "The directory to save the model to.")]
    public string SaveDir { get; set; }

    [Option('i', "include", Required = false, Default = "", HelpText = "The files to include in the download.")]
    public string Include { get; set; }

    [Option('e', "exclude", Required = false, Default = "", HelpText = "The files to exclude in the download.")]
    public string Exclude { get; set; }
}
