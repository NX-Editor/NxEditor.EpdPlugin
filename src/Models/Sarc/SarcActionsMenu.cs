using Avalonia.Controls;
using ConfigFactory.Avalonia.Helpers;
using ConfigFactory.Core.Attributes;
using NxEditor.EpdPlugin.ViewModels;
using NxEditor.PluginBase;
using NxEditor.PluginBase.Attributes;

namespace NxEditor.EpdPlugin.Models.Sarc;

public class SarcActionsMenu(SarcEditorViewModel sarc)
{
    private readonly SarcEditorViewModel _sarc = sarc;

    [Menu("Import File", "Sarc", "Ctrl + Shift + A", "fa-solid fa-file-circle-plus")]
    public async Task ImportFile()
    {
        BrowserDialog dialog = new(BrowserMode.OpenFile, "Import Files", "Any File:*.*", instanceBrowserKey: "import-sarc-file");
        if (await dialog.ShowDialog(allowMultiple: true) is IEnumerable<string> paths) {
            foreach (var path in paths.Where(File.Exists)) {
                _sarc.ImportFile(path, File.ReadAllBytes(path));
            }
        }
    }

    [Menu("Import Folder", "Sarc", "Ctrl + Shift + F", "fa-solid fa-folder-plus")]
    public async Task ImportFolder()
    {
        BrowserDialog dialog = new(BrowserMode.OpenFolder, "Import Folder", "Any File:*.*", instanceBrowserKey: "import-sarc-folder");
        if (await dialog.ShowDialog() is string path) {
            _sarc.ImportFolder(path);
        }
    }

    [Menu("Export All", "Sarc", "Ctrl + Shift + E", "fa-solid fa-arrow-right-from-bracket", IsSeparator = true)]
    public async Task ExportAll()
    {
        BrowserDialog dialog = new(BrowserMode.OpenFolder, "Open Folder", instanceBrowserKey: "export-all-sarc-folder");
        if (await dialog.ShowDialog() is string path) {
            StatusModal.Set("Exporting files", "fa-arrow-right-from-bracket", isWorkingStatus: true);
            await Task.Run(() => _sarc.Root.Export(path));
            StatusModal.Set("Export successful", "fa-circle-check", temporaryStatusTime: 1.8);
        }
    }
}
