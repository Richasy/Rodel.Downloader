// Copyright (c) AI Downloader. All rights reserved.

using System.Text.Json;
using AIDownloader.Core.Models;
using AIDownloader.UI.Models;
using AIDownloader.UI.Models.Constants;
using AIDownloader.UI.Toolkits;
using AIDownloader.UI.ViewModels;
using NLog;

namespace AIDownloader.UI.Controls;

/// <summary>
/// Civitai 下载对话框.
/// </summary>
public sealed partial class CivitaiDownloadDialog : ContentDialog
{
    private readonly DownloadPageViewModel _pageVM;
    private readonly ObservableCollection<FolderItem> _folders = new();
    private readonly ObservableCollection<ModelItem> _models = new();
    private readonly ObservableCollection<FileItemViewModel> _files = new();
    private string _modelId;
    private ModelItem _modelVersion;
    private string _savePath;

    /// <summary>
    /// Initializes a new instance of the <see cref="CivitaiDownloadDialog"/> class.
    /// </summary>
    public CivitaiDownloadDialog(DownloadPageViewModel pageVM)
    {
        InitializeComponent();
        AppToolkit.ResetControlTheme(this);
        _pageVM = pageVM;
        var saveFolders = SettingsToolkit.ReadLocalSetting(SettingNames.CivitaiSaveFolders, "[]");
        var folders = JsonSerializer.Deserialize<List<FolderItem>>(saveFolders);
        foreach (var item in folders)
        {
            _folders.Add(item);
        }

        var lastFolder = SettingsToolkit.ReadLocalSetting(SettingNames.HuggingFaceLastSaveFolder, string.Empty);
        if (string.IsNullOrEmpty(lastFolder) || !_folders.Any(p => p.Path.Equals(lastFolder)))
        {
            lastFolder = _folders.FirstOrDefault()?.Path;
        }

        var selectedFolderItem = _folders.FirstOrDefault(p => p.Path.Equals(lastFolder));
        if (selectedFolderItem is not null)
        {
            SaveFolderBox.SelectedItem = selectedFolderItem;
        }

        var lastModelId = SettingsToolkit.ReadLocalSetting(SettingNames.CivitaiLastModelId, string.Empty);
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
            var token = SettingsToolkit.ReadLocalSetting(SettingNames.CivitaiToken, string.Empty);
            downloader.InitializeCivitai(token, _savePath);
            var models = await downloader.GetCivitaiModelAsync(_modelId);
            if (models is null || models.Count == 0)
            {
                AppViewModel.Instance.ShowTip(ResourceToolkit.GetLocalizedString(StringNames.NoAvailableDownloadItems), InfoType.Error);
            }
            else
            {
                foreach (var item in models)
                {
                    _models.Add(item);
                }

                VersionSelection.SelectedIndex = 0;
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
            if (string.IsNullOrEmpty(id))
            {
                AppViewModel.Instance.ShowTip(StringNames.InvalidModelId, InfoType.Error);
                return;
            }

            if (id.Contains('/'))
            {
                var split = id.Split('/');
                foreach (var s in split)
                {
                    if (long.TryParse(s, out var mid))
                    {
                        id = mid.ToString();
                        break;
                    }
                }
            }

            if (!long.TryParse(id, out _))
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
            SettingsToolkit.WriteLocalSetting(SettingNames.HuggingFaceLastSaveFolder, _savePath);
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

            SettingsToolkit.WriteLocalSetting(SettingNames.CivitaiLastModelId, _modelId);
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

    private void OnVersionSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _files.Clear();
        _modelVersion = default;
        if (VersionSelection.SelectedItem is not ModelItem model)
        {
            return;
        }

        _modelVersion = model;
        var items = _pageVM.GetDownloader().GetCivitaiModelDownloadItems(model);
        foreach (var item in items)
        {
            _files.Add(new FileItemViewModel(item));
        }

        CheckSelectAllContent();
    }
}
