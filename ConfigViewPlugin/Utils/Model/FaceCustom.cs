using tsg = Tekla.Structures.Geometry3d;
namespace ConfigViewPlugin.Utils.Model
{
    public class FaceCustom
    {
        public tsg.Vector Normal { get; set; }
        public tsg.Point BasePoint { get; set; }

        public FaceCustom(tsg.Vector normal, tsg.Point basePoint)
        {
            Normal = normal;
            BasePoint = basePoint;
        }
    }
}
