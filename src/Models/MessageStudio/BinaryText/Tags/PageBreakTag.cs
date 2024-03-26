using Revrs;

namespace NxEditor.EpdPlugin.Models.MessageStudio.BinaryText.Tags;

public class PageBreakTag
{
    public const string NAME = "Break";

    public static bool WriteUtf16(RevrsWriter writer)
    {
        writer.Write<ushort>(0);
        return true;
    }
}
