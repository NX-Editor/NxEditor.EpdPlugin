﻿using NxEditor.EpdPlugin.ViewModels;
using NxEditor.PluginBase.Attributes;

namespace NxEditor.EpdPlugin.Models.ResourceSizeTable;

public class RstbActionsMenu(RstbEditorViewModel restbl)
{
    private readonly RstbEditorViewModel _restbl = restbl;

    [Menu("Reset RESTBL from File", "Rstb", icon: "fa-arrow-rotate-left")]
    public async Task ResetRestbl()
    {
        await _restbl.Reset();
    }

    [Menu("Generate RCL from File", "Rstb", icon: "fa-wand-magic-sparkles", IsSeparator = true)]
    public async Task GenerateRclFromRestbl()
    {
        await _restbl.GenerateRcl();
    }
}
