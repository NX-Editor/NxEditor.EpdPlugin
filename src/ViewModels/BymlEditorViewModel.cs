using BymlLibrary;
using NxEditor.EpdPlugin.Models.Common;
using NxEditor.PluginBase.Models;
using Revrs;

namespace NxEditor.EpdPlugin.ViewModels;

public class BymlEditorViewModel : TextEditorBase
{
    public Endianness Endianness { get; set; } = Endianness.Little;
    public int Version { get; set; } = 2;

    public override string[] ExportExtensions { get; } = [
        "General BYML:*.bgyml|",
        "BYML:*.byml|",
        "Compressed:*.zs|",
    ];

    public BymlEditorViewModel(IEditorFile handle) : base(handle)
    {
        View.GrammarId = "source.yaml";
        View.ReloadSyntaxHighlighting();
    }


    public override void Read()
    {
        RevrsReader reader = new(Handle.Source);
        ImmutableByml byml = new(ref reader);
        Endianness = byml.Endianness;
        Version = byml.Header.Version;
        View.TextEditor.Text = byml.ToYaml();
    }

    public override Span<byte> Write()
    {
        Byml byml = Byml.FromText(View.TextEditor.Text);
        return byml.ToBinary(Endianness, (ushort)Version);
    }
}
