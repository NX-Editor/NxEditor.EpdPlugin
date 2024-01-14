using NxEditor.EpdPlugin.ViewModels;
using NxEditor.PluginBase.Models;
using NxEditor.PluginBase.Services;

namespace NxEditor.EpdPlugin.Providers;

public class MsbtEditorProvider : IFormatServiceProvider
{
    public IFormatService GetService(IEditorFile handle)
    {
        return new MsbtEditorViewModel(handle);
    }

    public bool IsValid(IEditorFile handle)
    {
        return handle.Source.Length >= 8
            && handle.Source.AsSpan()[0..8].SequenceEqual("MsgStdBn"u8);
    }
}
