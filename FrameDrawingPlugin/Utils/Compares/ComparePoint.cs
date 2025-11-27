using System.Collections.Generic;
using Tekla.Structures.Geometry3d;
using tsg = Tekla.Structures.Geometry3d;

namespace FrameDrawingPlugin.Utils.Compares
{
    public class ComparePoint : IEqualityComparer<tsg.Point>
    {
        public bool Equals(Point x, Point y)
        {
            return x.IsSame(y);
        }

        public int GetHashCode(Point obj)
        {
            return 0;
        }
    }
}
