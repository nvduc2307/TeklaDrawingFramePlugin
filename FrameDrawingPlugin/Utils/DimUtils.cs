using FrameDrawingPlugin.Utils.Compares;
using FrameDrawingPlugin.Utils.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using tsd = Tekla.Structures.Drawing;
using tsg = Tekla.Structures.Geometry3d;
using tsm = Tekla.Structures.Model;
using tss = Tekla.Structures.Solid;

namespace FrameDrawingPlugin.Utils
{
    public static class DimUtils
    {
        public static void InstallDim(
            this tsd.View view,
            List<tsg.Point> ps,
            tsg.Vector vtDimDir,
            tsg.Vector vtDimNormal,
            double distance,
            bool isRayFace = false)
        {
            ps = ps
                .Distinct(new ComparePoint())
                .OrderBy(x => x.Vector().Dot(vtDimDir))
                .ToList();
            if (isRayFace)
            {
                var pmax = ps.OrderByDescending(x => x.Vector(vtDimNormal)).FirstOrDefault();
                var f = new FaceCustom(vtDimNormal, pmax);
                ps = ps
                    .Select(x => x.RayPointToFace(f.Normal, f))
                    .Where(x => !(double.IsNaN(x.X) || double.IsNaN(x.Y) || double.IsNaN(x.Z)))
                    .ToList();
            }
            if (ps.Any())
            {

                var q = ps.Count;
                for (var i = 0; i < q - 1; i++)
                {
                    var p1 = ps[i];
                    var p2 = ps[i + 1];
                    var dim = new tsd.StraightDimension(
                        view,
                        p1, p2,
                        vtDimNormal,
                        distance);
                    dim.Insert();
                }
            }
        }
        public static List<tsg.Point> GetPointDim(
            this List<tsd.Part> partDims,
            tsm.Model model, tsg.Vector vt)
        {
            var results = new List<tsg.Point>();
            foreach (var part in partDims)
            {
                var mPart = part.GetMObjFormDObj(model);
                var solid = mPart.GetSolid();
                var edgeEnumerator = solid.GetEdgeEnumerator();
                while (edgeEnumerator.MoveNext())
                {
                    var edge = edgeEnumerator.Current as tss.Edge;
                    if (edge == null) continue;
                    var dis = edge.StartPoint.DistancePToP(edge.EndPoint);
                    var vtCheck = new tsg.Vector(
                        Math.Round(edge.EndPoint.X / dis - edge.StartPoint.X / dis, 0),
                        Math.Round(edge.EndPoint.Y / dis - edge.StartPoint.Y / dis, 0),
                        Math.Round(edge.EndPoint.Z / dis - edge.StartPoint.Z / dis, 0));

                    if (Math.Abs(vtCheck.Dot(vt)) == 1)
                    {
                        if (edge.EndPoint.Vector().Dot(vt) >= edge.StartPoint.Vector().Dot(vt))
                            results.Add(edge.EndPoint);
                        else
                            results.Add(edge.StartPoint);
                    }
                }
            }
            return results.Distinct(new ComparePoint()).ToList();
        }
    }
}
