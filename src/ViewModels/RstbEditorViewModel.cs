using Avalonia.Controls;
using AvaloniaEdit;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConfigFactory.Avalonia.Helpers;
using ConfigFactory.Core.Attributes;
using NxEditor.EpdPlugin.Extensions;
using NxEditor.EpdPlugin.Models.ResourceSizeTable;
using NxEditor.EpdPlugin.Views;
using NxEditor.PluginBase;
using NxEditor.PluginBase.Common;
using NxEditor.PluginBase.Components;
using NxEditor.PluginBase.Models;
using RstbLibrary;
using RstbLibrary.Helpers;
using System.Collections.ObjectModel;
using System.Text;

namespace NxEditor.EpdPlugin.ViewModels;


public partial class RstbEditorViewModel : Editor<RestblEditorView>
{
    private bool _isChangeLocked = false;
    private Rstb _rstb = new();

    public RstbEditorViewModel(IEditorFile handle) : base(handle)
    {
        _current = _changelogFiles.FirstOrDefault() ?? new();
        MenuModel = new RstbActionsMenu(this);
    }

    public override bool HasChanged => ChangelogFiles.Any(x => x.HasChanged);
    public override string[] ExportExtensions { get; } = [
        "RESTBL:*.rsizetable|",
        "Compressed:*.rsizetable.zs|"
    ];

    [ObservableProperty]
    private ObservableCollection<RestblChangeLog> _changelogFiles = RestblChangeLog.FromLocalStorage();

    [ObservableProperty]
    private RestblChangeLog _current;

    public override void Read()
    {
        View.TextEditor.Text = Current.Content;

        if (File.Exists(EpdConfig.Shared.RestblStrings)) {
            View.StringsEditor.Text = File.ReadAllText(EpdConfig.Shared.RestblStrings);
        }

        _rstb = Rstb.FromBinary(Handle.Source);

        View.TextEditor.TextChanged += (s, e) => {
            if (Current is not null && !_isChangeLocked) {
                Current.HasChanged = true;
            }

            _isChangeLocked = false;
        };
    }

    public override Span<byte> Write()
    {
        foreach (var rcl in ChangelogFiles.Where(x => x.IsEnabled)) {
            RestblChange change = rcl.Parse();
            change.Patch(_rstb);
        }

        return _rstb.ToBinary().ToArray();
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
        await DialogBox.ShowAsync("RSTB Help",
            new Button {
                Content = "Online Documentation",
                Classes = { "Hyperlink" },
                Command = new RelayCommand(async () => {
                    await "https://github.com/NX-Editor/NxEditor.EpdPlugin/blob/master/src/Resources/RestblEditor.Info.md".OpenUrl();
                })
            }, showSecondaryButton: false);
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

            if (_rstb.OverflowTable.TryGetValue(stringKey, out uint value)) {
                size = value;
                sb.Append("* ");
                goto End;
            }

            uint hashKey = Crc32.Compute(stringKey);
            if (_rstb.HashTable.TryGetValue(hashKey, out value)) {
                size = value;
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
            await ResetFromHandle(EditorFile.FromFile(path));
        }
    }

    private async Task ResetFromHandle(IEditorFile handle)
    {
        // Process the request to make
        // sure compression is handled
        handle = (await ServiceLoader.Shared.RequestService(handle)).Handle;

        if (handle.Source.AsSpan()[0..6].SequenceEqual("RESTBL"u8)) {
            _rstb = Rstb.FromBinary(handle.Source);
        }
    }

    [RelayCommand]
    public async Task GenerateRcl()
    {
        BrowserDialog dialog = new(BrowserMode.OpenFile, "Open Un-edited Restbl File", "RESTBL:*.rsizetable|Any File:*.*",
            instanceBrowserKey: "epd-open-restbl-for-rcl-gen");
        if (await dialog.ShowDialog() is string path) {
            await GenerateRclFromHandle(EditorFile.FromFile(path));
        }
    }

    private async Task GenerateRclFromHandle(IEditorFile handle)
    {
        // Process the request to make
        // sure compression is handled
        handle = (await ServiceLoader.Shared.RequestService(handle)).Handle;

        if (handle.Source.AsSpan()[0..6].SequenceEqual("RESTBL"u8) || handle.Source.AsSpan()[0..4].SequenceEqual("RSTB"u8)) {
            RstbChangeLogGenerator generator = new(Rstb.FromBinary(handle.Source), _rstb);
            if (generator.GenerateRcl() is RestblChangeLog rcl) {
                ChangelogFiles.Add(rcl);
                rcl.Save();
            }
        }
    }
}
