using BfevLibrary;
using NxEditor.EpdPlugin.ViewModels;
using NxEditor.PluginBase.Models;
using NxEditor.PluginBase.Services;

namespace NxEditor.EpdPlugin.Providers;

public class BfevflEditorProvider : IFormatServiceProvider
{
    public IFormatService GetService(IFileHandle handle)
    {
        BfevFile f = BfevFile.FromBinary(handle.Data);
        Console.WriteLine(f.ToJson());

        return new BfevflEditorViewModel(handle);
    }

    public bool IsValid(IFileHandle handle)
    {
        return handle.Data.Length >= 6
            && handle.Data.AsSpan()[0..6].SequenceEqual("BFEVFL"u8);
    }
}
