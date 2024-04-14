using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using NxEditor.EpdPlugin.ViewModels;
using NxEditor.PluginBase.Models;
using System.Collections.ObjectModel;

namespace NxEditor.EpdPlugin.Models.Sarc;

public partial class SarcFileNode(string name, SarcFileNode? parent = null) : ObservableObject
{
    private TextBox? _renameClient;
    private ArraySegment<byte>? _data;

    [ObservableProperty]
    private SarcFileNode? _parent = parent;

    [ObservableProperty]
    private string _name = name;

    [ObservableProperty]
    private ObservableCollection<SarcFileNode> _children = [];

    [ObservableProperty]
    private bool _isRenaming;

    [ObservableProperty]
    private bool _isExpanded;

    [ObservableProperty]
    private bool _isVisible = true;

    public ArraySegment<byte> Data {
        get => _data ?? throw new NullReferenceException("Only file nodes contain data");
        set => _data = value;
    }

    public bool IsFile => _data != null;
    public string? PrevName { get; set; }

    public void Sort()
    {
        Children = new(Children.OrderBy(x => x.Name));
        foreach (var child in Children) {
            child.Sort();
        }
    }

    public void BeginRename()
    {
        PrevName = Name;
        IsRenaming = true;

        _renameClient?.SelectAll();
        _renameClient?.Focus();
    }

    public void EndRename(SarcEditorViewModel owner)
    {
        owner.History.StageChange(SarcChange.Rename, [
            (this, PrevName)
        ]);
        owner.RenameMapNode(this);

        IsRenaming = false;
    }

    public void Export(string path, bool recursive = true, bool isSingleFile = false, SarcFileNode? relativeTo = null)
    {
        if (IsFile && relativeTo != null) {
            Directory.CreateDirectory(path = Path.Combine(Path.GetDirectoryName(path)!, GetPath(relativeTo)));
            using FileStream fs = File.Create(Path.Combine(path, Name));
            fs.Write(Data);
        }
        else if (IsFile) {
            Directory.CreateDirectory(isSingleFile ? Path.GetDirectoryName(path)! : path);
            using FileStream fs = File.Create(isSingleFile ? path : Path.Combine(path, Name));
            fs.Write(Data);
        }
        else {
            foreach (var file in GetFileNodes(recursive)) {
                file.Export(Path.Combine(path, file.GetPath(relativeTo)));
            }
        }
    }

    public EditorFile GetEditorFile(IEditorFile parent)
    {
        return new(
            Path.Combine(parent.Id, GetFilePath()),
            $"{parent.Name}/{Name}",
            [.. Data], // The passed data is mutable, so we
                       // give the handle a copy for safety
            (ref Span<byte> data) => {
                Data = data.ToArray();
            }
        );
    }

    public string GetFilePath(SarcFileNode? relativeTo = null)
    {
        return Path.Combine(
            [.. GetPathParts(relativeTo), Name]);
    }

    public string GetPath(SarcFileNode? relativeTo = null)
    {
        return Path.Combine(
            [.. GetPathParts(relativeTo)]);
    }

    public Stack<string> GetPathParts(SarcFileNode? relativeTo = null)
    {
        Stack<string> parts = new();

        if (!IsFile && Name != "__root__") {
            parts.Push(Name);
        }

        SarcFileNode? parent = Parent;
        while (parent != null && parent != relativeTo && parent.Name != "__root__") {
            parts.Push(parent.Name);
            parent = parent.Parent;
        }

        return parts;
    }

    public IEnumerable<SarcFileNode> GetFileNodes(bool recursive = true)
    {
        IEnumerable<SarcFileNode> result;

        if (!IsFile) {
            result = Children.Where(x => x.IsFile);
            foreach (var child in Children.Where(x => !x.IsFile)) {
                result = result.Concat(child.GetFileNodes(recursive));
            }
        }
        else {
            result = [
                this
            ];
        }

        return result;
    }

    public void SetData(byte[] data)
    {
        Data = data;
    }

    internal void SetRenameClient(TextBox? renameClient)
    {
        _renameClient = renameClient;
    }
}
