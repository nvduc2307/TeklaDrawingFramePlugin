using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Drawing;
using tsd = Tekla.Structures.Drawing;
using tsg = Tekla.Structures.Geometry3d;

namespace FrameDrawingPlugin.Utils
{
    public static class SectionViewutils
    {
        public static void CreateSectionView(
            this tsd.View view1,
            tsg.Point p1,
            tsg.Point p2,
            tsg.Point viewInsertionPoint,
            double depthUp,
            double depthDown,
            string viewAttributes,
            string sectionMarkAttributes,
            out View sectionView,
            out SectionMark sectionMark)

        {
            sectionView = null;
            sectionMark = null;
            var viewAttr = new tsd.View.ViewAttributes();
            viewAttr.LoadAttributes(viewAttributes);
            var sectionMarkAttr = new tsd.SectionMarkBase.SectionMarkAttributes();
            sectionMarkAttr.LoadAttributes(sectionMarkAttributes);
            tsd.View.CreateSectionView(
                view1, p1, p2,
                p1.MidPoint(p2),
                depthUp, depthDown, viewAttr, sectionMarkAttr,
                out sectionView,
                out sectionMark);
        }
    }
}
