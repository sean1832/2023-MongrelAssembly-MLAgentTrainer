using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Grasshopper_IO.Data
{
    [System.Serializable]
    public class DataIn
    {
        public float score;
        public Observation observation;
        public Point3d[] candidates;
        public long timestamp;
    }
    [System.Serializable]
    public class Observation
    {
        public Point3d bBox;
        public float aggregationDiff;
        public float density;
        public float maxStress;
        public float maxDisplacement;
        public float[] nearDistance;
        public int[] candidatesMap;
    }
    [System.Serializable]
    public class Point3d
    {
        public float X;
        public float Y;
        public float Z;

        public Point3d(){}
        public Point3d(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
