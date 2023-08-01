using CsRestbl;
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
        foreach (var key in Entries.Keys)
        {
            var (value, mode) = (Tuple<uint, RestblChangeMode>)Entries[key]!;

            if (mode == RestblChangeMode.Addition)
            {
                AddEntry(restbl, (string)key, value);
            }
            else if (mode == RestblChangeMode.Modification)
            {
                PatchEntry(restbl, (string)key, value);
            }
            else if (mode == RestblChangeMode.Removal)
            {
                RemoveEntry(restbl, (string)key);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AddEntry(Restbl restbl, string key, uint value)
    {
        uint hash = Crc32.Compute(key);
        if (!restbl.CrcTable.Contains(hash))
        {
            restbl.CrcTable[hash] = value;
        }

        restbl.NameTable[key] = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void PatchEntry(Restbl restbl, string key, uint value)
    {
        if (restbl.NameTable.Contains(key))
        {
            restbl.NameTable[key] = value;
            return;
        }

        restbl.CrcTable[Crc32.Compute(key)] = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void RemoveEntry(Restbl restbl, string key)
    {
        if (restbl.NameTable.Contains(key))
        {
            restbl.NameTable.Remove(key);
            return;
        }

        uint hash = Crc32.Compute(key);
        if (restbl.CrcTable.Contains(hash))
        {
            restbl.CrcTable.Remove(hash);
        }
    }
}
