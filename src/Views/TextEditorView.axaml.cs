using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using AvaloniaEdit.TextMate;
using NxEditor.EpdPlugin.ViewModels;
using TextMateSharp.Grammars;

namespace NxEditor.EpdPlugin.Views;

public partial class TextEditorView : UserControl
{
    private static RegistryOptions _registryOptions = new(ThemeName.DarkPlus);

    public string GrammarId { get; set; } = string.Empty;

    public TextEditorView()
    {
        InitializeComponent();

        Application.Current!.PropertyChanged += (s, e) => {
            if (e.Property == Application.RequestedThemeVariantProperty) {
                _registryOptions = new((ThemeVariant)e.NewValue! == ThemeVariant.Dark
                    ? ThemeName.DarkPlus : ThemeName.LightPlus);
                TrySetGrammarId();
                ReloadSyntaxHighlighting();
            }
        };
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        TrySetGrammarId();
        ReloadSyntaxHighlighting();
    }

    public void ReloadSyntaxHighlighting()
    {
        TextMate.Installation textMateInstallation = TextEditor.InstallTextMate(_registryOptions);
        textMateInstallation.SetGrammar(GrammarId);
    }

    private void TrySetGrammarId()
    {
        if (DataContext is TextEditorViewModel textEditorViewModel) {
            if (_registryOptions.GetLanguageByExtension(Path.GetExtension(textEditorViewModel.Handle.Path))?.Id is string id) {
                GrammarId = _registryOptions.GetScopeByLanguageId(id);
            }
        }
    }
}
