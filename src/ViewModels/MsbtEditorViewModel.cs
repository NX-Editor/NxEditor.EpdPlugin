using CommunityToolkit.Mvvm.ComponentModel;
using MessageStudio.Common;
using MessageStudio.Formats.BinaryText;
using NxEditor.EpdPlugin.Models.Common;
using NxEditor.EpdPlugin.Models.Msbt;
using NxEditor.PluginBase.Models;
using Revrs;

namespace NxEditor.EpdPlugin.ViewModels;

public partial class MsbtEditorViewModel : TextEditorBase
{
    [ObservableProperty]
    private Endianness _endianness;

    [ObservableProperty]
    private TextEncoding _encoding;

    public override string[] ExportExtensions { get; } = [
        "MSBT:*.msbt|",
        "Compressed:*.zs|"
    ];

    public override object? FooterModel { get; protected set; }

    public MsbtEditorViewModel(IEditorFile handle) : base(handle)
    {
        View.GrammarId = "source.yaml";
        View.ReloadSyntaxHighlighting();
        FooterModel = new MsbtPropertiesFooter(this);
    }

    public override void Read()
    {
        Msbt msbt = Msbt.FromBinary(Handle.Source);
        Endianness = msbt.Endianness;
        Encoding = msbt.Encoding;
        View.TextEditor.Text = msbt.ToYaml();
    }

    public override Span<byte> Write()
    {
        Msbt msbt = Msbt.FromYaml(View.TextEditor.Text);
        return msbt.ToBinary(endianness: Endianness, encoding: Encoding);
    }
}
