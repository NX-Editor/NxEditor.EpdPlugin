using NxEditor.PluginBase.Models;
using NxEditor.PluginBase.Services;
using Yaz0Library;

namespace NxEditor.EpdPlugin.Processors;

public class Yaz0Processor : IProcessingService
{
    public bool IsValid(IEditorFile handle)
    {
        return handle.Source.Length >= 4
            && handle.Source.AsSpan()[0..4].SequenceEqual("Yaz0"u8);
    }

    public void Process(IEditorFile handle)
    {
        handle.Source = Yaz0.Decompress(handle.Source).ToArray();
    }

    public void Reprocess(IEditorFile handle)
    {
        WriteEditorFile baseWrite = handle.Write;
        handle.Write = (data) => {
            data = Yaz0.Compress(data, out Yaz0SafeHandle handle, int.TryParse(EpdConfig.Shared.Yaz0CompressionLevel, out int level) ? 7 : level);
            baseWrite(data);
        };
    }
}
