using NxEditor.EpdPlugin.ViewModels;
using NxEditor.PluginBase.Models;
using NxEditor.PluginBase.Services;

namespace NxEditor.EpdPlugin.Providers;

public class RestblEditorProvider : IFormatServiceProvider
{
    public IFormatService GetService(IFileHandle handle)
    {
        return new RestblEditorViewModel(handle);
    }

    public bool IsValid(IFileHandle handle)
    {
        return handle.Data.AsSpan()[0..6].SequenceEqual("RESTBL"u8);
    }
}
