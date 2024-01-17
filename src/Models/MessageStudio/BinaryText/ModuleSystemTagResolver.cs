using MessageStudio.Formats.BinaryText.Components;
using NxEditor.EpdPlugin.Models.MessageStudio.BinaryText.Tags;
using Revrs;
using System.Text;

namespace NxEditor.TotkPlugin.Models.MessageStudio.BinaryText;

public enum FontColor : ushort
{
    White = 0xFFFF,
    Red = 0x0000
}

public class ModuleSystemTagResolver : ITagResolver
{
    public string? GetName(ushort group, ushort type)
    {
        return (group, type) switch {
            (0, 3) => FontColorTag.NAME,
            _ => null
        };
    }

    public (ushort, ushort)? GetGroupAndType(ReadOnlySpan<char> name)
    {
        return name switch {
            FontColorTag.NAME => (0, 3),
            _ => null
        };
    }

    public bool WriteBinaryUtf16(RevrsWriter writer, ushort group, ushort type, in TagParams @params)
    {
        return (group, type) switch {
            (0, 3) => FontColorTag.WriteUtf16(writer, @params),
            _ => false
        };
    }

    public bool WriteBinaryUtf8(RevrsWriter writer, ushort group, ushort type, in TagParams @params)
    {
        throw new NotImplementedException();
    }

    public bool WriteText(StringBuilder sb, ushort group, ushort type, Span<byte> data)
    {
        return (group, type) switch {
            (0, 3) => FontColorTag.WriteText(sb, data),
            _ => false
        };
    }
}
