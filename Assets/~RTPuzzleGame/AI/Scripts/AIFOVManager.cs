using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class AIFOVManager
    {

        private FOV aiFov;

        public AIFOVManager()
        {
            aiFov = new FOV();
        }

        public NavMeshHit[] NavMeshRaycastSample(int sampleNB, Transform sourceTransform, Vector3 startDirection, float raySampleDistance)
        {
            //TODO more generic
            var fovSlice = aiFov.FovSlices[0];

            NavMeshHit[] navMeshHits = new NavMeshHit[sampleNB];
            var deltaAngle = (fovSlice.EndAngleExcluded - fovSlice.BeginAngleIncluded) / sampleNB;

            for (var i = 0; i < sampleNB; i++)
            {
                var currentAngle = fovSlice.BeginAngleIncluded + (i * deltaAngle);
                NavMeshRayCaster.CastNavMeshRay(sourceTransform.position, startDirection, Quaternion.Euler(0, currentAngle, 0), raySampleDistance, out navMeshHits[i]);
                //  NavMeshRayCaster.CastNavMeshRay(sourceTransform, Quaternion.Euler(0, currentAngle, 0), raySampleDistance, out navMeshHits[i]);
            }

            return navMeshHits;
        }

    }

    class FOV
    {
        private List<FOVSlice> fovSlices;

        public FOV()
        {
            fovSlices = new List<FOVSlice>() {
                new FOVSlice(0f,360f)
            };
        }

        internal List<FOVSlice> FovSlices { get => fovSlices; }
    }

    class FOVSlice
    {
        private float beginAngleIncluded;
        private float endAngleExcluded;

        public FOVSlice(float beginAngleIncluded, float endAngleExcluded)
        {
            this.beginAngleIncluded = beginAngleIncluded;
            this.endAngleExcluded = endAngleExcluded;
        }

        public float BeginAngleIncluded { get => beginAngleIncluded; }
        public float EndAngleExcluded { get => endAngleExcluded; }
    }

}