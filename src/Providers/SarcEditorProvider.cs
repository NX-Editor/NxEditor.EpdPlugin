using NxEditor.EpdPlugin.ViewModels;
using NxEditor.PluginBase.Models;
using NxEditor.PluginBase.Services;

namespace NxEditor.EpdPlugin.Providers;

public class SarcEditorProvider : IFormatServiceProvider
{
    public IFormatService GetService(IFileHandle handle)
    {
        return new SarcEditorViewModel(handle);
    }

    public bool IsValid(IFileHandle handle)
    {
        return handle.Data.Length >= 4
            && handle.Data.AsSpan()[0..4].SequenceEqual("SARC"u8);
    }
}
