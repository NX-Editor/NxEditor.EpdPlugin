using Cead.Interop;
using NxEditor.CeadPlugin.Providers;
using NxEditor.Plugin.Core;

namespace NxEditor.CeadPlugin;

public class CeadPlugin : IServiceExtension
{
    public string Name { get; } = "C# EAD Plugin";

    public void RegisterExtension(IServiceLoader serviceManager)
    {
        DllManager.LoadCead();
        serviceManager.Register("TextEditor", new TextEditorProvider());
    }
}
