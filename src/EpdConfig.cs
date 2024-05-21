using BymlLibrary;
using CommunityToolkit.Mvvm.ComponentModel;
using ConfigFactory.Core;
using ConfigFactory.Core.Attributes;
using MessageStudio.Formats.BinaryText.Components;

namespace NxEditor.EpdPlugin;

public partial class EpdConfig : ConfigModule<EpdConfig>
{
    [ObservableProperty]
    [property: NumericConfig(Increment = 1, Minimum = 0, Maximum = ulong.MaxValue)]
    [property: Config(
        Header = "BYML Inline Container Max Count",
        Description = "The max amount of child nodes allowed to still write an inline container.",
        Category = "Editor Config",
        Group = "BYML")]
    private int _bymlInlineContainerMaxCount = 8;

    [ObservableProperty]
    [property: Config(
        Header = "Default Function Map",
        Description = "The default function map to use when reading MSBT files.",
        Category = "Editor Config",
        Group = "MSBT")]
    [property: BrowserConfig(
        BrowserMode = BrowserMode.OpenFile,
        Title = "MSBT Function Map File",
        Filter = "YAML:*.yaml;*.yml",
        InstanceBrowserKey = "epdplugin-config-msbt-function-map")]
    private string _functionMapFile = string.Empty;

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
        Header = "Autosave Parent SARC",
        Description = "Automatically save the parent sarc archive ",
        Category = "Editor Config",
        Group = "SARC")]
    private bool _autosaveParentSarc = false;

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
        Validate(() => RestblStrings,
            value => string.IsNullOrEmpty(value) || File.Exists(value));
    }

    partial void OnFunctionMapFileChanged(string value)
    {
        if (File.Exists(value)) {
            FunctionMap.Current = FunctionMap.FromFile(value);
            return;
        }

        FunctionMap.Current = FunctionMap.Default;
    }

    partial void OnBymlInlineContainerMaxCountChanged(int value)
    {
        Byml.YamlConfig.InlineContainerMaxCount = value + 1;
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
