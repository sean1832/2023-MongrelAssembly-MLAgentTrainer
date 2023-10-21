using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Grasshopper_IO.Data
{
    [System.Serializable]
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

        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>()
            {
                {"data", Serialize()}
            };
        }

        public string ToJson(bool pretty = false)
        {
            return JsonUtility.ToJson(this, pretty);
        }

        // overload as needed
        public string ToJsonWithMeta(List<string> serializedDatas, Observation observation, bool pretty = false)
        {
            ExportData exportData = new ExportData()
            {
                serializedData = serializedDatas.ToArray(),
                observation = observation
            };
            return JsonUtility.ToJson(exportData, pretty);
        }

        [Serializable]
        private class ExportData
        {
            public string[] serializedData;
            public Observation observation;
        }
    }

    
}
