using ConfigViewPlugin.Utils.Model;
using System.Collections.Generic;
using Tekla.Structures.Geometry3d;
using tsd = Tekla.Structures.Drawing;
using tsm = Tekla.Structures.Model;
using tss = Tekla.Structures.Solid;
using tsg = Tekla.Structures.Geometry3d;
namespace ConfigViewPlugin.Utils
{
    public static class PartUtils
    {
        public static bool IsDPart(this tsd.DrawingObject dObj)
        {
            return dObj.GetType().Equals(typeof(tsd.Part));
        }
        public static bool IsMBeam(this tsm.Part mPart, tsm.Beam.BeamTypeEnum beamTypeEnum)
        {
            if (mPart is tsm.Beam)
            {
                var beam = mPart as tsm.Beam;
                return beam.Type == beamTypeEnum;
            }
            else
            {
                return false;
            }
        }
        public static bool IsMContourPlate(this tsm.Part mPart, tsm.ContourPlate.ContourPlateTypeEnum contourPlateTypeEnum)
        {
            if (mPart is tsm.ContourPlate)
            {
                var contour = mPart as tsm.ContourPlate;
                return contour.Type == contourPlateTypeEnum;
            }
            else
            {
                return false;
            }
        }
        public static tsd.Part GetDObjFormMObj(this tsm.Part mPart, tsd.ViewBase viewBase)
        {
            var dObjEnum = viewBase.GetModelObjects(mPart.Identifier);
            tsd.Part dObj = null;
            while (dObjEnum.MoveNext())
            {
                if (dObjEnum.Current != null)
                {
                    dObj = dObjEnum.Current as tsd.Part;
                }
            }
            return dObj;
        }
        public static tsm.Part GetMObjFormDObj(this tsd.Part dPart, tsm.Model cmodel)
        {
            var mObj = cmodel.SelectModelObject(dPart.ModelIdentifier) as tsm.Part;
            return mObj;
        }
        public static tsd.Part GetDObjSelected(this tsd.DrawingHandler dhandle)
        {
            var objS = dhandle.GetDrawingObjectSelector().GetSelected();
            tsd.Part result = null;
            while (objS.MoveNext())
            {
                if (objS.Current is tsd.Part part)
                {
                    result = part;
                    break;
                }
            }
            return result;
        }
        public static List<tsd.Part> GetDrawingPartElements(this tsd.View view, tsm.Model model, string namePartGroup = "")
        {
            var vtx = VectorCustom.BaseX;
            var vty = VectorCustom.BaseY;
            var drawingObjEnum = view.GetObjects();
            var result = new List<tsd.Part>();
            while (drawingObjEnum.MoveNext())
            {
                var obj = drawingObjEnum.Current;
                if (!(obj is tsd.Part dPart)) continue;
                if (namePartGroup == string.Empty)
                    result.Add(dPart);
                else
                {
                    var mPart = dPart.GetMObjFormDObj(model);
                    if (mPart.Name == namePartGroup)
                        result.Add(dPart);
                }
            }
            return result;
        }
        public static List<Point> GetPointsIntersect(this tsm.Part part, GeometricPlane sectionPlane)
        {
            var result = new List<Point>();
            var solid = part.GetSolid();
            var edgeEnum = solid.GetEdgeEnumerator();
            while (edgeEnum.MoveNext())
            {
                var obj = edgeEnum.Current;
                if (obj is tss.Edge edge)
                {
                    var seg = new LineSegment(edge.StartPoint, edge.EndPoint);
                    var intersection = Intersection.LineSegmentToPlane(seg, sectionPlane);
                    result.Add(intersection);
                }
            }
            return result;
        }
        public static bool isExistInView(this tsm.Part part, tsd.View view)
        {
            var solid = part.GetSolid();
            var edgeEnum = solid.GetEdgeEnumerator();
            var sectionPlane = new tsg.GeometricPlane(view.Origin, VectorCustom.BaseZ);
            while (edgeEnum.MoveNext())
            {
                var obj = edgeEnum.Current;
                if (obj is tss.Edge edge)
                {
                    var seg = new LineSegment(edge.StartPoint, edge.EndPoint);
                    var intersection = Intersection.LineSegmentToPlane(seg, sectionPlane);
                    if (intersection == null)
                        continue;
                    if (double.IsNaN(intersection.X))
                        continue;
                    if (double.IsNaN(intersection.Y))
                        continue;
                    if (double.IsNaN(intersection.Z))
                        continue;
                    return true;
                }
            }
            return false;
        }
    }
}
