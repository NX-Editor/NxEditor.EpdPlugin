using NxEditor.EpdPlugin.Models.Common;
using NxEditor.PluginBase.Models;
using System.Text;

namespace NxEditor.EpdPlugin.ViewModels;

public class TextEditorViewModel(IEditorFile handle) : TextEditorBase(handle)
{
    public virtual Encoding Encoding { get; } = Encoding.UTF8;
    public override string[] ExportExtensions { get; } = [
        $"Default:*{Path.GetExtension(handle.Name)}|"
    ];

    public override void Read()
    {
        View.SetGrammarFromFile(Handle.Name);
        View.ReloadSyntaxHighlighting();
        View.TextEditor.Text = Encoding.GetString(Handle.Source);
    }

    public override Span<byte> Write()
    {
        return Encoding.GetBytes(View.TextEditor.Text);
    }
}
