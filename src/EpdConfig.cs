using CommunityToolkit.Mvvm.ComponentModel;
using ConfigFactory.Core.Attributes;
using ConfigFactory.Core;

namespace NxEditor.EpdPlugin;

public partial class EpdConfig : ConfigModule<EpdConfig>
{
    [ObservableProperty]
    [property: Config(
        Header = "RESTBL Strings",
        Description = "The absolute path to a RESTBL strings file",
        Category = "Editor Config",
        Group = "RESTBL")]
    [property: BrowserConfig(
        BrowserMode = BrowserMode.OpenFile,
        Title = "RESTBL Strings File",
        InstanceBrowserKey = "epdplugin-config-restble-strings")]
    private string _restblStrings = string.Empty;

    [ObservableProperty]
    [property: Config(
        Header = "Yaz0 Compression Level",
        Description = "Compression level used when compressing with Yaz0",
        Category = "Editor Config",
        Group = "Yaz0")]
    [property: DropdownConfig(
        RuntimeItemsSourceMethodName = "GetCompressionLevels")]
    private string _yaz0CompressionLevel = "7";

    partial void OnRestblStringsChanged(string value)
    {
        SetValidation(() => RestblStrings,
            value => File.Exists(value));
    }

    public static string[] GetCompressionLevels()
    {
        string[] result = new string[9];
        for (int i = 0; i < 9; i++) {
            result[i] = (i + 1).ToString();
        }

        return result;
    }
}
