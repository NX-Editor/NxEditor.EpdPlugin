using NxEditor.EpdPlugin.ViewModels;
using NxEditor.PluginBase.Attributes;

namespace NxEditor.EpdPlugin.Models.Rstb;

public class RestblActionsMenu
{
    private readonly RestblEditorViewModel _restbl;

    public RestblActionsMenu(RestblEditorViewModel restbl)
    {
        _restbl = restbl;
    }

    [Menu("Reset RESTBL from File", "Rstb", icon: "fa-arrow-rotate-left")]
    public async Task ResetRestbl()
    {
        await _restbl.Reset();
    }

    [Menu("Generate RCL from File", "Rstb", icon: "fa-wand-magic-sparkles", IsSeparator = true)]
    public async Task GenerateRclFromRestbl()
    {
        await _restbl.Reset();
    }
}
