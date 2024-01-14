using NxEditor.EpdPlugin.ViewModels;
using NxEditor.PluginBase.Models;
using NxEditor.PluginBase.Services;

namespace NxEditor.EpdPlugin.Providers;

public class RestblEditorProvider : IFormatServiceProvider
{
    public IFormatService GetService(IEditorFile handle)
    {
        return new RestblEditorViewModel(handle);
    }

    public bool IsValid(IEditorFile handle)
    {
        return handle.Source.AsSpan()[0..6].SequenceEqual("RESTBL"u8);
    }
}
