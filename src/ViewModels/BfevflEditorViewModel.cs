using BfevLibrary;
using NxEditor.EpdPlugin.Models.Common;
using NxEditor.PluginBase.Models;

namespace NxEditor.EpdPlugin.ViewModels;

public class BfevflEditorViewModel : TextEditorBase
{
    public override string[] ExportExtensions { get; } = [
        "BFEVFL:*.bfevfl|",
    ];

    public BfevflEditorViewModel(IEditorFile handle) : base(handle)
    {
        View.GrammarId = "source.json";
        View.ReloadSyntaxHighlighting();
    }

    public override void Read()
    {
        BfevFile bfev = BfevFile.FromBinary(Handle.Source);
        View.TextEditor.Text = bfev.ToJson(format: true);
    }

    public override Span<byte> Write()
    {
        BfevFile bfev = BfevFile.FromJson(View.TextEditor.Text);
        return bfev.ToBinary();
    }
}
