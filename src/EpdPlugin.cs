using CsMsbt;
using CsOead;
using Native.IO.Services;
using NxEditor.EpdPlugin.Processors;
using NxEditor.EpdPlugin.Providers;
using NxEditor.PluginBase;

namespace NxEditor.EpdPlugin;

public class EpdPlugin : IServiceExtension
{
    public string Name { get; } = "EPD Plugin";

    public void RegisterExtension(IServiceLoader serviceManager)
    {
        NativeLibraryManager.RegisterAssembly(typeof(EpdPlugin).Assembly, out bool isCommonLoaded)
            .Register(new OeadLibrary(), out bool isOeadLoaded)
            .Register(new MsbtLibrary(), out bool isMsbtLoaded);

        Console.WriteLine($"Loaded native_io: {isCommonLoaded}");
        Console.WriteLine($"Loaded cs_oead: {isOeadLoaded}");
        Console.WriteLine($"Loaded cs_msbt: {isMsbtLoaded}");

        serviceManager
            .Register(new Yaz0Processor())
            .Register("TextEditor", new TextEditorProvider())
            .Register("BymlEditor", new BymlEditorProvider())
            .Register("MsbtEditor", new MsbtEditorProvider())
            .Register("SarcEditor", new SarcEditorProvider());
    }
}
