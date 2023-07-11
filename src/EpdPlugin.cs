using Cead.Interop;
using NxEditor.CeadPlugin.Providers;
using NxEditor.PluginBase;

namespace NxEditor.CeadPlugin;

public class EpdPlugin : IServiceExtension
{
    public string Name { get; } = "EPD Plugin";

    public void RegisterExtension(IServiceLoader serviceManager)
    {
        DllManager.LoadCead();

        serviceManager.Register("TextEditor", new TextEditorProvider());
    }
}
