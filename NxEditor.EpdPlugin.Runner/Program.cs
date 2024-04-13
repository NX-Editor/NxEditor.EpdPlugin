using Avalonia;
using NxEditor.EpdPlugin.Runner;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.FontAwesome;
using Projektanker.Icons.Avalonia.MaterialDesign;

namespace NxEditor;

internal class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        IconProvider.Current
            .Register(new FontAwesomeIconProvider())
            .Register(new MaterialDesignIconProvider());

        return AppBuilder.Configure<App>()
            .WithInterFont()
            .UsePlatformDetect();
    }
}
