using NxEditor.EpdPlugin.ViewModels;
using NxEditor.EpdPlugin.Views.Panels;
using NxEditor.PluginBase.Attributes;
using NxEditor.PluginBase.Common;

namespace NxEditor.EpdPlugin.Models.Msbt;

public class MsbtPropertiesFooter(MsbtEditorViewModel editor)
{
    private readonly MsbtEditorViewModel _editor = editor;

    [Footer("fa-solid fa-sliders", "Properties")]
    public async Task Properties()
    {
        await DialogBox.ShowAsync("Properties", new MsbtPropertiesPanel {
            DataContext = _editor
        });
    }
}
