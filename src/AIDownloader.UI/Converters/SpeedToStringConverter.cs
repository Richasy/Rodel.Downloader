// Copyright (c) AI Downloader. All rights reserved.

using NeoSmart.PrettySize;

namespace AIDownloader.UI.Converters;

internal sealed class SpeedToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var bytes = System.Convert.ToInt64(value);
        var num = new PrettySize(bytes);
        return $"{num}/s";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
