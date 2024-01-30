using CommunityToolkit.Mvvm.ComponentModel;
using ConfigFactory.Core.Attributes;
using ConfigFactory.Core;
using MessageStudio.Formats.BinaryText.Components;
using NxEditor.TotkPlugin.Models.MessageStudio.BinaryText;
using BymlLibrary;

namespace NxEditor.EpdPlugin;

public partial class EpdConfig : ConfigModule<EpdConfig>
{
    [ObservableProperty]
    [property: NumericConfig(Increment = 1)]
    [property: Config(
        Header = "BYML Inline Container Max Count",
        Description = "The max amount of child nodes allowed to still write an inline container.",
        Category = "Editor Config",
        Group = "BYML")]
    private int _bymlInlineContainerMaxCount = 8;

    [ObservableProperty]
    [property: Config(
        Header = "Tag Resolver",
        Description = "The tag resolver to use when decoding MSBT tags/functions.",
        Category = "Editor Config",
        Group = "MSBT")]
    [property: DropdownConfig("None", "Module System")]
    private string _tagResolverName = "None";

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

    partial void OnTagResolverNameChanged(string value)
    {
        switch (value) {
            case "Module System":
                TagResolver.Load<ModuleSystemTagResolver>();
                break;
            case "None":
                TagResolver.Load<DefaultTagResolver>();
                break;
        };
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
