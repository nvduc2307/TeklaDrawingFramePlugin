using ConfigViewPlugin.Utils.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using tsd = Tekla.Structures.Drawing;
using tsg = Tekla.Structures.Geometry3d;
using tsm = Tekla.Structures.Model;

namespace ConfigViewPlugin.Utils
{
    public static class GeometryUtils
    {
        public static tsg.Vector Reverse(this tsg.Vector vector)
        {
            return new tsg.Vector(-vector.X, -vector.Y, -vector.Z);
        }
        public static tsg.Vector Vector(this tsg.Point p1)
        {
            return new tsg.Vector(p1.X, p1.Y, p1.Z);
        }
        public static tsg.Vector Vector(this tsg.Point p1, tsg.Point p2)
        {
            var dis = p1.DistancePToP(p2);
            var x = (p2.X - p1.X) / dis;
            var y = (p2.Y - p1.Y) / dis;
            var z = (p2.Z - p1.Z) / dis;
            return new tsg.Vector(x, y, z);
        }
        public static bool IsSame(this tsg.Point p1, tsg.Point p2, double toolean = 0.0001)
        {
            var dis = p1.DistancePToP(p2);
            return dis <= toolean;
        }
        public static tsg.Point MidPoint(this tsg.Point p1, tsg.Point p2)
        {
            var minPoint = p1;
            var maxPoint = p2;
            var midPoint = new tsg.Point(
                (maxPoint.X + minPoint.X) * 0.5,
                (maxPoint.Y + minPoint.Y) * 0.5,
                (maxPoint.Z + minPoint.Z) * 0.5);
            return midPoint;
        }
        public static double DistancePToP(this tsg.Point p1, tsg.Point p2)
        {
            return Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2) + Math.Pow((p2.Z - p1.Z), 2));
        }
        public static double Distance(this tsg.Point p, FaceCustom faceCad)
        {
            var result = 0.0;
            try
            {
                var d = p.DistancePToP(faceCad.BasePoint);
                var vt = (p.Vector(faceCad.BasePoint));
                var angle = faceCad.Normal.Dot(vt) >= 0
                    ? faceCad.Normal.AngleTo(vt)
                    : faceCad.Normal.AngleTo(vt.Reverse());
                result = Math.Cos(angle) * d;
            }
            catch (Exception)
            {
            }
            return result;
        }
        public static double Distance(this tsg.Vector vt)
        {
            return Math.Sqrt(Math.Pow(vt.X, 2) + Math.Pow(vt.Y, 2) + Math.Pow(vt.Z, 2));
        }
        public static tsg.Point Rotate(this tsg.Point p1, tsg.Point centerRotate, double angleDeg)
        {
            angleDeg = angleDeg * Math.PI / 180;
            var vt = p1 - centerRotate;
            var newPoint = new tsg.Point(
                vt.X * Math.Cos(angleDeg) - vt.Y * Math.Sin(angleDeg),
                vt.X * Math.Sin(angleDeg) + vt.Y * Math.Cos(angleDeg),
                p1.Z);
            return newPoint;
        }
        public static double AngleTo(this tsg.Vector vt1, tsg.Vector vt2)
        {
            var result = 0.0;
            try
            {
                var cos = vt1.Dot(vt2) / (vt1.Distance() * vt2.Distance());
                result = Math.Acos(cos);
            }
            catch (Exception)
            {
            }
            return result;
        }
        public static tsg.Point RayPointToFace(this tsg.Point p, tsg.Vector vtRay, FaceCustom faceCad)
        {
            tsg.Point result = p;
            try
            {
                var vt = (p.Vector(faceCad.BasePoint));
                var normalFace = vt.Dot(faceCad.Normal) >= 0 ? faceCad.Normal : faceCad.Normal.Reverse();
                var angle1 = normalFace.AngleTo(vt);
                var angle2 = normalFace.AngleTo(vtRay);

                var angle1D = normalFace.AngleTo(vt) * 180 / Math.PI;
                var angle2D = normalFace.AngleTo(vtRay) * 180 / Math.PI;

                var dm = p.DistancePToP(faceCad.BasePoint);

                var dd = p.Distance(faceCad);

                var d = Math.Cos(angle1) * p.DistancePToP(faceCad.BasePoint) / Math.Cos(angle2);
                result = p + vtRay * d;
            }
            catch (Exception)
            {
                result = p;
            }
            return result;
        }

        public static tsg.Point Tranform(this tsg.Point tranformPoint, tsg.Vector vectorTranform)
        {
            return tranformPoint + vectorTranform;
        }
        public static List<tsg.Point> TransformCoordinate(
            this List<tsg.Point> points,
            tsg.CoordinateSystem pointsCoordinateSystem,
            tsm.TransformationPlane currentPlane)
        {
            var pointsTransformationPlane = new tsm.TransformationPlane(pointsCoordinateSystem);
            var results = points
                .Select(p => currentPlane.TransformationMatrixToGlobal.Transform(p))
                .ToList();
            return results;
        }
        public static List<tsg.Point> TransformPointsCoordinateToCurrentPlane(
            this List<tsg.Point> points,
            tsg.CoordinateSystem pointsCoordinateSystem,
            tsm.TransformationPlane currentPlane)
        {
            var pointsTransformationPlane = new tsm.TransformationPlane(pointsCoordinateSystem);
            var results = points
                .Select(p => currentPlane.TransformationMatrixToLocal.Transform(pointsTransformationPlane.TransformationMatrixToGlobal.Transform(p)))
                .ToList();
            return results;
        }
        public static List<tsg.Point> TransformPointsInModelToViewDrawing(
            this List<tsg.Point> points,
            tsg.CoordinateSystem pointsCoordinateSystem,
            tsm.Model cmodel,
            tsd.View partView)
        {
            var savePlane = cmodel.GetWorkPlaneHandler().GetCurrentTransformationPlane();
            cmodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new tsm.TransformationPlane());

            var pointsTransformationPlane = new tsm.TransformationPlane(pointsCoordinateSystem);
            var pointsGlobal = points
                .Select(p => pointsTransformationPlane.TransformationMatrixToGlobal.Transform(p))
                .Select(p => new tsg.Point(p.X, p.Y, 0))
                .ToList();
            var convMatrix = tsg.MatrixFactory.ToCoordinateSystem(partView.DisplayCoordinateSystem);
            var pointsDrawing = pointsGlobal
                .Select(p => convMatrix.Transform(p))
                .ToList();

            cmodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(savePlane);
            return pointsDrawing;
        }
        public static tsg.Point TransformPointsInModelToViewDrawing(
            this tsg.Point point,
            tsg.CoordinateSystem pointsCoordinateSystem,
            tsm.Model cmodel,
            tsd.View partView)
        {
            var savePlane = cmodel.GetWorkPlaneHandler().GetCurrentTransformationPlane();
            cmodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new tsm.TransformationPlane());

            var pointsTransformationPlane = new tsm.TransformationPlane(pointsCoordinateSystem);
            var pGlobal = pointsTransformationPlane.TransformationMatrixToGlobal.Transform(point);
            var convMatrix = tsg.MatrixFactory.ToCoordinateSystem(partView.DisplayCoordinateSystem);
            var pDrawing = convMatrix.Transform(pGlobal);
            cmodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(savePlane);
            return pDrawing;
        }
        public static tsg.Point Get2PHasDistanceMax(this List<tsg.Point> polygons, out tsg.Point pResult2)
        {
            //polygons la tap hop cac diem khep kin
            var lines = new List<tsg.LineSegment>();
            var pointsCount = polygons.Count;
            for (int i = 0; i < pointsCount - 1; i++)
            {
                lines.Add(new tsg.LineSegment(polygons[i], polygons[i + 1]));
            }
            var lineResults = lines
                .OrderBy(x => x.Length())
                .ToList();
            var lineSelected = pointsCount < 12
                ? lineResults.LastOrDefault()
                : lineResults[pointsCount - 2];
            pResult2 = lineSelected.Point2;
            return lineSelected.Point1;
        }
    }
}
