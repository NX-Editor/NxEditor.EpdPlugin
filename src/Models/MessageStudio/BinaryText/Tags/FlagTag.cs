using MessageStudio.Formats.BinaryText.Components;
using MessageStudio.Formats.BinaryText.Extensions;
using Revrs;
using System.Runtime.InteropServices;
using System.Text;

namespace NxEditor.EpdPlugin.Models.MessageStudio.BinaryText.Tags;

public class FlagTag
{
    public const string FLOAT_NAME = "IntFlag";
    public const string INT_NAME = "FloatFlag";
    public const string STRING_NAME = "StringFlag";

    private const string NAME_PARAM = "Name";
    private const string UNKNOWN_PARAM_1 = "Unknown1";
    private const string UNKNOWN_PARAM_2 = "Unknown2";

    public static bool WriteUtf16(RevrsWriter writer, in TagParams @params)
    {
        ReadOnlySpan<char> name = @params.Get<string>(NAME_PARAM);
        ReadOnlySpan<byte> nameRawBytes = MemoryMarshal.Cast<char, byte>(name);

        writer.Write((ushort)(
            sizeof(ushort) + nameRawBytes.Length +
            sizeof(ushort) + sizeof(ushort)));

        writer.Write((ushort)nameRawBytes.Length);
        writer.Write(nameRawBytes);

        writer.Write(@params.Get<ushort>(UNKNOWN_PARAM_2));
        writer.Write(@params.Get<ushort>(UNKNOWN_PARAM_1));
        return true;
    }

    public static bool WriteText(StringBuilder sb, Span<byte> data)
    {
        RevrsReader reader = RevrsReader.Native(data);

        ushort rawNameLength = reader.Read<ushort>();

        sb.OpenParam(NAME_PARAM);
        sb.Append(MemoryMarshal.Cast<byte, char>(reader.ReadSpan<byte>(rawNameLength)));
        sb.CloseParam();

        sb.OpenParam(UNKNOWN_PARAM_1);
        sb.Append(reader.Read<ushort>());
        sb.CloseParam();

        sb.OpenParam(UNKNOWN_PARAM_2);
        sb.Append(reader.Read<ushort>());
        sb.CloseParam();
        return true;
    }
}
