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
            (0, 0) => RubyTag.NAME,
            (0, 2) => FontSizeTag.NAME,
            (0, 3) => FontColorTag.NAME,
            (0, 4) => PageBreakTag.NAME,
            (2, 1) => FlagTag.STRING_NAME,
            (2, 2) => FlagTag.FLOAT_NAME,
            (2, 9) => FlagTag.INT_NAME,
            _ => null
        };
    }

    public (ushort, ushort)? GetGroupAndType(ReadOnlySpan<char> name)
    {
        return name switch {
            RubyTag.NAME => (0, 0),
            FontSizeTag.NAME => (0, 2),
            FontColorTag.NAME => (0, 3),
            PageBreakTag.NAME => (0, 4),
            FlagTag.STRING_NAME => (2, 1),
            FlagTag.FLOAT_NAME => (2, 2),
            FlagTag.INT_NAME => (2, 9),
            _ => null
        };
    }

    public bool WriteBinaryUtf16(RevrsWriter writer, ushort group, ushort type, in TagParams @params)
    {
        return (group, type) switch {
            (0, 0) => RubyTag.WriteUtf16(writer, @params),
            (0, 2) => FontSizeTag.WriteUtf16(writer, @params),
            (0, 3) => FontColorTag.WriteUtf16(writer, @params),
            (0, 4) => PageBreakTag.WriteUtf16(writer),
            (2, 1) or (2, 2) or (2, 9) => FlagTag.WriteUtf16(writer, @params),
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
            (0, 0) => RubyTag.WriteText(sb, data),
            (0, 2) => FontSizeTag.WriteText(sb, data),
            (0, 3) => FontColorTag.WriteText(sb, data),
            (0, 4) => true,
            (2, 1) or (2, 2) or (2, 9) => FlagTag.WriteText(sb, data),
            _ => false
        };
    }
}
