using FrameDrawingPlugin.Utils.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tsd = Tekla.Structures.Drawing;

namespace FrameDrawingPlugin.Actions
{
    public partial class FrameDrawingPluginAction
    {
        public void ConfigViewTop(tsd.View view)
        {
            //posigin view top
            view.Origin += VectorCustom.BaseY * 100;
        }
    }
}
