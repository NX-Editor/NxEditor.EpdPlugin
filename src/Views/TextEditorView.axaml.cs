using Avalonia.Controls;
using AvaloniaEdit.TextMate;
using NxEditor.CeadPlugin.ViewModels;
using TextMateSharp.Grammars;

namespace NxEditor.CeadPlugin.Views;

public partial class TextEditorView : UserControl
{
    private static readonly RegistryOptions _registryOptions = new(ThemeName.DarkPlus);
    private static string _grammerId = string.Empty;

    public TextEditorView()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (DataContext is TextEditorViewModel textEditorViewModel) {
            if (_registryOptions.GetLanguageByExtension(Path.GetExtension(textEditorViewModel.Handle.Path))?.Id is string id) {
                _grammerId = _registryOptions.GetScopeByLanguageId(id);
                TextMate.Installation textMateInstallation = TextEditor.InstallTextMate(_registryOptions);
                textMateInstallation.SetGrammar(_grammerId);
            }
        }
    }
}
