using MessageStudio.Formats.BinaryText.Components;
using MessageStudio.Formats.BinaryText.Extensions;
using Revrs;
using System.Runtime.InteropServices;
using System.Text;

namespace NxEditor.EpdPlugin.Models.MessageStudio.BinaryText.Tags;

public class AnimationTag
{
    public const string NAME = "Anim";

    public const string TYPE_PARAM = "Type";

    public static bool WriteUtf16(RevrsWriter writer, in TagParams @params)
    {
        ReadOnlySpan<char> name = @params.Get<string>(TYPE_PARAM);
        ReadOnlySpan<byte> nameRawBytes = MemoryMarshal.Cast<char, byte>(name);

        writer.Write((ushort)(
            sizeof(ushort) + nameRawBytes.Length
        ));

        writer.Write((ushort)nameRawBytes.Length);
        writer.Write(nameRawBytes);

        return true;
    }

    public static bool WriteText(StringBuilder sb, Span<byte> data)
    {
        RevrsReader reader = RevrsReader.Native(data);

        ushort rawNameLength = reader.Read<ushort>();

        sb.OpenParam(TYPE_PARAM);
        sb.Append(MemoryMarshal.Cast<byte, char>(reader.ReadSpan<byte>(rawNameLength)));
        sb.CloseParam();

        return true;
    }
}
