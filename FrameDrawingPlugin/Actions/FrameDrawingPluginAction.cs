using FrameDrawingPlugin.Utils;
using FrameDrawingPlugin.Utils.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Drawing;
using tsd = Tekla.Structures.Drawing;
using tsm = Tekla.Structures.Model;
using tsg = Tekla.Structures.Geometry3d;
using tsdui = Tekla.Structures.Drawing.UI;

namespace FrameDrawingPlugin.Actions
{
    public partial class FrameDrawingPluginAction
    {
        private double _distanceDim;
        private tsm.Model _model;
        private tsd.DrawingHandler _dHandler;
        public FrameDrawingPluginAction(tsm.Model model, tsd.DrawingHandler dHandler)
        {
            _model = model;
            _dHandler = dHandler;
            _distanceDim = 100;
        }

        public void Execute(tsm.Assembly assembly)
        {
            var hasDrawing = _dHandler.HasDrawing(assembly, out tsd.AssemblyDrawing drawing);
            #region create drawing
            if (hasDrawing)
                _dHandler.SetActiveDrawing(drawing, true);
            else
            {
                drawing = new tsd.AssemblyDrawing(assembly.Identifier, StringDefine.Template_Drawing);
                drawing.Insert();
                _dHandler.SetActiveDrawing(drawing, true, true);
            }
            #endregion
            #region config view
            if (!hasDrawing)
            {
                var sheets = drawing.GetSheet();
                var sheetViews = sheets.GetViews();
                while (sheetViews.MoveNext())
                {
                    var v = sheetViews.Current as tsd.View;
                    var viewType = v.ViewType;
                    var origin = v.Origin;
                    switch (viewType)
                    {
                        case tsd.View.ViewTypes.FrontView:
                            ConfigViewFront(v);
                            break;
                        case tsd.View.ViewTypes.TopView:
                            ConfigViewTop(v);
                            break;
                    }
                    v.Modify();
                    drawing.CommitChanges();
                }
            }
            #endregion
        }
    }
}
