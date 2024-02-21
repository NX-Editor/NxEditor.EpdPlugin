using NxEditor.EpdPlugin.Providers;
using NxEditor.EpdPlugin.Transformers;
using NxEditor.PluginBase;

namespace NxEditor.EpdPlugin;

public class EpdPlugin : IServiceExtension
{
    public string Name { get; } = "EPD Plugin";
    public string Version { get; } = typeof(EpdPlugin).Assembly.GetName().Version?.ToString(3) ?? "1.0.0";

    public void RegisterExtension(IServiceLoader serviceManager)
    {
        serviceManager
            .Register(new Yaz0Transformer())
            .Register("TextEditor", new TextEditorProvider())
            .Register("BymlEditor", new BymlEditorProvider())
            .Register("MsbtEditor", new MsbtEditorProvider())
            .Register("SarcEditor", new SarcEditorProvider())
            .Register("RestblEditor", new RestblEditorProvider())
            .Register("BfevflEditor", new BfevflEditorProvider());
    }
}
