using ConfigViewPlugin.Utils;
using ConfigViewPlugin.Utils.Compares;
using ConfigViewPlugin.Utils.Messages;
using ConfigViewPlugin.Utils.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Tekla.Structures.Drawing.Tools;
using Tekla.Structures.Model;
using Tekla.Structures.Plugins;
using tsd = Tekla.Structures.Drawing;
using tsdui = Tekla.Structures.Drawing.UI;
using tsg = Tekla.Structures.Geometry3d;
using tsm = Tekla.Structures.Model;
using tss = Tekla.Structures.Solid;

namespace ConfigViewPlugin
{
    [Plugin("Config View")]
    [PluginUserInterface("ConfigViewPlugin.ConfigViewPluginView")]
    public class ConfigViewPlugin : DrawingPluginBase
    {
        private tsm.Model _model;
        private tsdui.Picker _picker;
        private tsd.DrawingHandler _dHandler;
        private tsdui.DrawingObjectSelector _selector;
        private double _distanceDim;
        public ConfigViewPlugin()
        {
            _model = new tsm.Model();
            _dHandler = new tsd.DrawingHandler();
            _picker = _dHandler.GetPicker();
            _selector = _dHandler.GetDrawingObjectSelector();
            _distanceDim = 100;
        }
        public override List<InputDefinition> DefineInput()
        {
            var result = new List<InputDefinition>();
            var isDo = true;
            do
            {
                try
                {
                    var tup = _picker.PickObject("Pick point");
                    if (!(tup.Item2 is tsd.View view))
                    {
                        IO.ShowWarning("Obj must be a view");
                        continue;
                    }
                    result.Add(InputDefinitionFactory.CreateInputDefinition(tup));
                    isDo = false;
                }
                catch (System.Exception)
                {
                    isDo = false;
                }
            } while (isDo);
            return result;
        }
        public override bool Run(List<InputDefinition> inputs)
        {
            try
            {
                var viewBase = InputDefinitionFactory.GetView(inputs[0]);
                if (viewBase == null) return false;
                var view = viewBase as tsd.View;

                if (view == null) return false;
                var drawing = viewBase.GetDrawing();
                //========================
                switch (view.ViewType)
                {
                    case tsd.View.ViewTypes.FrontView:
                        DimViewHorizontal(view);
                        DimViewVerticalFront(view);
                        TagPartViewFront(view);
                        break;
                    case tsd.View.ViewTypes.TopView:
                        DimViewHorizontal(view);
                        DimViewVerticalTop(view);
                        TagPartViewTop(view);
                        break;
                }

                //========================
                drawing.CommitChanges();
            }
            catch (Exception)
            {
            }
            return true;
        }
        private void DimViewVerticalFront(tsd.View view)
        {
            var vtx = VectorCustom.BaseX;
            var vty = VectorCustom.BaseY;
            var drawingObjEnum = view.GetObjects();
            var drawingObjs = new List<tsd.DrawingObject>();
            var endPart1s = view.GetDrawingPartElements(_model, StringDefine.EndPart1);
            var psdim = endPart1s.GetPointDim(_model, vtx);
            view.InstallDim(psdim, vty, vtx, _distanceDim, true);
        }
        private void DimViewVerticalTop(tsd.View view)
        {
            var vtx = VectorCustom.BaseX;
            var vty = VectorCustom.BaseY;
            var drawingObjEnum = view.GetObjects();
            var drawingObjs = new List<tsd.DrawingObject>();
            var endPart1s = view.GetDrawingPartElements(_model, StringDefine.EndPart1);
            var psdim = endPart1s.GetPointDim(_model, vtx);
            view.InstallDim(psdim, vty, vtx, _distanceDim, true);
        }
        private void DimViewHorizontal(tsd.View view)
        {
            var vtx = VectorCustom.BaseX;
            var vty = VectorCustom.BaseY;
            var xaGos = view.GetDrawingPartElements(_model, StringDefine.XG);
            var giaCuong1 = view.GetDrawingPartElements(_model, StringDefine.GiaCuong1);
            var giaCuong2 = view.GetDrawingPartElements(_model, StringDefine.GiaCuong2);
            var endPart1s = view.GetDrawingPartElements(_model, StringDefine.EndPart1);
            var partDims = xaGos
                .Concat(giaCuong1)
                .Concat(giaCuong2)
                .Concat(endPart1s)
                .ToList();
            var psdim = partDims
                .GetPointDim(_model, vty)
                .OrderBy(x=>x.Vector().Dot(vtx))
                .ToList();
            view.InstallDim(psdim, vtx, vty, _distanceDim * 2.5, true);
            view.InstallDim(
                new List<tsg.Point>() { psdim.FirstOrDefault(), psdim.LastOrDefault() }, 
                vtx, vty, _distanceDim * 3.5);
        }
        private void TagPartViewFront(tsd.View view)
        {
            #region tag xagos
            var xagos = view.GetDrawingPartElements(_model, StringDefine.XG);
            foreach (var xag in xagos)
            {
                var mPart = xag.GetMObjFormDObj(_model);
                var solid = mPart.GetSolid();
                var mid = solid.MinimumPoint.MidPoint(solid.MaximumPoint);
                var position = 
                    mid
                    + VectorCustom.BaseY * 50
                    - VectorCustom.BaseX * 200;
                xag.CreatePartMark(position, StringDefine.MarkStyle);
            }
            #endregion
            #region tag end part
            var endPart1s = view.GetDrawingPartElements(_model, StringDefine.EndPart1);
            var endPart2s = view.GetDrawingPartElements(_model, StringDefine.EndPart2);
            foreach (var endPart in endPart1s)
            {
                var mPart = endPart.GetMObjFormDObj(_model);
                var solid = mPart.GetSolid();
                var mid = solid.MinimumPoint.MidPoint(solid.MaximumPoint);
                var position =
                    mid
                    - VectorCustom.BaseY * 250
                    - VectorCustom.BaseX * 250;
                endPart.CreatePartMark(position, StringDefine.MarkStyle);
            }
            foreach (var endPart in endPart2s)
            {
                var mPart = endPart.GetMObjFormDObj(_model);
                var solid = mPart.GetSolid();
                var mid = solid.MinimumPoint.MidPoint(solid.MaximumPoint);
                var position =
                    mid
                    - VectorCustom.BaseY * 10
                    - VectorCustom.BaseX * 450;
                endPart.CreatePartMark(position, StringDefine.MarkStyle);
            }
            #endregion
            #region tag gia cuong
            var giaCuong1 = view.GetDrawingPartElements(_model, StringDefine.GiaCuong1);
            var giaCuong2 = view.GetDrawingPartElements(_model, StringDefine.GiaCuong2);
            var giaCuong3 = view.GetDrawingPartElements(_model, StringDefine.GiaCuong3);
            var giaCuong4 = view.GetDrawingPartElements(_model, StringDefine.GiaCuong4);
            foreach (var part in giaCuong1)
            {
                var mPart = part.GetMObjFormDObj(_model);
                var solid = mPart.GetSolid();
                var mid = solid.MinimumPoint.MidPoint(solid.MaximumPoint);
                var position =
                    mid
                    + VectorCustom.BaseY * 50
                    - VectorCustom.BaseX * 200;
                part.CreatePartMark(position, StringDefine.MarkStyle);
                break;
            }
            foreach (var part in giaCuong2)
            {
                var mPart = part.GetMObjFormDObj(_model);
                var solid = mPart.GetSolid();
                var mid = solid.MinimumPoint.MidPoint(solid.MaximumPoint);
                var position =
                    mid
                    + VectorCustom.BaseY * 50
                    + VectorCustom.BaseX * 200;
                part.CreatePartMark(position, StringDefine.MarkStyle);
                break;
            }
            foreach (var part in giaCuong3.Concat(giaCuong4))
            {
                var mPart = part.GetMObjFormDObj(_model);
                var solid = mPart.GetSolid();
                var mid = solid.MinimumPoint.MidPoint(solid.MaximumPoint);
                var position =
                    mid
                    - VectorCustom.BaseY * 50
                    - VectorCustom.BaseX * 300;
                part.CreatePartMark(position, StringDefine.MarkStyle);
            }
            #endregion
            #region tag main part
            #endregion
        }
        private void TagPartViewTop(tsd.View view)
        {
            #region tag xagos
            var xagos = view.GetDrawingPartElements(_model, StringDefine.XG);
            foreach (var xag in xagos)
            {
                var mPart = xag.GetMObjFormDObj(_model);
                var solid = mPart.GetSolid();
                var mid = solid.MinimumPoint.MidPoint(solid.MaximumPoint);
                var position =
                    mid
                    - VectorCustom.BaseY * 150
                    - VectorCustom.BaseX * 200;
                xag.CreatePartMark(position, StringDefine.MarkStyle);
            }
            #endregion
            #region tag end part
            var endPart1s = view.GetDrawingPartElements(_model, StringDefine.EndPart1);
            var endPart2s = view.GetDrawingPartElements(_model, StringDefine.EndPart2);
            foreach (var endPart in endPart1s)
            {
                var mPart = endPart.GetMObjFormDObj(_model);
                var solid = mPart.GetSolid();
                var mid = solid.MinimumPoint.MidPoint(solid.MaximumPoint);
                var position =
                    mid
                    - VectorCustom.BaseY * 250
                    - VectorCustom.BaseX * 250;
                endPart.CreatePartMark(position, StringDefine.MarkStyle);
            }
            foreach (var endPart in endPart2s)
            {
                var mPart = endPart.GetMObjFormDObj(_model);
                var solid = mPart.GetSolid();
                var mid = solid.MinimumPoint.MidPoint(solid.MaximumPoint);
                var position =
                    mid
                    - VectorCustom.BaseY * 200
                    + VectorCustom.BaseX * 100;
                endPart.CreatePartMark(position, StringDefine.MarkStyle);
            }
            #endregion
            #region tag gia cuong
            var giaCuong1 = view.GetDrawingPartElements(_model, StringDefine.GiaCuong1);
            var giaCuong2 = view.GetDrawingPartElements(_model, StringDefine.GiaCuong2);
            var giaCuong3 = view.GetDrawingPartElements(_model, StringDefine.GiaCuong3);
            var giaCuong4 = view.GetDrawingPartElements(_model, StringDefine.GiaCuong4);
            foreach (var part in giaCuong1)
            {
                var mPart = part.GetMObjFormDObj(_model);
                var solid = mPart.GetSolid();
                var mid = solid.MinimumPoint.MidPoint(solid.MaximumPoint);
                var position =
                    mid
                    - VectorCustom.BaseY * 200
                    - VectorCustom.BaseX * 200;
                part.CreatePartMark(position, StringDefine.MarkStyle);
                break;
            }
            foreach (var part in giaCuong2)
            {
                var mPart = part.GetMObjFormDObj(_model);
                var solid = mPart.GetSolid();
                var mid = solid.MinimumPoint.MidPoint(solid.MaximumPoint);
                var position =
                    mid
                    - VectorCustom.BaseY * 270
                    + VectorCustom.BaseX * 250;
                part.CreatePartMark(position, StringDefine.MarkStyle);
                break;
            }
            foreach (var part in giaCuong3)
            {
                var mPart = part.GetMObjFormDObj(_model);
                var solid = mPart.GetSolid();
                var mid = solid.MinimumPoint.MidPoint(solid.MaximumPoint);
                var position =
                    mid
                    - VectorCustom.BaseY * 300
                    + VectorCustom.BaseX * 150;
                part.CreatePartMark(position, StringDefine.MarkStyle);
                break;
            }
            foreach (var part in giaCuong4)
            {
                var mPart = part.GetMObjFormDObj(_model);
                var solid = mPart.GetSolid();
                var mid = solid.MinimumPoint.MidPoint(solid.MaximumPoint);
                var position =
                    mid
                    - VectorCustom.BaseY * 250
                    - VectorCustom.BaseX * 250;
                part.CreatePartMark(position, StringDefine.MarkStyle);
                break;
            }
            #endregion
        }
    }
}
