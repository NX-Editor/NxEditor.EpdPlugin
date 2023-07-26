using CsMsbt;
using CsOead;
using NxEditor.EpdPlugin.Views;
using NxEditor.PluginBase.Components;
using NxEditor.PluginBase.Models;

namespace NxEditor.EpdPlugin.ViewModels;

public class MsbtEditorViewModel : Editor<MsbtEditorViewModel, TextEditorView>
{
    public MsbtEditorViewModel(IFileHandle handle) : base(handle)
    {
        Handle = handle;
        ExportExtensions = new string[] {
            "MSBT:*.msbt|",
            "Compressed:*.zs|"
        };

        View.GrammarId = "source.yaml";
        View.ReloadSyntaxHighlighting();
    }

    public override string[] ExportExtensions { get; }

    public override Task Read()
    {
        Msbt msbt = Msbt.FromBinary(Handle.Data);
        View.TextEditor.Text = msbt.ToText().ToString();

        return Task.CompletedTask;
    }

    public override Task<IFileHandle> Write()
    {
        throw new NotImplementedException();
    }

    public override void Cleanup()
    {
        View.TextMateInstallation.Dispose();
        base.Cleanup();
    }
}
