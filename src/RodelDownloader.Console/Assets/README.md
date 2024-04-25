# 🛠️ CLI Instruction Manual

## `1` Install

> \[!WARNING]
>
> CLI relies on .NET 8 framework, please make sure that `.NET 8 Desktop Runtime` or `SDK` is installed on your device.
> You can download `.NET SDK` or `.NET Desktop Runtime` at [Download .NET 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).

1. Open PowerShell
2. Enter the command
   ```powershell
   dotnet-tool install --global RodelDownloader.CLI
   ```
3. After the installation is complete, enter the command `rodel-downloader` to enter the interactive command interface
4. You can also enter the command `rodel-downloader --help` to view detailed parameter definitions

## `2` Interactive Commands

The CLI defaults to an interactive mode to guide you through the download process.

After installation, you can simply enter `rodel-downloader` to access the interactive interface.

The basic download steps are as follows:

1. Choose the model hosting service. Currently, `Hugging Face / HF-Mirror`, `Civitai`, and `Model Scope` are supported.
2. Enter the model ID you want to download. This model ID is usually provided by the hosting service.
3. The CLI will search for the repository and download file list corresponding to the model ID. Once found, it will be listed and you can freely choose which files to download.
4. Start downloading. The CLI will provide progress tips. You can wait for the download to complete or press `Ctrl` + `C` at any time to interrupt the download.

## `3` Standard Commands

The CLI also accepts another mode of operation, which is the regular parameter call.

When you need to use parameter calls, you must first disable the interactive mode, which is `-n` or `--no-interaction`.

After that, you need to provide the specified parameters.

```
  --ignore-config         (Default: false) Ignore the local configuration and use the default process for interaction.

  --clean-up              (Default: false) Kill all aria2c processes before starting the download.

  --edit-config           (Default: false) Edit the configuration file.

  -n, --no-interaction    (Default: false) Disable interaction and use the command-line parameters.

  -m, --model-id          The model ID to download.

  -s, --service           (Default: hf) The service to download the model from. Support hf | civitai | ms

  --use-hf-mirror         (Default: false) Use the hf-mirror (https://hf-mirror.com/) for downloading. Only available
                          when service is hf.

  -t, --token             (Default: ) The token to use for authentication.

  -d, --save-dir          (Default: ) The directory to save the model to.

  -i, --include           (Default: ) The files to include in the download.

  -e, --exclude           (Default: ) The files to exclude in the download.

  --help                  Display this help screen.

  --version               Display version information.
```

### Example

Download the `microsoft/Phi-3-mini-4k-instruct` model from Hugging Face:

```powershell
rodel-downloader -n -m "microsoft/Phi-3-mini-4k-instruct" -s hf --token "hf-xxxxxxxx" -d "C:\Models"
```

## `4` Configuration and Saving

Repeated input can be quite troublesome. The CLI supports using a configuration file to fix optional parameters, simplifying input each time it is called.

Enter the following command in the command line:

```powershell
rodel-downloader --edit-config
```

The application will call the default editor to open the configuration file `config.json` (if it does not exist, create a new one), the specific parameters are as follows:

```json
{
  "hf_token": "",
  "hf_save_folder": "",
  "hf_backup_folders": {
    "folder1": "path1",
    "folder2": "path2"
  },
  "hf_uri_type": "{official} or {mirror}",

  "civitai_token": "",
  "civitai_save_folder": "",
  "civitai_backup_folders": {
    "folder1": "path1",
    "folder2": "path2"
  },

  "ms_token": "",
  "ms_save_folder": "",
  "ms_backup_folders": {
    "folder1": "path1",
    "folder2": "path2"
  }
}
```

1. `*_token`
   This is the access token for the corresponding service. Here, `hf` stands for Hugging Face, and `ms` stands for Model Scope.
2. `*_save_folder` and `*_backup_folders`
   These are a pair of mutually exclusive properties, and the application prefers to use `*_save_folder`.
   - If the model you download will only be saved in a specific folder, fill in `*_save_folder`, and the CLI will download the model of the corresponding service to this folder.
   - If you have multiple optional locations, such as downloading the models required for SD-WebUI (check point, lora...), you can fill in the corresponding paths into `*_backup_folders`, where `key` is the readable name of the folder path, and `value` is its absolute path. When running the CLI, you can choose from the defined folder list.
   
   > \[!WARNING]
   >
   > The "save folder" means the parent folder for storing the model. For `Hugging Face` and `Model Scope`, the application will create a subfolder with the same name as the model in this folder as the directory for storing model files.
   >
   > For example, if you specify `C:\MyFolder` as the save folder, then after you download the Llama 3 8B model, the actual model file folder path is `C:\MyFolder\Meta-Llama-3-8B`.
   >
   > **But for `Civitai`, since it is usually a single file download, the application will not create a same-name subfolder, but directly download the model file to the specified storage directory.**

### Overview of Access Token Acquisition

|||
|-|-|
|Hugging Face| [User Access Tokens](https://huggingface.co/docs/hub/security-tokens) |
|Civitai | [Civitai's Guide to Downloading via API](https://education.civitai.com/civitais-guide-to-downloading-via-api/) |
|Model Scope|[Access Token](https://www.modelscope.cn/my/myaccesstoken), `Model Scope Personal Center -> Access Token`|

## `5` Resume from Breakpoint

The application is based on aria2, so it has the ability to resume from breakpoints. The management and recovery of download progress is controlled by aria2.

If for some reason, you interrupt the download.

Recovery is also simple, just enter the same parameters as the last call.

Make sure your `model ID`, `save path`, and `hosting service` are the same as before.

> \[!TIP]
>
> The basis for resuming from a breakpoint is that you still have the files from the last download, and the binary files with the suffix `.aria2` in the same directory, which save your download progress.
> 
> If the corresponding files are deleted, you need to download again.