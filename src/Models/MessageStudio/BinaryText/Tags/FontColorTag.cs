using MessageStudio.Formats.BinaryText.Components;
using Revrs.Extensions;
using Revrs;
using System.Text;
using MessageStudio.Formats.BinaryText.Extensions;

namespace NxEditor.EpdPlugin.Models.MessageStudio.BinaryText.Tags;

public class FontColorTag
{
    public enum FontColor : ushort
    {
        White = 0xFFFF,
        Red = 0x0000
    }

    public const string NAME = "FontColor";

    private const string FONT_COLOR_PARAM = "Color";

    public static bool WriteUtf16(RevrsWriter writer, in TagParams @params)
    {
        writer.Write<ushort>(sizeof(FontColor));
        writer.Write(@params.GetEnum<FontColor>(FONT_COLOR_PARAM));
        return true;
    }

    public static bool WriteText(StringBuilder sb, Span<byte> data)
    {
        sb.OpenParam(FONT_COLOR_PARAM);
        sb.Append(data.Read<FontColor>());
        sb.CloseParam();
        return true;
    }
}
