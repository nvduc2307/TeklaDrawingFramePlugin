using tsd = Tekla.Structures.Drawing;

namespace ConfigViewPlugin.Utils.Model
{
    public static class TagCustom
    {
        public const string PROFILE = "PROFILE";
        public const string PANEL_NAME = "PANEL NAME";
        public const string TEXT = "TEXT";

        public static void ConfigMarkSetting(this tsd.Mark mark, tsd.ModelObject drawingModel, MarkType markType, string text = "")
        {
            mark.Attributes.Content.Clear();
            switch (markType)
            {
                case MarkType.PROFILE:
                    var contentStart = new tsd.TextElement("(");
                    var contentMid = new tsd.UserDefinedElement(ExtUDA.PROFILE);
                    var contentEnd = new tsd.TextElement(")");
                    contentStart.Font.Height = 2.0;
                    contentMid.Font.Height = 2.0;
                    contentEnd.Font.Height = 2.0;
                    mark.Attributes.Content.Add(contentStart);
                    mark.Attributes.Content.Add(contentMid);
                    mark.Attributes.Content.Add(contentEnd);
                    break;
                case MarkType.TEXT:
                    if (!string.IsNullOrEmpty(text))
                    {
                        mark.Attributes.Content.Add(new tsd.TextElement(text));
                    }
                    else
                    {
                        mark.Attributes.Content.Add(new tsd.UserDefinedElement(ExtUDA.NAME));
                    }
                    break;
                case MarkType.PANEL_NAME:
                    mark.Attributes.Content.Add(new tsd.UserDefinedElement(ExtUDA.PANEL_NAME));
                    break;
                case MarkType.PART_POSITION:
                    mark.Attributes.Content.Add(new tsd.UserDefinedElement(ExtUDA.PART_POSITION));
                    break;
            }
        }

        public static MarkType TransformTextToMarkType(this string markSettingName)
        {
            var result = MarkType.PANEL_NAME;
            switch (markSettingName)
            {
                case TagCustom.PROFILE:
                    result = MarkType.PROFILE;
                    break;
                case TagCustom.TEXT:
                    result = MarkType.TEXT;
                    break;
                case TagCustom.PANEL_NAME:
                    result = MarkType.PANEL_NAME;
                    break;
                default:
                    result = MarkType.PANEL_NAME;
                    break;
            }
            return result;
        }
        public static string TransformMarkTypeToText(this MarkType markType)
        {
            var result = "";
            switch (markType)
            {
                case MarkType.PROFILE:
                    result = TagCustom.PROFILE;
                    break;
                case MarkType.TEXT:
                    result = TagCustom.TEXT;
                    break;
                case MarkType.PANEL_NAME:
                    result = TagCustom.PANEL_NAME;
                    break;
            }
            return result;
        }

        public static LocationMark TransformIntToLocationMark(this int locationMark)
        {
            var result = LocationMark.MiddlePart;
            switch (locationMark)
            {
                case 0:
                    result = LocationMark.TopPart;
                    break;
                case 1:
                    result = LocationMark.MiddlePart;
                    break;
                case 2:
                    result = LocationMark.BottomPart;
                    break;
            }
            return result;
        }
        public static int TransformLocationMarkToInt(this LocationMark locationMark)
        {
            var result = 1;
            switch (locationMark)
            {
                case LocationMark.TopPart:
                    result = 0;
                    break;
                case LocationMark.MiddlePart:
                    result = 1;
                    break;
                case LocationMark.BottomPart:
                    result = 2;
                    break;
            }
            return (int)result;
        }
    }
    public enum LocationMark
    {
        TopPart,
        MiddlePart,
        BottomPart,
    }
    public enum MarkType
    {
        PROFILE,
        PANEL_NAME,
        TEXT,
        PART_POSITION
    }
}
