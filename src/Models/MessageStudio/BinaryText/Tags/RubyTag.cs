using MessageStudio.Formats.BinaryText.Components;
using MessageStudio.Formats.BinaryText.Extensions;
using Revrs;
using System.Runtime.InteropServices;
using System.Text;

namespace NxEditor.EpdPlugin.Models.MessageStudio.BinaryText.Tags;

public class RubyTag
{
    public const string NAME = "Ruby";

    private const string CHAR_SPAN_PARAM = "SpanChars";
    private const string TEXT_PARAM = "Text";

    public static bool WriteUtf16(RevrsWriter writer, in TagParams @params)
    {
        ReadOnlySpan<char> name = @params.Get<string>(TEXT_PARAM);
        ReadOnlySpan<byte> nameRawBytes = MemoryMarshal.Cast<char, byte>(name);

        writer.Write((ushort)(sizeof(ushort) + sizeof(ushort) + nameRawBytes.Length));
        writer.Write(@params.Get<ushort>(CHAR_SPAN_PARAM));

        writer.Write((ushort)nameRawBytes.Length);
        writer.Write(nameRawBytes);

        return true;
    }

    public static bool WriteText(StringBuilder sb, Span<byte> data)
    {
        RevrsReader reader = RevrsReader.Native(data);

        sb.OpenParam(CHAR_SPAN_PARAM);
        sb.Append(reader.Read<ushort>());
        sb.CloseParam();

        ushort rawNameLength = reader.Read<ushort>();
        sb.OpenParam(TEXT_PARAM);
        sb.Append(MemoryMarshal.Cast<byte, char>(reader.ReadSpan<byte>(rawNameLength)));
        sb.CloseParam();

        return true;
    }
}
