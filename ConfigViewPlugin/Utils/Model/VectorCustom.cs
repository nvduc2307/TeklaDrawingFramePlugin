using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tsg = Tekla.Structures.Geometry3d;

namespace ConfigViewPlugin.Utils.Model
{
    public class VectorCustom
    {
        public static tsg.Vector BaseX { get => new tsg.Vector(1, 0, 0); }
        public static tsg.Vector BaseY { get => new tsg.Vector(0, 1, 0); }
        public static tsg.Vector BaseZ { get => new tsg.Vector(0, 0, 1); }
    }
}
