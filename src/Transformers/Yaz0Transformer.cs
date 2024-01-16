using CsYaz0;
using NxEditor.PluginBase.Models;
using NxEditor.PluginBase.Services;

namespace NxEditor.EpdPlugin.Transformers;

public class Yaz0Transformer : ITransformer
{
    public bool IsValid(IEditorFile handle)
    {
        return handle.Source.Length >= 4
            && handle.Source.AsSpan()[0..4].SequenceEqual("Yaz0"u8);
    }

    public void TransformSource(IEditorFile handle)
    {
        handle.Source = Yaz0.Decompress(handle.Source);
    }

    public void Transform(ref Span<byte> data, IEditorFile handle)
    {
        data = Yaz0.Compress(data, level: int.TryParse(EpdConfig.Shared.Yaz0CompressionLevel, out int level) ? 7 : level);
    }
}
