using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;
using CoreGame;

namespace RTPuzzle
{
    public interface IFovManagerCalcuation
    {
        NavMeshHit[] NavMeshRaycastSample(int sampleNB, Transform sourceTransform, float raySampleDistance);
        NavMeshHit[] NavMeshRaycastEndOfRanges(Transform sourceTransform, float raySampleDistance);
        List<StartEndSlice> IntersectFOV_FromEscapeDirection(Vector3 from, Vector3 to, float escapeSemiAngle);
        List<StartEndSlice> IntersectFOV(float beginAngle, float endAngle);
        void ResetFOV();
        float GetFOVAngleSum();
    }
}
