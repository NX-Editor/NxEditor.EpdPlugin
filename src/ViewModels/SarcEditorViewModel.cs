using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using ConfigFactory.Avalonia.Helpers;
using ConfigFactory.Core.Attributes;
using CsOead;
using NxEditor.EpdPlugin.Models.Sarc;
using NxEditor.EpdPlugin.Views;
using NxEditor.PluginBase;
using NxEditor.PluginBase.Components;
using NxEditor.PluginBase.Models;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using NodeMap = System.Collections.Generic.Dictionary<string, (NxEditor.EpdPlugin.Models.Sarc.SarcFileNode root, object map)>;

namespace NxEditor.EpdPlugin.ViewModels;

public partial class SarcEditorViewModel : Editor<SarcEditorView>
{
    private readonly List<SarcFileNode> _searchCache = [];
    private readonly NodeMap _map = [];
    private readonly string _temp;

    public SarcHistoryStack History { get; }

    [ObservableProperty]
    private SarcFileNode _root = new("__root__");

    [ObservableProperty]
    private ObservableCollection<SarcFileNode> _selected = new();

    [ObservableProperty]
    private bool _isFinding;

    [ObservableProperty]
    private bool _isReplacing;

    [ObservableProperty]
    private bool _matchCase;

    [ObservableProperty]
    private string? _findField;

    [ObservableProperty]
    private string? _replaceField;

    [ObservableProperty]
    private int _searchCount;

    [ObservableProperty]
    private int _searchIndex = -1;

    public SarcEditorViewModel(IFileHandle handle) : base(handle)
    {
        History = new(this);
        MenuModel = new SarcActionsMenu(this);

        // Create temp directory
        _temp = Directory.CreateDirectory(
            Path.Combine(Path.GetTempPath(), "nx-editor", Path.GetFileName(handle.FilePath ?? handle.Name), Guid.NewGuid().ToString())
        ).FullName;
    }

    public override bool HasChanged => History.HasChanges;
    public override string[] ExportExtensions { get; } = {
        "Actor Pack:*.bactorpack|",
        "Pack:*.pack|",
        "Sead Archive:*.sarc|",
        "Compressed:*.zs|"
    };

    public override Task Read()
    {
        Sarc sarc = Sarc.FromBinary(Handle.Data);
        foreach ((var name, var sarcFile) in sarc.OrderBy(x => x.Key)) {
            CreateNodeFromPath(name, sarcFile.ToArray());
        }

        return Task.CompletedTask;
    }

    public override Task<IFileHandle> Write()
    {
        using Sarc sarc = new();
        foreach (var file in Root.GetFileNodes()) {
            sarc.Add(Path.Combine(file.GetPath(), file.Name)
                .Replace(Path.DirectorySeparatorChar, '/'), file.Data);
        }

        Handle.Data = sarc.ToBinary().ToArray();
        return Task.FromResult(Handle);
    }

    public override Task Undo()
    {
        History.InvokeLastAction(isRedo: false);
        return base.Undo();
    }

    public override Task Redo()
    {
        History.InvokeLastAction(isRedo: true);
        return base.Redo();
    }

    public override Task SelectAll()
    {
        View.DropClient.SelectAll();
        return base.SelectAll();
    }

    public override async Task Cut()
    {
        await Copy();
        Remove();
        await base.Cut();
    }

    public override async Task Copy()
    {
        DataObject obj = new();

        List<IStorageItem?> payload = new();
        IStorageProvider storageProvider = Frontend.Locate<IStorageProvider>();
        foreach (var node in Selected) {
            foreach (var file in node.GetFileNodes()) {
                string path = Path.Combine(_temp, file.GetPath(), file.Name);
                file.Export(path, isSingleFile: true);

                payload.Add(node.IsFile
                    ? await storageProvider.TryGetFileFromPathAsync(path)
                    : await storageProvider.TryGetFolderFromPathAsync(Path.Combine(_temp, node.GetPath(), node.Name))
                );
            }
        }

        obj.Set("Files", payload.DistinctBy(x => x?.Path));

        await Frontend.Locate<IClipboard>().SetDataObjectAsync(obj);
        await base.Copy();
    }

    public override async Task Paste()
    {
        if (await Frontend.Locate<IClipboard>().GetDataAsync("Files") is IEnumerable<IStorageItem> files) {
            foreach (var path in files.Select(x => x.Path.LocalPath)) {
                if (File.Exists(path)) {
                    ImportFile(path, File.ReadAllBytes(path));
                }
                else if (Directory.Exists(path)) {
                    ImportFolder(path);
                }
            }
        }

        await base.Paste();
    }

    public override Task Find()
    {
        IsFinding = !(IsReplacing = false);
        return base.Find();
    }

