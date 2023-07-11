using CommunityToolkit.Mvvm.ComponentModel;
using ConfigFactory.Core;
using ConfigFactory.Core.Attributes;

namespace NxEditor.CeadPlugin;

public partial class CeadConfig : ConfigModule<CeadConfig>
{
    [ObservableProperty]
    [property: Config(Header = "RESTBL Strings", Description = "", Category = "EAD Plugin")]
    private string _restblStrings = string.Empty;

    partial void OnRestblStringsChanged(string value)
    {
        SetValidation(() => RestblStrings,
            value => !string.IsNullOrEmpty(value));
    }
}
