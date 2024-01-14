using NxEditor.EpdPlugin.ViewModels;
using NxEditor.PluginBase.Models;
using NxEditor.PluginBase.Services;

namespace NxEditor.EpdPlugin.Providers;

public class BymlEditorProvider : IFormatServiceProvider
{
    public IFormatService GetService(IEditorFile handle)
    {
        return new BymlEditorViewModel(handle);
    }

    public bool IsValid(IEditorFile handle)
    {
        if (handle.Source.Length < 2) {
            return false;
        }

        Span<byte> magic = handle.Source.AsSpan()[0..2];
        return magic.SequenceEqual("BY"u8) || magic.SequenceEqual("YB"u8);
    }
}
