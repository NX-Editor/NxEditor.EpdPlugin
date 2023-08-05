using CsOead;
using NxEditor.EpdPlugin.Models.Common;
using NxEditor.PluginBase.Models;

namespace NxEditor.EpdPlugin.ViewModels;

public class BymlEditorViewModel : TextEditorBase
{
    public BymlEditorViewModel(IFileHandle handle) : base(handle)
    {
        Handle = handle;
        ExportExtensions = new string[] {
            "General BYML:*.bgyml|",
            "BYML:*.byml|",
            "Compressed:*.zs|",
        };

        View.GrammarId = "source.yaml";
        View.ReloadSyntaxHighlighting();
    }

    public override string[] ExportExtensions { get; }

    public override Task Read()
    {
        Byml byml = Byml.FromBinary(Handle.Data);
        View.TextEditor.Text = byml.ToText().ToString();

        return Task.CompletedTask;
    }

    public override Task<IFileHandle> Write()
    {
        Byml byml = Byml.FromText(View.TextEditor.Text);
        Handle.Data = byml.ToBinary().ToArray();
        return Task.FromResult(Handle);
    }
}
