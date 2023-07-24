using CsOead;
using Native.IO.Services;
using NxEditor.EpdPlugin.Providers;
using NxEditor.PluginBase;

namespace NxEditor.EpdPlugin;

public class EpdPlugin : IServiceExtension
{
    public string Name { get; } = "EPD Plugin";

    public void RegisterExtension(IServiceLoader serviceManager)
    {
        NativeLibraryManager.RegisterAssembly(typeof(EpdPlugin).Assembly, out bool isCommonLoaded)
            .Register(new OeadLibrary(), out bool isOeadLoaded);

        Console.WriteLine($"Loaded native_io: {isCommonLoaded}");
        Console.WriteLine($"Loaded cs_oead: {isOeadLoaded}");

        serviceManager
            .Register("TextEditor", new TextEditorProvider())
            .Register("BymlEditor", new BymlEditorProvider());
    }
}
