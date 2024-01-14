using NxEditor.EpdPlugin.ViewModels;
using NxEditor.PluginBase.Models;
using NxEditor.PluginBase.Services;

namespace NxEditor.EpdPlugin.Providers;

public class BfevflEditorProvider : IFormatServiceProvider
{
    public IFormatService GetService(IEditorFile handle)
    {
        return new BfevflEditorViewModel(handle);
    }

    public bool IsValid(IEditorFile handle)
    {
        return handle.Source.Length >= 6
            && handle.Source.AsSpan()[0..6].SequenceEqual("BFEVFL"u8);
    }
}
