using CommunityToolkit.Mvvm.ComponentModel;
using ConfigFactory.Core;
using ConfigFactory.Core.Attributes;

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

    partial void OnRestblStringsChanged(string value)
    {
        SetValidation(() => RestblStrings,
            value => File.Exists(value));
    }
}
