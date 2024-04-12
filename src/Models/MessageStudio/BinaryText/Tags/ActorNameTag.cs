using MessageStudio.Formats.BinaryText.Components;
using MessageStudio.Formats.BinaryText.Extensions;
using Revrs;
using System.Runtime.InteropServices;
using System.Text;

namespace NxEditor.EpdPlugin.Models.MessageStudio.BinaryText.Tags;

public class ActorNameTag
{
    public const string NAME = "ActorName";

    private const string NAME_PARAM = "Name";
    private const string UNKNOWN_PARAM = "Unknown";

    public static bool WriteUtf16(RevrsWriter writer, in TagParams @params)
    {
        ReadOnlySpan<char> name = @params.Get<string>(NAME);
        ReadOnlySpan<byte> nameRawBytes = MemoryMarshal.Cast<char, byte>(name);

        writer.Write((ushort)(
            sizeof(ushort) + nameRawBytes.Length +
            sizeof(ushort) + sizeof(ushort)));

        writer.Write((ushort)nameRawBytes.Length);
        writer.Write(nameRawBytes);

        writer.Write(@params.Get<short>(UNKNOWN_PARAM));
        return true;
    }

    public static bool WriteText(StringBuilder sb, Span<byte> data)
    {
        RevrsReader reader = RevrsReader.Native(data);

        ushort rawNameLength = reader.Read<ushort>();

        sb.OpenParam(NAME_PARAM);
        sb.Append(MemoryMarshal.Cast<byte, char>(reader.ReadSpan<byte>(rawNameLength)));
        sb.CloseParam();

        sb.OpenParam(UNKNOWN_PARAM);
        sb.Append(reader.Read<short>());
        sb.CloseParam();

        return true;
    }
}
