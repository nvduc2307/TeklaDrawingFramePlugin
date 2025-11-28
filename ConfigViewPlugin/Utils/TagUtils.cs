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
            var view = modelObject.GetView();
            var extent = 100;
            var parkMark = new tsd.Mark(modelObject);
            var p_along_mark_1 = location;
            var p_along_mark_2 = location 
                + VectorCustom.BaseY * extent
                - VectorCustom.BaseX * extent;
            var placingBase = new tsd.AlongLinePlacing(p_along_mark_1, p_along_mark_2);
            parkMark.InsertionPoint = location;
            //parkMark.ConfigMarkSetting(modelObject, MarkType.PART_POSITION, "");
            parkMark.Insert();
        }
    }
}
