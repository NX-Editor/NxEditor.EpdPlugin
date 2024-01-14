using NxEditor.EpdPlugin.ViewModels;
using NxEditor.PluginBase.Models;
using NxEditor.PluginBase.Services;
using System.Buffers;

namespace NxEditor.EpdPlugin.Providers;

public class TextEditorProvider : IFormatServiceProvider
{
    private static readonly SearchValues<byte> _nullptrs = SearchValues.Create("\0"u8);

    public IFormatService GetService(IEditorFile handle)
    {
        return new TextEditorViewModel(handle);
    }

    public bool IsValid(IEditorFile handle)
    {
        return !handle.Source.AsSpan()
            .ContainsAny(_nullptrs);
    }
}
