using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using AvaloniaEdit.TextMate;
using TextMateSharp.Grammars;

namespace NxEditor.EpdPlugin.Views;

public partial class TextEditorView : UserControl
{
    public static RegistryOptions RegistryOptions { get; set; } = new(Application.Current?.RequestedThemeVariant == ThemeVariant.Dark
        ? ThemeName.DarkPlus : ThemeName.LightPlus);

    static TextEditorView()
    {
        Application.Current!.PropertyChanged += (s, e) => {
            if (e.Property == Application.RequestedThemeVariantProperty) {
                RegistryOptions = new((ThemeVariant)e.NewValue! == ThemeVariant.Dark
                    ? ThemeName.DarkPlus : ThemeName.LightPlus);
            }
        };
    }

    public TextMate.Installation TextMateInstallation { get; }
    public string GrammarId { get; set; } = string.Empty;

    public TextEditorView()
    {
        InitializeComponent();
        TextMateInstallation = TextEditor.InstallTextMate(RegistryOptions);
    }

    public void ReloadSyntaxHighlighting()
    {
        TextMateInstallation.SetGrammar(GrammarId);
    }

    public void SetGrammarFromFile(string path)
    {
        if (RegistryOptions.GetLanguageByExtension(Path.GetExtension(path))?.Id is string id) {
            GrammarId = RegistryOptions.GetScopeByLanguageId(id);
        }
    }
}
