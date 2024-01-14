using MessageStudio.Formats.BinaryText;
using NxEditor.EpdPlugin.Models.Common;
using NxEditor.PluginBase.Models;
using Revrs;

namespace NxEditor.EpdPlugin.ViewModels;

public class MsbtEditorViewModel : TextEditorBase
{
    public Endianness Endianness { get; set; } = Endianness.Little;
    public override string[] ExportExtensions { get; } = [
        "MSBT:*.msbt|",
        "Compressed:*.zs|"
    ];

    public MsbtEditorViewModel(IEditorFile handle) : base(handle)
    {
        View.GrammarId = "source.yaml";
        View.ReloadSyntaxHighlighting();
    }

    public override void Read()
    {
        Msbt msbt = Msbt.FromBinary(Handle.Source);
        Endianness = msbt.Endianness;
        View.TextEditor.Text = msbt.ToYaml();
    }

    public override Span<byte> Write()
    {
        Msbt msbt = Msbt.FromYaml(View.TextEditor.Text);
        return msbt.ToBinary(endianness: Endianness);
    }
}
