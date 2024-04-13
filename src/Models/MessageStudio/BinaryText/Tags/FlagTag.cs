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
    private const string INDEX_PARAM = "Index";
    private const string ALT_ARRAY_PARAM = "AltArray";

    public static bool WriteUtf16(RevrsWriter writer, in TagParams @params)
    {
        ReadOnlySpan<char> name = @params.Get<string>(NAME_PARAM);
        ReadOnlySpan<byte> nameRawBytes = MemoryMarshal.Cast<char, byte>(name);

        writer.Write((ushort)(
            sizeof(ushort) + nameRawBytes.Length + sizeof(short) + sizeof(short)
        ));

        writer.Write((ushort)nameRawBytes.Length);
        writer.Write(nameRawBytes);

        writer.Write(@params.Get<short>(INDEX_PARAM));
        writer.Write(
            Convert.ToInt16(@params.Get<bool>(ALT_ARRAY_PARAM))
        );

        return true;
    }

    public static bool WriteText(StringBuilder sb, Span<byte> data)
    {
        RevrsReader reader = RevrsReader.Native(data);

        ushort rawNameLength = reader.Read<ushort>();

        sb.OpenParam(NAME_PARAM);
        sb.Append(MemoryMarshal.Cast<byte, char>(reader.ReadSpan<byte>(rawNameLength)));
        sb.CloseParam();

        sb.OpenParam(INDEX_PARAM);
        sb.Append(reader.Read<short>());
        sb.CloseParam();

        sb.OpenParam(ALT_ARRAY_PARAM);
        sb.Append(reader.Read<short>() > 0);
        sb.CloseParam();

        return true;
    }
}
