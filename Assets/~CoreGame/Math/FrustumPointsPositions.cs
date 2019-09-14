﻿using UnityEngine;
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

        public Vector3 normal1;
        public Vector3 normal2;
        public Vector3 normal3;
        public Vector3 normal4;
        public Vector3 normal5;
        public Vector3 normal6;

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

            float crossSign = Mathf.Sign(Vector3.Dot(FC5 - FC1, Vector3.Cross(FC2 - FC1, FC4 - FC1)));

            this.normal1 = crossSign * Vector3.Cross(FC2 - FC1, FC3 - FC1);
            this.normal2 = crossSign * Vector3.Cross(FC5 - FC1, FC2 - FC1);
            this.normal3 = crossSign * Vector3.Cross(FC6 - FC2, FC3 - FC2);
            this.normal4 = crossSign * Vector3.Cross(FC7 - FC3, FC4 - FC3);
            this.normal5 = crossSign * Vector3.Cross(FC8 - FC4, FC1 - FC4);
            this.normal6 = crossSign * Vector3.Cross(FC8 - FC5, FC6 - FC5);
        }
        
        public static int GetByteSize()
        {
            return ( ((8 * 3) + (6 * 3)) * sizeof(float));
        }
    }

}
