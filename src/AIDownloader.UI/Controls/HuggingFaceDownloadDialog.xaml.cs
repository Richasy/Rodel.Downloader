// Copyright (c) AI Downloader. All rights reserved.

using AIDownloader.Core;
using AIDownloader.UI.Models.Constants;
using AIDownloader.UI.Toolkits;
using AIDownloader.UI.ViewModels;

namespace AIDownloader.UI.Controls;

/// <summary>
/// HuggingFace 下载对话框.
/// </summary>
public sealed partial class HuggingFaceDownloadDialog : ContentDialog
{
    private string _modelId;
    private HuggingFaceUriType _uriType;

    /// <summary>
    /// Initializes a new instance of the <see cref="HuggingFaceDownloadDialog"/> class.
    /// </summary>
    public HuggingFaceDownloadDialog()
    {
        InitializeComponent();
        var type = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.HuggingFaceUriType, HuggingFaceUriType.Official);
        TypeSelector.SelectedIndex = (int)type;
    }

    private void OnPrimaryButtonClickAsync(ContentDialog sender, ContentDialogButtonClickEventArgs args)
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
        }
    }

    private void OnTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (TypeSelector.SelectedIndex == -1)
        {
            return;
        }

        _uriType = (HuggingFaceUriType)TypeSelector.SelectedIndex;
    }
}
