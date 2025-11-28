using ConfigViewPlugin.Utils.Compares;
using ConfigViewPlugin.Utils.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using tsd = Tekla.Structures.Drawing;
using tsg = Tekla.Structures.Geometry3d;
using tsm = Tekla.Structures.Model;
using tss = Tekla.Structures.Solid;

namespace ConfigViewPlugin.Utils
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
            if (!ps.Any()) return;
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
                    var vtCheck = edge.StartPoint.Vector(edge.EndPoint);
                    var dot = Math.Round(Math.Abs(vtCheck.Dot(vt)), 0);
                    if (dot == 1)
                    {
                        if (edge.EndPoint.Vector().Dot(vt) >= edge.StartPoint.Vector().Dot(vt))
                            results.Add(edge.EndPoint);
                        else
                            results.Add(edge.StartPoint);
                    }
                }
            }
            results = results.Distinct(new ComparePoint()).ToList();
            return results;
        }

        public static List<tsg.Point> GetPointDim(
            this List<tsd.Part> partDims,
            tsd.View view,
            tsm.Model model, 
            tsg.Vector vtDim)
        {
            var vtDimNor = vtDim.Cross(VectorCustom.BaseZ);
            
            var fSection = new tsg.GeometricPlane(view.Origin, VectorCustom.BaseZ);
            var results = new List<tsg.Point>();
            foreach (var part in partDims)
            {
                var mPart = part.GetMObjFormDObj(model);
                var ps = mPart.GetPointsIntersect(fSection);
                results.AddRange(ps);
            }
            results = results
                .Where(x => x != null).ToList()
                .OrderBy(x=>x.Vector().Dot(vtDimNor))
                .ToList();
            var fDim = new FaceCustom(vtDimNor, results.LastOrDefault());
            return results
                .Where(x=>x!=null)
                .Select(x=>x.RayPointToFace(fDim.Normal, fDim))
                .Distinct(new ComparePoint())
                .OrderBy(x=>x.Vector().Dot(vtDim))
                .Where(x => !(double.IsNaN(x.X)|| double.IsNaN(x.Y)|| double.IsNaN(x.Z)))
                .ToList();
        }
    }
}
