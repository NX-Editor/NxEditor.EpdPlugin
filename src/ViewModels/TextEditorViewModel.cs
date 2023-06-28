using NxEditor.CeadPlugin.Views;
using NxEditor.Plugin.Component;
using NxEditor.Plugin.Core.Models;
using NxEditor.Plugin.Core.Services;
using System.Text;

namespace NxEditor.CeadPlugin.ViewModels;

public class TextEditorViewModel : Editor<TextEditorViewModel, TextEditorView>
{
    public virtual Encoding Encoding { get; } = Encoding.UTF8;

    public TextEditorViewModel(IFileHandle handle) : base(handle) { }

    public override Task Read(IFileHandle handle)
    {
        View.TextEditor.Text = Encoding.GetString(handle.Data);
        return Task.CompletedTask;
    }

    public override Task<IFileHandle> Write()
    {
        FileHandle handle = new(Encoding.GetBytes(View.TextEditor.Text));
        return Task.FromResult((IFileHandle)handle);
    }
}
