using CsMsbt;
using CsOead;
using CsRestbl;
using Native.IO.Services;
using NxEditor.EpdPlugin.Processors;
using NxEditor.EpdPlugin.Providers;
using NxEditor.EpdPlugin.Providers.UI;
using NxEditor.PluginBase;
using Projektanker.Icons.Avalonia;

namespace NxEditor.EpdPlugin;

public class EpdPlugin : IServiceExtension
{
    public string Name { get; } = "EPD Plugin";

    public void RegisterExtension(IServiceLoader serviceManager)
    {
        NativeLibraryManager.RegisterAssembly(typeof(EpdPlugin).Assembly, out bool isCommonLoaded)
            .Register(new OeadLibrary(), out bool isOeadLoaded)
            .Register(new MsbtLibrary(), out bool isMsbtLoaded)
            .Register(new RestblLibrary(), out bool isRestblLoaded);

        Console.WriteLine($"Loaded native_io: {isCommonLoaded}");
        Console.WriteLine($"Loaded cs_oead: {isOeadLoaded}");
        Console.WriteLine($"Loaded cs_msbt: {isMsbtLoaded}");
        Console.WriteLine($"Loaded cs_restbl: {isRestblLoaded}");

        IconProvider.Current.Register(new MaterialSymbolIconProvider());

        serviceManager
            .Register(new Yaz0Processor())
            .Register("TextEditor", new TextEditorProvider())
            .Register("BymlEditor", new BymlEditorProvider())
            .Register("MsbtEditor", new MsbtEditorProvider())
            .Register("SarcEditor", new SarcEditorProvider())
            .Register("RestblEditor", new RestblEditorProvider());
    }
}
