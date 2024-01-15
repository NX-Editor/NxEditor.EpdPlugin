using RstbLibrary;
using RstbLibrary.Helpers;
using System.Text;

namespace NxEditor.EpdPlugin.Models.ResourceSizeTable;

public class RstbChangeLogGenerator
{
    private readonly Rstb _source;
    private readonly Rstb _edited;
    private readonly Dictionary<uint, string> _lookup;

    public RstbChangeLogGenerator(Rstb source, Rstb edited)
    {
        _source = source;
        _edited = edited;
        _lookup = [];

        string[] strings = File.Exists(EpdConfig.Shared.RestblStrings)
            ? File.ReadAllLines(EpdConfig.Shared.RestblStrings) : [];

        foreach (string str in strings) {
            uint hash = Crc32.Compute(str);
            _lookup.TryAdd(hash, str);
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
        foreach ((uint key, uint modifiedValue) in _edited.HashTable) {
            if (!_source.HashTable.TryGetValue(key, out uint srcValue)) {
                content.AppendLine($"+ {GetHashName(key)} = {modifiedValue}");
            }
            else if (srcValue != modifiedValue) {
                content.AppendLine($"* {GetHashName(key)} = {modifiedValue}");
            }
        }

        foreach ((uint key, _) in _source.HashTable) {
            if (!_edited.HashTable.ContainsKey(key)) {
                content.AppendLine($"- {GetHashName(key)}");
            }
        }
    }

    private void AppendNameTableDiff(StringBuilder content)
    {
        foreach ((string key, uint modifiedValue) in _edited.OverflowTable) {
            if (!_source.OverflowTable.TryGetValue(key, out uint srcValue)) {
                content.AppendLine($"+ {key} = {modifiedValue}");
            }
            else if (srcValue != modifiedValue) {
                content.AppendLine($"* {key} = {modifiedValue}");
            }
        }

        foreach ((string key, _) in _source.OverflowTable) {
            if (!_edited.OverflowTable.ContainsKey(key)) {
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
