using NxEditor.EpdPlugin.Models.Common;
using NxEditor.PluginBase.Models;
using System.Text;

namespace NxEditor.EpdPlugin.ViewModels;

public class TextEditorViewModel(IFileHandle handle) : TextEditorBase(handle)
{
    public virtual Encoding Encoding { get; } = Encoding.UTF8;
    public override string[] ExportExtensions { get; } = [
        $"Default:*{Path.GetExtension(handle.FilePath ?? handle.Name)}|"
    ];

    public override Task Read()
    {
        View.SetGrammarFromFile(Handle.FilePath ?? Handle.Name);
        View.ReloadSyntaxHighlighting();
        View.TextEditor.Text = Encoding.GetString(Handle.Data);

        return Task.CompletedTask;
    }

    public override Task<IFileHandle> Write()
    {
        Handle.Data = Encoding.GetBytes(View.TextEditor.Text);
        return Task.FromResult(Handle);
    }
}
