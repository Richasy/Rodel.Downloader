<a name="readme-top"></a>

<div align="center">

<img height="160" src="./assets/logo.png">

<h1 align="center">AI Downloader</h1>

A dedicated downloader for downloading AI model files, which can stably and reliably download your favorite models from Hugging Face and Civitai. It supports both command line and UI, and is currently only available on Windows.

English · [简体中文](./README.zh-CN.md)

<!-- SHIELD GROUP -->

[![][github-release-shield]][github-release-link]
[![][github-releasedate-shield]][github-releasedate-link]
[![][github-contributors-shield]][github-contributors-link]
[![][github-forks-shield]][github-forks-link]
[![][github-stars-shield]][github-stars-link]
[![][github-issues-shield]][github-issues-link]
[![][github-license-shield]][github-license-link]

</div>

> \[!WARNING]
>
> The application is developed based on .NET 8, it is not a cross-platform application, and is currently only available on **Windows**. For users below Windows 10 19043, please use the command line instead of the UI.

<details>
<summary><kbd>Table of contents</kbd></summary>

#### TOC

- [✨ Feature Overview](#-feature-overview)
  - [`1` CLI \& APP](#1-cli--app)
  - [`2` Built-in Aria2](#2-built-in-aria2)
- [🎛️ Environment Support](#️-environment-support)
- [�️ CLI Instruction Manual](#️-cli-instruction-manual)
    - [`1` Install](#1-install)
- [🔗 Links](#-links)

####

</details>

## ✨ Feature Overview

### `1` CLI & APP

The application, with aria2 at its core, offers two modes of use:

- Command line
- UI

This caters to the needs of different users.

Both the command line and UI have simple localization support (supporting `en-US` and `zh-CN`), and can switch automatically according to the current system language.

> \[!TIP]
>
> The UI is based on the Windows App SDK, which requires your system version to be Windows 10 19043 and above. It is highly recommended to download and install from the Microsoft Store.

### `2` Built-in Aria2

I have to say, the sole motivation for building this tool was that I couldn't find a convenient, reliable download tool to download entire repositories from hugging face. Either there's no progress indication, or the robustness is too poor.

My technical skills aren't great, but [aria2](https://github.com/aria2/aria2) has left a good impression on me in the past, so I chose to build a simple download tool based on it.

The download has the following features:

1. Customizable download directory
2. Support for resuming downloads
3. Complete progress display
4. Ability to operate on individual items, pause/resume/cancel **(App only)**

Both CLI and APP come with a built-in `1.3.7` **aria2c.exe**, no additional download required, trying to be as plug and play as possible.

## 🎛️ Environment Support

|||
|-|-|
|Framework|.NET 8|
|UI Framework|Windows App SDK 1.5|
|System Requirements|`CLI`: Windows 7 and above, `APP`: Windows 10 19043 and above|

## 🛠️ CLI Instruction Manual

#### `1` Install

1. Download the latest `AIDownloader-CLI.zip` from the Release list of the repository.
2. Unzip the downloaded package to a directory you prefer.
3. Add the unzipped directory to the system or user's **PATH** environment variable.
4. Open the command line you are familiar with and enter `ai-downloader`.

## 🔗 Links

- [Spectre.Console](https://spectreconsole.net)
- [Windows App SDK](https://github.com/microsoft/WindowsAppSDK)
- [aria2](https://github.com/aria2/aria2)
- [Aria2.NET](https://github.com/rogerfar/Aria2.NET)
- [CommunityToolkit](https://github.com/CommunityToolkit)
- [Hugging Face](https://huggingface.co)
- [Hugging Face Mirror](https://hf-mirror.com)
- [Civitai](https://civitai.com)

<!-- LINK GROUP -->
[github-contributors-link]: https://github.com/Richasy/AIDownloader/graphs/contributors
[github-contributors-shield]: https://img.shields.io/github/contributors/Richasy/AIDownloader?color=c4f042&labelColor=black&style=flat-square
[github-forks-link]: https://github.com/Richasy/AIDownloader/network/members
[github-forks-shield]: https://img.shields.io/github/forks/Richasy/AIDownloader?color=8ae8ff&labelColor=black&style=flat-square
[github-issues-link]: https://github.com/Richasy/AIDownloader/issues
[github-issues-shield]: https://img.shields.io/github/issues/Richasy/AIDownloader?color=ff80eb&labelColor=black&style=flat-square
[github-license-link]: https://github.com/Richasy/AIDownloader/blob/main/LICENSE
[github-license-shield]: https://img.shields.io/github/license/Richasy/AIDownloader?color=white&labelColor=black&style=flat-square
[github-release-link]: https://github.com/Richasy/AIDownloader/releases
[github-release-shield]: https://img.shields.io/github/v/release/Richasy/AIDownloader?color=369eff&labelColor=black&logo=github&style=flat-square
[github-releasedate-link]: https://github.com/Richasy/AIDownloader/releases
[github-releasedate-shield]: https://img.shields.io/github/release-date/Richasy/AIDownloader?labelColor=black&style=flat-square
[github-stars-link]: https://github.com/Richasy/AIDownloader/network/stargazers
[github-stars-shield]: https://img.shields.io/github/stars/Richasy/AIDownloader?color=ffcb47&labelColor=black&style=flat-square