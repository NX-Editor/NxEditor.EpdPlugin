using Cead.Interop;
using NxEditor.EpdPlugin.Providers;
using NxEditor.PluginBase;

namespace NxEditor.EpdPlugin;

public class EpdPlugin : IServiceExtension
{
    public string Name { get; } = "EPD Plugin";

    public void RegisterExtension(IServiceLoader serviceManager)
    {
        DllManager.LoadCead();

        serviceManager.Register("TextEditor", new TextEditorProvider());
    }
}
