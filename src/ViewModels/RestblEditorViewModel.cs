using AvaloniaEdit;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConfigFactory.Avalonia.Helpers;
using ConfigFactory.Core.Attributes;
using CsRestbl;
using Native.IO.Handles;
using NxEditor.EpdPlugin.Models.Rstb;
using NxEditor.EpdPlugin.Views;
using NxEditor.PluginBase.Components;
using NxEditor.PluginBase.Models;
using System.Collections.ObjectModel;
using System.Text;

namespace NxEditor.EpdPlugin.ViewModels;


public partial class RestblEditorViewModel : Editor<RestblEditorView>
{
    private bool _isChangeLocked = false;
    private Restbl _restbl = new();

    public RestblEditorViewModel(IFileHandle handle) : base(handle) { }

    public override bool HasChanged => ChangelogFiles.Any(x => x.HasChanged);
    public override string[] ExportExtensions { get; } = {
        "RESTBL:*.rsizetable|",
        "Compressed:*.rsizetable.zs|"
    };

    [ObservableProperty]
    private ObservableCollection<RestblChangeLog> _changelogFiles = RestblChangeLog.FromLocalStorage();

    [ObservableProperty]
    private RestblChangeLog _current = new();

    public override Task Read()
    {
        View.TextEditor.Text = Current.Content;

        if (File.Exists(EpdConfig.Shared.RestblStrings)) {
            View.StringsEditor.Text = File.ReadAllText(EpdConfig.Shared.RestblStrings);
        }

        _restbl = Restbl.FromBinary(Handle.Data);

        View.TextEditor.TextChanged += (s, e) => {
            if (Current is not null && !_isChangeLocked) {
                Current.HasChanged = true;
            }

            _isChangeLocked = false;
        };

        return Task.CompletedTask;
    }

    public override Task<IFileHandle> Write()
    {
        foreach (var rcl in ChangelogFiles.Where(x => x.IsEnabled)) {
            RestblChange change = rcl.Parse();
            change.Patch(_restbl);
        }

        DataMarshal marshal = _restbl.ToBinary();
        Handle.Data = marshal.ToArray();
        return Task.FromResult(Handle);
    }

    partial void OnCurrentChanging(RestblChangeLog? oldValue, RestblChangeLog newValue)
    {
        if (oldValue is not null) {
            oldValue.Content = View.TextEditor.Text;
        }
    }

    partial void OnCurrentChanged(RestblChangeLog value)
    {
        _isChangeLocked = true;
        View.TextEditor.Text = value?.Content;
    }

    [RelayCommand]
    public void Create()
    {
        Current = null!; // deselect before re-assigning
        Current = new();
    }

    [RelayCommand]
    public void Save()
    {
        Current.Content = View.TextEditor.Text;
        Current.Save();

        if (!ChangelogFiles.Contains(Current)) {
            ChangelogFiles.Add(Current);
        }
    }

    [RelayCommand]
    public void Remove()
    {
        Current.Remove();
        ChangelogFiles.Remove(Current);
        Current = new();
    }

    [RelayCommand]
    public async Task Export()
    {
        BrowserDialog dialog = new(BrowserMode.SaveFile, "Export Restbl Changelog", "Restbl Changelog:*.rcl", $"{Current.Name}.rcl", "epd-library-rcl-export");
        if (await dialog.ShowDialog() is string path) {
            Save();
            File.WriteAllText(path, Current.Content);
        }
    }

    [RelayCommand]
    public async Task Import()
    {
        BrowserDialog dialog = new(BrowserMode.OpenFile, "Import Restbl Changelog", "Restbl Changelog:*.rcl", "epd-library-rcl-import");
        if (await dialog.ShowDialog() is string path) {
            RestblChangeLog rcl = new(path) {
                IsEnabled = true
            };

            ChangelogFiles.Add(rcl);
            Current = rcl;
            Save();
        }
    }

    [RelayCommand]
    public void Help()
    {

    }

    [RelayCommand]
    public void Enable()
    {
        Current.IsEnabled = !Current.IsEnabled;
    }

    [RelayCommand]
    public void FormatText(TextEditor editor)
    {
        StringBuilder sb = new();

        foreach (var rawText in editor.Text.Replace("\r\n", "\n").Split('\n')) {
            string text = rawText.Trim();

            if (string.IsNullOrEmpty(text)) {
                sb.AppendLine();
                continue;
            }

            if (text.StartsWith('+') || text.StartsWith('*') || text.StartsWith('-') || text.StartsWith('#')) {
                sb.AppendLine(text);
                continue;
            }

            int index;
            string stringKey = (index = text.IndexOf(' ')) > -1 ? text[..index] : text;
            uint size = 0;

            if (_restbl.NameTable.Contains(stringKey)) {
                size = _restbl.NameTable[stringKey];
                sb.Append("* ");
                goto End;
            }

            uint hashKey = Crc32.Compute(stringKey);
            if (_restbl.CrcTable.Contains(hashKey)) {
                size = _restbl.CrcTable[hashKey];
                sb.Append("* ");
                goto End;
            }

            sb.Append("+ ");

        End:
            sb.Append(stringKey);
            sb.AppendLine($" = {size}");
        }

        editor.Text = sb.ToString()[..^Environment.NewLine.Length];
    }
}
