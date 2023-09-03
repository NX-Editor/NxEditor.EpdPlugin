using AvaloniaEdit;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConfigFactory.Avalonia.Helpers;
using ConfigFactory.Core.Attributes;
using CsRestbl;
using Native.IO.Handles;
using NxEditor.EpdPlugin.Models.Rstb;
using NxEditor.EpdPlugin.Views;
using NxEditor.PluginBase;
using NxEditor.PluginBase.Common;
using NxEditor.PluginBase.Components;
using NxEditor.PluginBase.Models;
using NxEditor.PluginBase.Services;
using System.Collections.ObjectModel;
using System.Text;

namespace NxEditor.EpdPlugin.ViewModels;


public partial class RestblEditorViewModel : Editor<RestblEditorView>
{
    private bool _isChangeLocked = false;
    private Restbl _restbl = new();

    public RestblEditorViewModel(IFileHandle handle) : base(handle)
    {
        _current = _changelogFiles.FirstOrDefault() ?? new();
        MenuModel = new RestblActionsMenu(this);
    }

    public override bool HasChanged => ChangelogFiles.Any(x => x.HasChanged);
    public override string[] ExportExtensions { get; } = {
        "RESTBL:*.rsizetable|",
        "Compressed:*.rsizetable.zs|"
    };

    [ObservableProperty]
    private ObservableCollection<RestblChangeLog> _changelogFiles = RestblChangeLog.FromLocalStorage();

    [ObservableProperty]
    private RestblChangeLog _current;

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

        if (View != null) {
            View.TextEditor.Text = value?.Content;
        }
    }

    [RelayCommand]
    public void Create()
    {
        RestblChangeLog rcl = new();
        ChangelogFiles.Add(rcl);
        Current = rcl;
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
    public async Task Remove()
    {
        if (await DialogBox.ShowAsync("Warning", $"Are you sure you wish to permanently\ndelete {Current.Name}?",
                primaryButtonContent: "Yes Please", secondaryButtonContent: "No Thanks") == DialogResult.Primary) {
            Current.Remove();
            ChangelogFiles.Remove(Current);
            Current = ChangelogFiles.FirstOrDefault() ?? new();
        }
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
            RestblChangeLog rcl = new(path, copyToLocalStorage: true) {
                IsEnabled = true
            };

            ChangelogFiles.Add(rcl);
            Current = rcl;
            Save();
        }
    }

    [RelayCommand]
    public static async Task Help()
    {
        await DialogBox.ShowAsync("RSTB Help", "Work in progress™", showSecondaryButton: false);
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

    [RelayCommand]
    public async Task Reset()
    {
        BrowserDialog dialog = new(BrowserMode.OpenFile, "Open Restbl File", "RESTBL:*.rsizetable|Any File:*.*",
            instanceBrowserKey: "epd-open-restbl-for-reset");
        if (await dialog.ShowDialog() is string path) {
            await ResetFromHandle(new FileHandle(path));
        }
    }

    private async Task ResetFromHandle(IFileHandle handle)
    {
        IFormatService service = await ServiceLoader.Shared.RequestService(handle);
        if (service.Handle.Data.AsSpan()[0..6].SequenceEqual("RESTBL"u8)) {
            _restbl = Restbl.FromBinary(service.Handle.Data);
        }
    }

    [RelayCommand]
    public async Task GenerateRcl()
    {
        BrowserDialog dialog = new(BrowserMode.OpenFile, "Open Un-edited Restbl File", "RESTBL:*.rsizetable|Any File:*.*",
            instanceBrowserKey: "epd-open-restbl-for-rcl-gen");
        if (await dialog.ShowDialog() is string path) {
            await GenerateRclFromHandle(new FileHandle(path));
        }
    }

    private async Task GenerateRclFromHandle(IFileHandle handle)
    {
        IFormatService service = await ServiceLoader.Shared.RequestService(handle);
        if (service.Handle.Data.AsSpan()[0..6].SequenceEqual("RESTBL"u8)) {
            RestblChangeLogGenerator generator = new(Restbl.FromBinary(service.Handle.Data), _restbl);
            if (generator.GenerateRcl() is RestblChangeLog rcl) {
                ChangelogFiles.Add(rcl);
                rcl.Save();
            }
        }
    }
}
