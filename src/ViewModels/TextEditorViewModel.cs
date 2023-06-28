using NxEditor.CeadPlugin.Views;
using NxEditor.PluginBase.Component;
using NxEditor.PluginBase.Models;
using System.Text;

namespace NxEditor.CeadPlugin.ViewModels;

public class TextEditorViewModel : Editor<TextEditorViewModel, TextEditorView>
{
    public virtual Encoding Encoding { get; } = Encoding.UTF8;
    public override string[] ExportExtensions { get; }

    public TextEditorViewModel(IFileHandle handle) : base(handle)
    {
        ExportExtensions = new string[] {
            $"Default:*{Path.GetExtension(handle.Path ?? handle.Name)}|"
        };
    }

    public override Task Read()
    {
        View.TextEditor.Text = Encoding.GetString(Handle.Data);
        return Task.CompletedTask;
    }

    public override Task<IFileHandle> Write()
    {
        FileHandle handle = new(Encoding.GetBytes(View.TextEditor.Text));
        return Task.FromResult((IFileHandle)handle);
    }
}
