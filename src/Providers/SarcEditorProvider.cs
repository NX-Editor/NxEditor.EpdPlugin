using NxEditor.EpdPlugin.ViewModels;
using NxEditor.PluginBase.Models;
using NxEditor.PluginBase.Services;

namespace NxEditor.EpdPlugin.Providers;

public class SarcEditorProvider : IFormatServiceProvider
{
    public IFormatService GetService(IEditorFile handle)
    {
        return new SarcEditorViewModel(handle);
    }

    public bool IsValid(IEditorFile handle)
    {
        return handle.Source.Length >= 4
            && handle.Source.AsSpan()[0..4].SequenceEqual("SARC"u8);
    }
}
