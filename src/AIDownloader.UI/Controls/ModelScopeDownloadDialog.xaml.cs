﻿// Copyright (c) AI Downloader. All rights reserved.

using System.Text.Json;
using AIDownloader.UI.Models;
using AIDownloader.UI.Models.Constants;
using AIDownloader.UI.Toolkits;
using AIDownloader.UI.ViewModels;
using NLog;

namespace AIDownloader.UI.Controls;

/// <summary>
/// ModelScope 下载对话框.
/// </summary>
public sealed partial class ModelScopeDownloadDialog : ContentDialog
{
    private readonly DownloadPageViewModel _pageVM;
    private readonly ObservableCollection<FolderItem> _folders = new();
    private readonly ObservableCollection<FileItemViewModel> _files = new();
    private string _modelId;
    private string _savePath;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModelScopeDownloadDialog"/> class.
    /// </summary>
    public ModelScopeDownloadDialog(DownloadPageViewModel pageVM)
    {
        InitializeComponent();
        AppToolkit.ResetControlTheme(this);
        _pageVM = pageVM;
        var saveFolders = SettingsToolkit.ReadLocalSetting(SettingNames.ModelScopeSaveFolders, "[]");
        var folders = JsonSerializer.Deserialize<List<FolderItem>>(saveFolders);
        foreach (var item in folders)
        {
            _folders.Add(item);
        }

        var lastFolder = SettingsToolkit.ReadLocalSetting(SettingNames.ModelScopeLastSaveFolder, string.Empty);
        if (!string.IsNullOrEmpty(lastFolder) && !_folders.Any(p => p.Path.Equals(lastFolder)))
        {
            _folders.Add(new FolderItem
            {
                Name = Path.GetFileName(lastFolder),
                Path = lastFolder,
            });
        }

        var selectedFolderItem = _folders.FirstOrDefault(p => p.Path.Equals(lastFolder));
        if (selectedFolderItem is not null)
        {
            SaveFolderBox.SelectedItem = selectedFolderItem;
        }

        var lastModelId = SettingsToolkit.ReadLocalSetting(SettingNames.ModelScopeLastModelId, string.Empty);
        if (!string.IsNullOrEmpty(lastModelId))
        {
            ModelIdBox.Text = lastModelId;
        }

        CheckSelectAllContent();
    }

    private async Task FetchModelAsync()
    {
        IsPrimaryButtonEnabled = false;
        try
        {
            InputContainer.Visibility = Visibility.Collapsed;
            DetailContainer.Visibility = Visibility.Visible;
            _files.Clear();
            FetchRing.IsActive = true;
            var downloader = _pageVM.GetDownloader();
            var token = SettingsToolkit.ReadLocalSetting(SettingNames.ModelScopeToken, string.Empty);
            downloader.InitializeModelScope(token, Path.Combine(_savePath, _modelId.Split('/').Last()));
            var fileItems = await downloader.GetModelScopeModelAsync(_modelId);
            if (fileItems is null || fileItems.Count == 0)
            {
                AppViewModel.Instance.ShowTip(ResourceToolkit.GetLocalizedString(StringNames.NoAvailableDownloadItems), InfoType.Error);
            }
            else
            {
                foreach (var item in fileItems)
                {
                    _files.Add(new FileItemViewModel(item));
                }
            }
        }
        catch (Exception ex)
        {
            InputContainer.Visibility = Visibility.Visible;
            DetailContainer.Visibility = Visibility.Collapsed;
            AppViewModel.Instance.ShowTip(ex.Message, InfoType.Error);
            LogManager.GetCurrentClassLogger().Error(ex);
        }

        IsPrimaryButtonEnabled = true;
        FetchRing.IsActive = false;
    }

    private async void OnPrimaryButtonClickAsync(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        args.Cancel = true;
        if (InputContainer.Visibility == Visibility.Visible)
        {
            var id = ModelIdBox.Text;
            if (string.IsNullOrEmpty(id) || !id.Contains('/'))
            {
                AppViewModel.Instance.ShowTip(StringNames.InvalidModelId, InfoType.Error);
                return;
            }

            _modelId = id;

            if (SaveFolderBox.SelectedItem is not FolderItem saveFolder || !saveFolder.IsAvailable())
            {
                AppViewModel.Instance.ShowTip(StringNames.InvalidSavePath, InfoType.Error);
                return;
            }

            _savePath = saveFolder.Path;
            SettingsToolkit.WriteLocalSetting(SettingNames.ModelScopeLastSaveFolder, _savePath);
            await FetchModelAsync();
        }
        else if (DetailContainer.Visibility == Visibility.Visible)
        {
            var selectedItems = _files.Where(p => p.IsSelected).Select(p => p.Data).ToList();
            if (selectedItems.Count == 0)
            {
                AppViewModel.Instance.ShowTip(StringNames.NoItemSelected, InfoType.Error);
                return;
            }

            SettingsToolkit.WriteLocalSetting(SettingNames.ModelScopeLastModelId, _modelId);
            _pageVM.AddDownloadItemsCommand.Execute(selectedItems);
            Hide();
        }
    }

    private async void OnChooseOtherButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var folder = await FileToolkit.PickFolderAsync(AppViewModel.Instance.ActivatedWindow);
        if (folder is not null)
        {
            var existItem = _folders.FirstOrDefault(f => f.Path == folder.Path);
            if (existItem != null)
            {
                SaveFolderBox.SelectedItem = existItem;
            }
            else
            {
                var newItem = new FolderItem
                {
                    Name = Path.GetFileName(folder.Path),
                    Path = folder.Path,
                };

                _folders.Add(newItem);
                SaveFolderBox.SelectedItem = newItem;
            }
        }
    }

    private void OnSelectAllButtonClick(object sender, RoutedEventArgs e)
    {
        var isSelectAll = _files.All(p => p.IsSelected);
        if (isSelectAll)
        {
            foreach (var item in _files)
            {
                item.IsSelected = false;
            }
        }
        else
        {
            foreach (var item in _files)
            {
                item.IsSelected = true;
            }
        }

        CheckSelectAllContent();
    }

    private void CheckSelectAllContent()
    {
        var isSelectAll = _files.All(p => p.IsSelected);
        SelectAllButton.Content = isSelectAll
            ? ResourceToolkit.GetLocalizedString(StringNames.DeselectAll)
            : ResourceToolkit.GetLocalizedString(StringNames.SelectAll);
    }

    private void OnFileItemClick(object sender, RoutedEventArgs e)
        => CheckSelectAllContent();
}
