using CsRestbl;
using System.Text;

namespace NxEditor.EpdPlugin.Models.Rstb;

public class RestblChangeLogGenerator
{
    private readonly Restbl _source;
    private readonly Restbl _edited;
    private readonly Dictionary<uint, string> _lookup;

    public RestblChangeLogGenerator(Restbl source, Restbl edited)
    {
        _source = source;
        _edited = edited;
        _lookup = new();

        string[] strings = File.Exists(EpdConfig.Shared.RestblStrings)
            ? File.ReadAllLines(EpdConfig.Shared.RestblStrings) : Array.Empty<string>();

        foreach (string str in strings) {
            uint hash = Crc32.Compute(str);
            if (!_lookup.ContainsKey(hash)) {
                _lookup.Add(hash, str);
            }
        }
    }

    public RestblChangeLog? GenerateRcl()
    {
        StringBuilder content = new();

        AppendHashTableDiff(content);
        AppendNameTableDiff(content);

        if (content.Length <= 0) {
            return null;
        }

        RestblChangeLog rcl = new() {
            Content = content.ToString()
        };

        rcl.Save();
        return rcl;
    }

    private void AppendHashTableDiff(StringBuilder content)
    {
        foreach ((uint key, uint value) in _edited.CrcTable) {
            if (!_source.CrcTable.Contains(key)) {
                content.AppendLine($"+ {GetHashName(key)} = {value}");
            }
            else if (_source.CrcTable[key] != value) {
                content.AppendLine($"* {GetHashName(key)} = {value}");
            }
        }

        foreach ((uint key, _) in _source.CrcTable) {
            if (!_edited.CrcTable.Contains(key)) {
                content.AppendLine($"- {GetHashName(key)}");
            }
        }
    }

    private void AppendNameTableDiff(StringBuilder content)
    {
        foreach ((string key, uint value) in _edited.NameTable) {
            if (!_source.NameTable.Contains(key)) {
                content.AppendLine($"+ {key} = {value}");
            }
            else if (_source.NameTable[key] != value) {
                content.AppendLine($"* {key} = {value}");
            }
        }

        foreach ((string key, _) in _source.NameTable) {
            if (!_edited.NameTable.Contains(key)) {
                content.AppendLine($"- {key}");
            }
        }
    }

    private string GetHashName(uint hash)
    {
        if (_lookup.TryGetValue(hash, out var name)) {
            return name;
        }

        return $"0x{hash:X2}";
    }
}
