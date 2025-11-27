using ConfigViewPlugin.Utils.Model;
using static Tekla.Structures.Drawing.Mark;
using tsd = Tekla.Structures.Drawing;
using tsg = Tekla.Structures.Geometry3d;
namespace ConfigViewPlugin.Utils
{
    public static class TagUtils
    {
        public static void CreatePartMark(this tsd.ModelObject modelObject, tsg.Point location, string attr)
        {
            var parkMark = new tsd.Mark(modelObject);
            parkMark.InsertionPoint = location;
            parkMark.ConfigMarkSetting(modelObject, MarkType.PROFILE, "");
            parkMark.Insert();
        }
    }
}
