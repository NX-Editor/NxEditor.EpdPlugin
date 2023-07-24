using NxEditor.EpdPlugin.ViewModels;
using NxEditor.PluginBase.Models;
using NxEditor.PluginBase.Services;

namespace NxEditor.EpdPlugin.Providers;

public class BymlEditorProvider : IFormatServiceProvider
{
    public IFormatService GetService(IFileHandle handle)
    {
        return new BymlEditorViewModel(handle);
    }

    public bool IsValid(IFileHandle handle)
    {
        Span<byte> magic = handle.Data.AsSpan()[0..2];
        return magic.SequenceEqual("BY"u8) || magic.SequenceEqual("YB"u8);
    }
}
