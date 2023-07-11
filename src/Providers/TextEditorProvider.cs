using NxEditor.EpdPlugin.ViewModels;
using NxEditor.PluginBase.Models;
using NxEditor.PluginBase.Services;

namespace NxEditor.EpdPlugin.Providers;

public class TextEditorProvider : IFormatServiceProvider
{
    public IFormatService GetService(IFileHandle handle)
    {
        return new TextEditorViewModel(handle);
    }

    public bool IsValid(IFileHandle handle)
    {
        return Array.IndexOf<byte>(handle.Data, 0x0) == -1;
    }
}
