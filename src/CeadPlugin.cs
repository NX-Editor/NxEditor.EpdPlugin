using Cead.Interop;
using NxEditor.CeadPlugin.Providers;
using NxEditor.PluginBase;

namespace NxEditor.CeadPlugin;

public class CeadPlugin : ServiceExtension<CeadPlugin>
{
    public override string Name { get; } = "C# EAD Plugin";

    public override void RegisterExtension(IServiceLoader serviceManager)
    {
        DllManager.LoadCead();

        serviceManager.Register("TextEditor", new TextEditorProvider());
    }
}
