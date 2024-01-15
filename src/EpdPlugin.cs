using NxEditor.EpdPlugin.Processors;
using NxEditor.EpdPlugin.Providers;
using NxEditor.PluginBase;

namespace NxEditor.EpdPlugin;

public class EpdPlugin : IServiceExtension
{
    public string Name { get; } = "EPD Plugin";

    public void RegisterExtension(IServiceLoader serviceManager)
    {
        serviceManager
            .Register(new Yaz0Processor())
            .Register("TextEditor", new TextEditorProvider())
            .Register("BymlEditor", new BymlEditorProvider())
            .Register("MsbtEditor", new MsbtEditorProvider())
            .Register("SarcEditor", new SarcEditorProvider())
            .Register("RestblEditor", new RestblEditorProvider())
            .Register("BfevflEditor", new BfevflEditorProvider());
    }
}
