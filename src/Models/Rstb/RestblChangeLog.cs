using CommunityToolkit.Mvvm.ComponentModel;
using NxEditor.PluginBase;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace NxEditor.EpdPlugin.Models.Rstb;

public partial class RestblChangeLog : ObservableObject
{
    private static readonly string _path = Path.Combine(GlobalConfig.Shared.StorageFolder, "resources", "rcl");
    private static readonly string _metaPath = Path.Combine(_path, "meta.json");
    private static readonly Dictionary<string, bool> _meta = File.Exists(_metaPath)
        ? JsonSerializer.Deserialize<Dictionary<string, bool>>(File.ReadAllBytes(_metaPath))! : new();

    static RestblChangeLog()
    {
        Directory.CreateDirectory(_path);
    }

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _filePath;

    [ObservableProperty]
    private string _content = string.Empty;

    [ObservableProperty]
    private bool _isEnabled = false;

    [ObservableProperty]
    private bool _hasChanged = false;

    public RestblChangeLog()
    {
        _name = GetIncrementName();
        _filePath = Path.Combine(_path, $"{_name}.rcl");
        _isEnabled = true;
    }

    public RestblChangeLog(string path)
    {
        _filePath = path;
        _name = Path.GetFileNameWithoutExtension(path);
        _content = File.ReadAllText(path);

        if (_meta.TryGetValue(_name, out bool value)) {
            _isEnabled = value;
        }
    }

    public void Save()
    {
        HasChanged = false;
        _meta[Name] = IsEnabled;
        File.WriteAllText(FilePath, Content);
    }

    public void Remove()
    {
        _meta.Remove(Name);
        File.Delete(FilePath);
    }

    public RestblChange Parse()
    {
        RestblChange result = new();
        foreach (var line in Content.Replace(" ", string.Empty).Replace("\r\n", "\n").Split('\n'))
        {
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            ReadOnlySpan<char> text = line.AsSpan();

            if (line.StartsWith('-'))
            {
                result.Entries.Add(text[1..].ToString(), new Tuple<uint, RestblChangeMode>(
                    0, RestblChangeMode.Removal));
                continue;
            }

            if (line.StartsWith('+'))
            {
                if (Parse(text[1..], out (string name, uint value) entry))
                {
                    result.Entries.Add(entry.name, new Tuple<uint, RestblChangeMode>(
                        entry.value, RestblChangeMode.Addition));
                }

                continue;
            }

            if (line.StartsWith('*'))
            {
                if (Parse(text[1..], out (string name, uint value) entry))
                {
                    result.Entries.Add(entry.name, new Tuple<uint, RestblChangeMode>(
                        entry.value, RestblChangeMode.Modification));
                }

                continue;
            }
        }

        return result;
    }

    private static bool Parse(ReadOnlySpan<char> text, out (string name, uint value) entry)
    {
        entry = new();
        int splitIndex = text.IndexOf('=');

        if (splitIndex < 0)
        {
            return false;
        }

        entry.name = text[..splitIndex].ToString();
        entry.value = Convert.ToUInt32(text[++splitIndex..].ToString().Replace(",", string.Empty));

        return true;
    }

    public static string GetIncrementName()
    {
        int i = 1;
        string name = $"changelog-{i}";
        while (File.Exists(Path.Combine(_path, $"{name}.rcl")))
        {
            i++;
            name = $"changelog-{i}";
        }

        return name;
    }

    public static ObservableCollection<RestblChangeLog> FromLocalStorage()
    {
        if (!Directory.Exists(_path))
        {
            return new();
        }

        return new(Directory.EnumerateFiles(_path, "*.rcl")
            .Select(x => new RestblChangeLog(x)));
    }

    partial void OnNameChanged(string value)
    {
        if (File.Exists(FilePath))
        {
            _meta.Remove(Path.GetFileNameWithoutExtension(FilePath));
            _meta[value] = IsEnabled;

            File.Move(FilePath, FilePath = Path.Combine(_path, $"{value}.rcl"));
        }
    }

    partial void OnIsEnabledChanged(bool value)
    {
        _meta[Name] = value;

        using FileStream fs = File.Create(_metaPath);
        JsonSerializer.Serialize(fs, _meta);
    }
}
