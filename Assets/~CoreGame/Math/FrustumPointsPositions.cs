using UnityEngine;
using System.Collections;

namespace CoreGame
{
    [System.Serializable]
    public struct FrustumPointsPositions
    {
        public Vector3 FC1;
        public Vector3 FC2;
        public Vector3 FC3;
        public Vector3 FC4;
        public Vector3 FC5;
        public Vector3 FC6;
        public Vector3 FC7;
        public Vector3 FC8;

        public FrustumPointsPositions(Vector3 fC1, Vector3 fC2, Vector3 fC3, Vector3 fC4, Vector3 fC5, Vector3 fC6, Vector3 fC7, Vector3 fC8)
        {
            FC1 = fC1;
            FC2 = fC2;
            FC3 = fC3;
            FC4 = fC4;
            FC5 = fC5;
            FC6 = fC6;
            FC7 = fC7;
            FC8 = fC8;
        }

        public static int GetByteSize()
        {
            return (8 * 3 * sizeof(float));
        }
    }

}
