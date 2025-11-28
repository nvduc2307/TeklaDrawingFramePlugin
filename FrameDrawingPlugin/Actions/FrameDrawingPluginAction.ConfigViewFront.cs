using FrameDrawingPlugin.Utils;
using FrameDrawingPlugin.Utils.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Model;
using tsd = Tekla.Structures.Drawing;
using tsg = Tekla.Structures.Geometry3d;

namespace FrameDrawingPlugin.Actions
{
    public partial class FrameDrawingPluginAction
    {
        public void ConfigViewFront(tsd.View view)
        {
            //posigin view front
            view.Origin += VectorCustom.BaseY * 50;
            CreateSectionViewOnViewFront(view);
        }
        private void CreateSectionViewOnViewFront(tsd.View view)
        {
            var extent = 400;
            var savePlane = _model.GetWorkPlaneHandler().GetCurrentTransformationPlane();
            _model.GetWorkPlaneHandler()
                .SetCurrentTransformationPlane(
                new TransformationPlane(view.DisplayCoordinateSystem));
            var giacuong1 = view.GetDrawingPartElements(_model, StringDefine.GiaCuong1);

            var viewOrigin = view.Origin;
            var viewHeight = view.Height;
            var viewWidth = view.Width;
            var xaGos = view.GetDrawingPartElements(_model, StringDefine.XG)
                .Concat(new List<tsd.Part>() { giacuong1.FirstOrDefault()})
                .OrderBy(x=>
                {
                    var mPart = x.GetMObjFormDObj(_model);
                    var solid = mPart.GetSolid();
                    var mid = solid.MinimumPoint.MidPoint(solid.MaximumPoint);
                    return mid.Vector().Dot(VectorCustom.BaseX);
                }).ToList();
            var web = view.GetDrawingPartElements(_model, StringDefine.Web).FirstOrDefault()
                .GetMObjFormDObj(_model);
            var webSolid = web.GetSolid();
            var webMid = webSolid.MinimumPoint
                .MidPoint(webSolid.MaximumPoint);
            var fSection = new FaceCustom(VectorCustom.BaseX, webMid);
            var fLong = new FaceCustom(VectorCustom.BaseY, webMid);
            var webMin = webSolid.MinimumPoint
                .RayPointToFace(fSection.Normal, fSection);
            var webMax = webSolid.MaximumPoint
                .RayPointToFace(fSection.Normal, fSection);
            var webHeight = Math.Round(webMax.DistancePToP(webMin), 0) + extent;
            var viewSections = new List<tsd.View>();
            foreach (var xg in xaGos)
            {
                var index = xaGos.IndexOf(xg);
                if (index < 1) continue;
                if (index > 7) continue;
                var mPart = xg.GetMObjFormDObj(_model);
                var solid = mPart.GetSolid();
                var mid = solid.MinimumPoint.MidPoint(solid.MaximumPoint)
                    .RayPointToFace(fLong.Normal, fLong);
                var p1 = mid + VectorCustom.BaseY * webHeight * 0.5;
                var p2 = mid - VectorCustom.BaseY * webHeight * 0.5;

                view.CreateSectionView(
                    p1, p2, 
                    p1.MidPoint(p2) - VectorCustom.BaseX * 2, 
                    50, 
                    50, 
                    StringDefine.Template_View_Section, 
                    StringDefine.SectionMarkStyle,
                    out tsd.View viewSection, out tsd.SectionMark sectionMark);
                viewSections.Add(viewSection);
            }
            _model.GetWorkPlaneHandler()
                .SetCurrentTransformationPlane(savePlane);
            var width = 0.0;
            foreach (var viewSection in viewSections)
            {
                var index = viewSections.IndexOf(viewSection) + 1;
                var pos = viewOrigin
                    - VectorCustom.BaseY * (100 + viewHeight * 0.5)
                    + VectorCustom.BaseX * (width);
                width = width + viewSection.Width + 50;
                viewSection.Origin = pos;
                viewSection.Modify();
            }
        }
    }
}
