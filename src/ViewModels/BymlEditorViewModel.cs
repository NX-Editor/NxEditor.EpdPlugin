using BymlLibrary;
using NxEditor.EpdPlugin.Models.Common;
using NxEditor.PluginBase.Models;
using Revrs;

namespace NxEditor.EpdPlugin.ViewModels;

public class BymlEditorViewModel : TextEditorBase
{
    public Endianness Endianness { get; set; } = Endianness.Little;

    public BymlEditorViewModel(IFileHandle handle) : base(handle)
    {
        Handle = handle;
        ExportExtensions = [
            "General BYML:*.bgyml|",
            "BYML:*.byml|",
            "Compressed:*.zs|",
        ];

        View.GrammarId = "source.yaml";
        View.ReloadSyntaxHighlighting();
    }

    public override string[] ExportExtensions { get; }

    public override Task Read()
    {
        RevrsReader reader = new(Handle.Data);
        ImmutableByml byml = new(ref reader);
        Endianness = byml.Endianness;
        View.TextEditor.Text = byml.ToYaml();

        return Task.CompletedTask;
    }

    public override Task<IFileHandle> Write()
    {
        Byml byml = Byml.FromText(View.TextEditor.Text);
        Handle.Data = byml.ToBinary(Endianness);
        return Task.FromResult(Handle);
    }
}
