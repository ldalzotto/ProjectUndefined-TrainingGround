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

        public NavMeshHit[] NavMeshRaycastSample(int sampleNB, Transform sourceTransform, Vector3 inputRandomDirection, float raySampleDistance)
        {
            //TODO more generic
            aiFov.FovSlices.Sort(delegate (FOVSlice x, FOVSlice y)
            {
                if (x.BeginAngleIncluded > y.BeginAngleIncluded)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            });

            var cumulatorNormalizedAngleFOV = 0f;
            foreach (var fovSlice in aiFov.FovSlices)
            {
                cumulatorNormalizedAngleFOV += fovSlice.GetNormalizedSliceAngleFactor();
            }

            NavMeshHit[] navMeshHits = new NavMeshHit[sampleNB];
            var deltaAngleNormalized = cumulatorNormalizedAngleFOV / sampleNB;

            var randomAngleNormalized = Vector3.SignedAngle(sourceTransform.forward, Vector3.ProjectOnPlane(inputRandomDirection, sourceTransform.up).normalized, sourceTransform.up) / 360f;
            randomAngleNormalized = (randomAngleNormalized / 2) + 1;

            for (var i = 0; i < sampleNB; i++)
            {
                var currentAngle = ((i * deltaAngleNormalized) + randomAngleNormalized) % (cumulatorNormalizedAngleFOV);
                var deNormalizedCurrentAngle = DeNormalizeAngle(currentAngle, aiFov.FovSlices);

                foreach (var fovSlice in aiFov.FovSlices)
                {
                    if (fovSlice.Contains(deNormalizedCurrentAngle))
                    {
                        NavMeshRayCaster.CastNavMeshRayWorldSpace(sourceTransform, Quaternion.Euler(0, deNormalizedCurrentAngle, 0), raySampleDistance, out navMeshHits[i]);
                        break;
                    }
                }
                //  NavMeshRayCaster.CastNavMeshRay(sourceTransform, Quaternion.Euler(0, currentAngle, 0), raySampleDistance, out navMeshHits[i]);
            }
            return navMeshHits;
        }

        private float DeNormalizeAngle(float normalizedAngle, List<FOVSlice> fovSlices)
        {
            var normalizedAngleSum = 0f;
            var currentDenormalizedAngle = 0f;
            foreach (var fovSlice in fovSlices)
            {
                if (fovSlices.IndexOf(fovSlice) == 0 && fovSlice.BeginAngleIncluded > 0f)
                {
                    //first
                    currentDenormalizedAngle += fovSlice.BeginAngleIncluded;
                }

                if (fovSlices.IndexOf(fovSlice) > 0)
                {
                    if (fovSlice.BeginAngleIncluded != fovSlices[fovSlices.IndexOf(fovSlice) - 1].EndAngleExcluded)
                    {
                        var delta = (fovSlice.BeginAngleIncluded - fovSlices[fovSlices.IndexOf(fovSlice) - 1].EndAngleExcluded);
                        currentDenormalizedAngle += delta;
                        normalizedAngleSum += (delta / 360);
                    }
                }

                if (fovSlice.Contains((normalizedAngleSum + normalizedAngle) * 360))
                {
                    currentDenormalizedAngle += 360 * normalizedAngle;
                    return currentDenormalizedAngle;
                }
                else
                {
                    normalizedAngle -= fovSlice.GetNormalizedSliceAngleFactor();
                    currentDenormalizedAngle += (fovSlice.EndAngleExcluded - fovSlice.BeginAngleIncluded);
                }

                normalizedAngleSum += fovSlice.GetNormalizedSliceAngleFactor();
            }
            return currentDenormalizedAngle;
        }

    }

    class FOV
    {
        private List<FOVSlice> fovSlices;

        public FOV()
        {
            fovSlices = new List<FOVSlice>() {
              new FOVSlice(0f, 360f)
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

        public float GetNormalizedSliceAngleFactor()
        {
            return (endAngleExcluded - beginAngleIncluded) / 360f;
        }

        public bool Contains(float a)
        {
            return a >= beginAngleIncluded && a < endAngleExcluded;
        }
    }

    class FOVCalculationRange
    {
        private float beginAngleIncluded;
        private float endAngleExcluded;

        public FOVCalculationRange(float beginAngleIncluded, float endAngleExcluded)
        {
            this.beginAngleIncluded = beginAngleIncluded;
            this.endAngleExcluded = endAngleExcluded;
        }

        public float BeginAngleIncluded { get => beginAngleIncluded; }
        public float EndAngleExcluded { get => endAngleExcluded; }
    }

}