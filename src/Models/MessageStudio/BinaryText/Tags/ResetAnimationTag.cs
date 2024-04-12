using MessageStudio.Formats.BinaryText.Components;
using MessageStudio.Formats.BinaryText.Extensions;
using Revrs;
using Revrs.Extensions;
using System.Text;

namespace NxEditor.EpdPlugin.Models.MessageStudio.BinaryText.Tags;

public class ResetAnimationTag
{
    public const string NAME = "ResetAnim";

    public const string ARG_PARAM = "Arg";

    public static bool WriteUtf16(RevrsWriter writer, in TagParams @params)
    {
        writer.Write<ushort>(sizeof(short));
        writer.Write(@params.Get<short>(ARG_PARAM));
        return true;
    }

    public static bool WriteText(StringBuilder sb, Span<byte> data)
    {
        sb.OpenParam(ARG_PARAM);
        sb.Append(data.Read<short>());
        sb.CloseParam();
        return true;
    }
}
