using FrameDrawingPlugin.Utils;
using FrameDrawingPlugin.Utils.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tsd = Tekla.Structures.Drawing;
using tsg = Tekla.Structures.Geometry3d;

namespace FrameDrawingPlugin.Actions
{
    public partial class FrameDrawingPluginAction
    {
        public void ConfigViewFront(tsd.View view)
        {
            var drawing = view.GetDrawing();
            var sheet = drawing.GetSheet();
        }
    }
}
