using NxEditor.PluginBase;
using RstbLibrary;
using RstbLibrary.Helpers;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;

namespace NxEditor.EpdPlugin.Models.ResourceSizeTable;

public enum RestblChangeMode
{
    Addition,
    Removal,
    Modification
}

public class RestblChange
{
    public OrderedDictionary Entries { get; set; } = [];

    public void Patch(Rstb rstb)
    {
        foreach (var _key in Entries.Keys) {
            var (value, mode) = (Tuple<uint, RestblChangeMode>)Entries[_key]!;

            string key = (string)_key;
            bool isHashOnly = key.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase);

            if (mode == RestblChangeMode.Addition) {
                AddEntry(rstb, key, value, isHashOnly);
            }
            else if (mode == RestblChangeMode.Modification) {
                PatchEntry(rstb, key, value, isHashOnly);
            }
            else if (mode == RestblChangeMode.Removal) {
                RemoveEntry(rstb, key, isHashOnly);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AddEntry(Rstb rstb, string key, uint value, bool isHashOnly)
    {
        uint hash = isHashOnly ? Convert.ToUInt32(key[2..], 16) : Crc32.Compute(key);
        if (!rstb.HashTable.ContainsKey(hash)) {
            rstb.HashTable[hash] = value;
            return;
        }

        if (isHashOnly) {
            StatusModal.Set($"Hash-only entries cannot be added to the collision table (unknown string '{key}')",
                "fa-solid fa-triangle-exclamation", isWorkingStatus: false, temporaryStatusTime: 1.7);
            return;
        }

        rstb.OverflowTable[key] = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void PatchEntry(Rstb rstb, string key, uint value, bool isHashOnly)
    {
        if (!isHashOnly && rstb.OverflowTable.ContainsKey(key)) {
            rstb.OverflowTable[key] = value;
            return;
        }

        uint hash = isHashOnly ? Convert.ToUInt32(key[2..], 16) : Crc32.Compute(key);
        rstb.HashTable[hash] = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void RemoveEntry(Rstb rstb, string key, bool isHashOnly)
    {
        if (!isHashOnly && rstb.OverflowTable.Remove(key)) {
            return;
        }

        uint hash = isHashOnly ? Convert.ToUInt32(key[2..], 16) : Crc32.Compute(key);
        rstb.HashTable.Remove(hash);
    }
}
