using Tekla.Structures.Drawing;
using tsd = Tekla.Structures.Drawing;
using tsm = Tekla.Structures.Model;

namespace FrameDrawingPlugin.Utils
{
    public static class AssemblyUtils
    {
        public static bool HasDrawing(
            this tsd.DrawingHandler dHandler,
            tsm.Assembly assembly, 
            out AssemblyDrawing drawing)
        {
            drawing = null;
            if(assembly == null) return false;
            var position = string.Empty;
            assembly.GetReportProperty(nameof(AssemblyProperties.ASSEMBLY_POS), ref position);
            if (position == string.Empty)
                return false;
            var drawings = dHandler.GetDrawings();
            foreach (var d in drawings)
            {
                if (!(d is tsd.AssemblyDrawing assd)) continue;
                var mark = assd.Mark.Replace("[", "").Replace("]", "").Replace(".", "");
                if (mark == position)
                {
                    drawing = assd;
                    return true;
                }    
            }
            return false;
        }
    }
}
