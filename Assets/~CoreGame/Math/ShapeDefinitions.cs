using UnityEngine;

namespace CoreGame
{
    /*
     * 
     *     C5----C6
     *    / |    /|
     *   C1----C2 |
     *   |  C8  | C7   
     *   | /    |/     C3->C7  Forward
     *   C4----C3     
     */

    [System.Serializable]
    public class FrustumV2
    {
        [SerializeField]
        public Vector3 Center;
        [SerializeField]
        public Vector3 WorldPosition;
        [SerializeField]
        public Quaternion Rotation;
        [SerializeField]
        public Vector3 LossyScale;
        [SerializeField]
        public FrustumFaceV2 F1;
        [SerializeField]
        public bool UseFaceDefinition = true;
        [SerializeField]
        public FrustumFaceV2 F2;
        [SerializeField]
        public bool UseAngleDefinition = true;
        [SerializeField]
        public Vector2 C1Angles;
        [SerializeField]
        public Vector2 C2Angles;
        [SerializeField]
        public Vector2 C3Angles;
        [SerializeField]
        public Vector2 C4Angles;
        [SerializeField]
        public float FaceDistance;
    }

    [System.Serializable]
    public class FrustumFaceV2
    {
        [SerializeField]
        public Vector3 FaceOffsetFromCenter;
        [SerializeField]
        public float Height;
        [SerializeField]
        public float Width;
    }

}
