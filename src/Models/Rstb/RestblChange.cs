using CsRestbl;
using NxEditor.PluginBase;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;

namespace NxEditor.EpdPlugin.Models.Rstb;

public enum RestblChangeMode
{
    Addition,
    Removal,
    Modification
}

public class RestblChange
{
    public OrderedDictionary Entries { get; set; } = new();

    public void Patch(Restbl restbl)
    {
        foreach (var _key in Entries.Keys) {
            var (value, mode) = (Tuple<uint, RestblChangeMode>)Entries[_key]!;

            string key = (string)_key;
            bool isHashOnly = key.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase);

            if (mode == RestblChangeMode.Addition) {
                AddEntry(restbl, key, value, isHashOnly);
            }
            else if (mode == RestblChangeMode.Modification) {
                PatchEntry(restbl, key, value, isHashOnly);
            }
            else if (mode == RestblChangeMode.Removal) {
                RemoveEntry(restbl, key, isHashOnly);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AddEntry(Restbl restbl, string key, uint value, bool isHashOnly)
    {
        uint hash = isHashOnly ? Convert.ToUInt32(key[2..], 16) : Crc32.Compute(key);
        if (!restbl.CrcTable.Contains(hash)) {
            restbl.CrcTable[hash] = value;
        }

        if (isHashOnly) {
            StatusModal.Set($"Hash-only entries cannot be added to the collision table (unknown string '{key}')",
                "fa-solid fa-triangle-exclamation", isWorkingStatus: false, temporaryStatusTime: 1.7);
            return;
        }

        restbl.NameTable[key] = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void PatchEntry(Restbl restbl, string key, uint value, bool isHashOnly)
    {
        if (!isHashOnly && restbl.NameTable.Contains(key)) {
            restbl.NameTable[key] = value;
            return;
        }

        uint hash = isHashOnly ? Convert.ToUInt32(key[2..], 16) : Crc32.Compute(key);
        restbl.CrcTable[hash] = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void RemoveEntry(Restbl restbl, string key, bool isHashOnly)
    {
        if (!isHashOnly && restbl.NameTable.Contains(key)) {
            restbl.NameTable.Remove(key);
            return;
        }

        uint hash = isHashOnly ? Convert.ToUInt32(key[2..], 16) : Crc32.Compute(key);
        if (restbl.CrcTable.Contains(hash)) {
            restbl.CrcTable.Remove(hash);
        }
    }
}
