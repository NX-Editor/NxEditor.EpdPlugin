using MessageStudio.Formats.BinaryText.Components;
using MessageStudio.Formats.BinaryText.Extensions;
using Revrs;
using Revrs.Extensions;
using System.Text;

namespace NxEditor.EpdPlugin.Models.MessageStudio.BinaryText.Tags;

public class FontSizeTag
{
    public const string NAME = "FontSize";

    private const string FONT_SIZE_PARAM = "Scale";

    public static bool WriteUtf16(RevrsWriter writer, in TagParams @params)
    {
        writer.Write<ushort>(sizeof(ushort));
        writer.Write(@params.Get<ushort>(FONT_SIZE_PARAM));
        return true;
    }

    public static bool WriteText(StringBuilder sb, Span<byte> data)
    {
        sb.OpenParam(FONT_SIZE_PARAM);
        sb.Append(data.Read<ushort>());
        sb.CloseParam();
        return true;
    }
}
