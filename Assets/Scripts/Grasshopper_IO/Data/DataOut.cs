using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Grasshopper_IO.Data
{
    public class DataOut
    {
        public float LocX { get; set; }
        public float LocY { get; set; }
        public float LocZ { get; set; }
        public float RotX { get; set; }
        public float RotY { get; set; }
        public int Reset { get; set; }

        public DataOut(Point3d loc, Point3d rot, int reset)
        {
            LocX = loc.X;
            LocY = loc.Y;
            LocZ = loc.Z;
            RotX = rot.X;
            RotY = rot.Y;
            Reset = reset;
        }
        public DataOut(float locX, float locY, float locZ, float rotX, float rotY, int reset)
        {
            LocX = locX;
            LocY = locY;
            LocZ = locZ;
            RotX = rotX;
            RotY = rotY;
            Reset = reset;
        }

        public string Serialize()
        {
            return $"{LocX},{LocY},{LocZ},{RotX},{RotY},{Reset}";
        }
    }
}