    public override Task FindAndReplace()
    {
        IsFinding = IsReplacing = true;
        return base.FindAndReplace();
    }

    partial void OnMatchCaseChanged(bool value) => OnFindFieldChanged(FindField);
    partial void OnFindFieldChanged(string? value)
    {
        _searchCache.Clear();
        SearchCount = -1;

        if (string.IsNullOrEmpty(value)) {
            return;
        }

        Iter(Root);
        SearchCount = _searchCache.Count - 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Iter(SarcFileNode node)
        {
            foreach (var child in node.Children) {
                if (!child.IsFile) {
                    Iter(child);
                }
                else if ((MatchCase ? child.Name : child.Name.ToLower()).Contains(MatchCase ? value! : value!.ToLower())) {
                    _searchCache.Add(child);
                }
            }
        }
    }

    public void FindNextCommand(bool clearSelection) => FindNext(clearSelection, findLast: false);
    public (SarcFileNode, string?) FindNext(bool clearSelection, bool findLast = false)
    {
        if (!_searchCache.Any()) {
            return (null!, null);
        }

        if (clearSelection) {
            Selected.Clear();
        }

        // Find/select next node
        SarcFileNode node = findLast
            ? _searchCache[SearchIndex = SearchIndex == 0 ? SearchCount : --SearchIndex]
            : _searchCache[SearchIndex = SearchIndex >= SearchCount ? 0 : ++SearchIndex];
        node.IsSelected = true;

        // Expand path to node
        SarcFileNode? parent = node;
        while ((parent = parent.Parent) != null) {
            parent.IsExpanded = true;
        }

        // Add to selection
        Selected.Add(node);

        // Execute replace function
        string? prevNameCache = null;
        if (IsReplacing && ReplaceField != null) {
            // Rename the selected item
            node.PrevName = prevNameCache = node.Name;
            node.Name = node.Name.Replace(FindField ?? "", ReplaceField);

            if (clearSelection) {
                History.StageChange(SarcChange.Rename, new List<(SarcFileNode, object?)>() {
                    (node, node.PrevName)
                });
            }

            RenameMapNode(node);
        }

        return (node, prevNameCache);
    }

    public void FindAll()
    {
        List<(SarcFileNode, object?)>? changes = (IsReplacing && ReplaceField != null) ? new() : null;

        Selected.Clear();
        SearchIndex = -1;
        while (SearchIndex < SearchCount) {
            (SarcFileNode node, string? prevName) = FindNext(clearSelection: false);
            changes?.Add((node, prevName!));
        }

        if (changes != null) {
            History.StageChange(SarcChange.Rename, changes);
        }
    }

    public void ChangeFindMode() => IsReplacing = !IsReplacing;
    public void CloseFindDialog() => IsFinding = false;

    public SarcFileNode ImportFile(string path, byte[] data, bool isRelPath = false, SarcFileNode? parentNode = null)
    {
        if (CreateNodeFromPath(isRelPath ? path : Path.GetFileName(path), data, expandParentTree: true, parentNode) is SarcFileNode node) {
            node.IsSelected = true;
            Selected.Add(node);

            if (!isRelPath) {
                History.StageChange(SarcChange.Import, new(1) {
                    (node, null)
                });
            }

            return node;
        }

        throw new InvalidOperationException("Import Failed: The file node could not be created");
    }

    public void ImportFolder(string path, bool importTopLevel = false, SarcFileNode? parentNode = null)
    {
        string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
        if (files.Length > 0) {
            (SarcFileNode node, object? _)[] nodes = new (SarcFileNode, object? _)[files.Length];
            for (int i = 0; i < files.Length; i++) {
                string file = files[i];
                nodes[i].node = ImportFile(Path.GetRelativePath(importTopLevel ? Path.GetDirectoryName(path)! : path, file),
                    File.ReadAllBytes(file), isRelPath: true, parentNode);
            }

            History.StageChange(SarcChange.Import, nodes.ToList());
        }
    }

    public void Edit()
    {
        if (Selected.FirstOrDefault() is SarcFileNode node && !node.IsRenaming && node.IsFile) {
            IEditorManager? editorMgr = Frontend.Locate<IEditorManager>();
            editorMgr?.TryLoadEditor(node);
        }
    }

    public void Rename()
    {
        if (Selected.FirstOrDefault() is SarcFileNode node) {
            node.BeginRename();
        }
    }

