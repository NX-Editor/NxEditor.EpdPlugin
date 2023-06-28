using NxEditor.CeadPlugin.ViewModels;
using NxEditor.PluginBase.Core.Models;
using NxEditor.PluginBase.Core.Services;

namespace NxEditor.CeadPlugin.Providers;

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
