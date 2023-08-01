using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using AvaloniaEdit.TextMate;
using TextMateSharp.Grammars;

namespace NxEditor.EpdPlugin.Views;

public partial class RestblEditorView : UserControl
{
    public static RegistryOptions RegistryOptions { get; set; } = new(Application.Current?.RequestedThemeVariant == ThemeVariant.Dark
        ? ThemeName.DarkPlus : ThemeName.LightPlus);

    static RestblEditorView()
    {
        Application.Current!.PropertyChanged += (s, e) => {
            if (e.Property == Application.RequestedThemeVariantProperty) {
                RegistryOptions = new((ThemeVariant)e.NewValue! == ThemeVariant.Dark
                    ? ThemeName.DarkPlus : ThemeName.LightPlus);
            }
        };
    }

    public RestblEditorView()
    {
        InitializeComponent();

        var textEditorInstallation = TextEditor.InstallTextMate(RegistryOptions);
        textEditorInstallation.SetGrammar("source.diff");

        var stringsEditorInstallation = StringsEditor.InstallTextMate(RegistryOptions);
        stringsEditorInstallation.SetGrammar("source.yaml");
    }
}