    public async Task Export()
    {
        if (Selected.Count <= 0) {
            return;
        }

        if (Selected.Count == 1 && Selected[0].IsFile) {
            BrowserDialog dialog = new(BrowserMode.SaveFile, "Save File", "Any File:*.*", Path.GetFileName(Selected[0].Name), "export-sarc-file");
            if (await dialog.ShowDialog() is string path) {
                Selected[0].Export(path, isSingleFile: true);
            }
        }
        else {
            BrowserDialog dialog = new(BrowserMode.OpenFolder, "Save to Folder", "Any File:*.*", instanceBrowserKey: "export-sarc-folder");
            if (await dialog.ShowDialog() is string path) {
                foreach (var node in Selected) {
                    node.Export(path, relativeTo: node.Parent);
                }
            }
        }
    }

    public async Task ExportPath()
    {
        if (Selected.Count <= 0) {
            return;
        }

        if (Selected.Count == 1 && Selected[0].IsFile) {
            BrowserDialog dialog = new(BrowserMode.SaveFile, "Save File", "Any File:*.*", Path.GetFileName(Selected[0].Name), "export-sarc-file");
            if (await dialog.ShowDialog() is string path) {
                Selected[0].Export(path, relativeTo: Root);
            }
        }
        else {
            BrowserDialog dialog = new(BrowserMode.OpenFolder, "Save to Folder", "Any File:*.*", instanceBrowserKey: "export-sarc-folder");
            if (await dialog.ShowDialog() is string path) {
                foreach (var node in Selected) {
                    node.Export(path, relativeTo: Root);
                }
            }
        }
    }

    public async Task Replace()
    {
        if (Selected.FirstOrDefault() is SarcFileNode node && node.IsFile) {
            BrowserDialog dialog = new(BrowserMode.OpenFile, "Replace File", "Any File:*.*", instanceBrowserKey: "replace-sarc-file");
            if (await dialog.ShowDialog() is string path && File.Exists(path)) {
                node.Data = File.ReadAllBytes(path);
                StatusModal.Set($"File Replaced", isWorkingStatus: false, temporaryStatusTime: 2.5);
            }
        }
    }

    public void Remove()
    {
        if (Selected.Any()) {
            History.StageChange(SarcChange.Remove, Selected.Select(x => (x, null as object)).ToList());
            for (int i = 0; i < Selected.Count; i++) {
                var item = Selected[i];
                (item.Parent ?? Root).Children.Remove(item);
                RemoveNodeFromMap(item);
            }
        }
    }

    private SarcFileNode? CreateNodeFromPath(string path, byte[] data, bool expandParentTree = false, SarcFileNode? parentNode = null)
    {
        NodeMap map = parentNode != null ? FindNodeMap(parentNode)! : _map;
        SarcFileNode item = parentNode ?? Root;

        foreach (var part in path.Replace('\\', '/').Split('/')) {
            if (!map.TryGetValue(part, out var node)) {
                map[part] = node = (new(part, item), new NodeMap());
                item.Children.Add(node.root);
            }
            else if (node.root.IsFile) {
                StatusModal.Set($"Replaced {node.root.Name}", isWorkingStatus: false, temporaryStatusTime: 2.5);
            }

            item = node.root;
            map = (NodeMap)node.map;

            if (expandParentTree) {
                item.IsExpanded = true;
                Selected.Add(item);
            }
        }

        if (item != null) {
            item.Data = data;
            return item;
        }

        throw new Exception($"Import Failed: the tree item was null - '{path}' ({data.Length})");
    }

    internal void AddNodeToTree(SarcFileNode node)
    {
        NodeMap? map = FindNodeMap(node, createPath: true);
        map?.Add(node.Name, (node, new NodeMap()));
    }

    internal (NodeMap, NodeMap) RemoveNodeFromMap(SarcFileNode node, string? key = null)
    {
        key ??= node.Name;
        NodeMap? map = FindNodeMap(node.Parent!);
        if (map != null) {
            NodeMap child = (NodeMap)map[key].map;
            map?.Remove(key);
            return (map!, child!);
        }

        return (null!, null!);
    }

    internal NodeMap? FindNodeMap(SarcFileNode node, bool createPath = false)
    {
        NodeMap map = _map;
        SarcFileNode item = Root;

        foreach (var part in node.GetPathParts()) {
            if (!map.TryGetValue(part, out var _node)) {
                if (createPath) {
                    map[part] = _node = (new(part, item), new NodeMap());
                }

                return null;
            }

            item = _node.root;
            map = (NodeMap)_node.map;
        }

        return map;
    }

    internal void RenameMapNode(SarcFileNode node)
    {
        (NodeMap map, NodeMap child) = RemoveNodeFromMap(node, node.PrevName)!;

        map![node.Name] = (node, child);
        node.PrevName = null;
    }

    public override void Cleanup()
    {
        if (Directory.Exists(_temp)) {
            Directory.Delete(_temp, true);
        }
    }
}
