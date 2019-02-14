using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class AIFOVManager
    {

        private NavMeshAgent agent;
        private FOV aiFov;


        public AIFOVManager(NavMeshAgent agent)
        {
            this.agent = agent;
            aiFov = new FOV();
        }

        public NavMeshHit[] NavMeshRaycastSample(int sampleNB, Transform sourceTransform, Vector3 inputRandomDirection, float raySampleDistance)
        {
            //TODO more generi
            var navMeshHits = new NavMeshHit[sampleNB];
            var deltaAngle = (aiFov.FovSlices[0].EndAngleExcluded - aiFov.FovSlices[0].BeginAngleIncluded) / sampleNB;
            for (var i = 0; i < sampleNB; i++)
            {
                NavMeshRayCaster.CastNavMeshRayFOVAgent(agent, deltaAngle * i + aiFov.FovSlices[0].BeginAngleIncluded, raySampleDistance, out navMeshHits[i]);
                // NavMeshRayCaster.CastNavMeshRayWorldSpace(sourceTransform, Quaternion.Euler(0, deltaAngle * i + aiFov.FovSlices[0].BeginAngleIncluded, 0), raySampleDistance, out navMeshHits[i]);
            }


            return navMeshHits;
        }

        public void SetAvailableFROVRange(List<FOVSlice> fovSlices)
        {
            Debug.Log("Setting Slices : ");
            foreach (var fovSclice in aiFov.FovSlices)
            {
                Debug.Log("b : " + fovSclice.BeginAngleIncluded + " e : " + fovSclice.EndAngleExcluded);
            }
            aiFov.ReplaceFovSlices(fovSlices);
        }

        /**
        public void RemoveAnglesFromFOV(float begin, float end)
        {
            Debug.Log("REMOVE ANGLES " + "begin : " + begin + " end : " + end);

            var beginAngle = Mathf.Repeat(begin, 360f);
            var endAngle = Mathf.Repeat(end, 360f);

            var minAngle = beginAngle;
            var maxAngle = endAngle;

            List<FOVSlice> cuttedFOVSlices = new List<FOVSlice>();
            foreach (var fovSclice in aiFov.FovSlices)
            {
                if (fovSclice.Contains(minAngle))
                {
                    if (fovSclice.BeginAngleIncluded != minAngle)
                    {
                        cuttedFOVSlices.Add(new FOVSlice(fovSclice.BeginAngleIncluded, minAngle));
                        if (fovSclice.Contains(maxAngle))
                        {
                            cuttedFOVSlices.Add(new FOVSlice(maxAngle, fovSclice.EndAngleExcluded));
                        }
                    }
                }
                else if (fovSclice.Contains(maxAngle))
                {
                    cuttedFOVSlices.Add(new FOVSlice(maxAngle, fovSclice.EndAngleExcluded));
                }
            }

            aiFov.ReplaceFovSlices(cuttedFOVSlices);
            Debug.Log("After Slice : ");
            foreach (var fovSclice in aiFov.FovSlices)
            {
                Debug.Log("b : " + fovSclice.BeginAngleIncluded + " e : " + fovSclice.EndAngleExcluded);
            }
            UpdateDisplayTexture();
        }
    **/

        public void ResetFOV()
        {
            SetAvailableFROVRange(new List<FOVSlice>() { new FOVSlice(0f, 360f) });
            //aiFov.ReplaceFovSlices(new List<FOVSlice>() { new FOVSlice(0f, 360f) });
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

        public void ReplaceFovSlices(List<FOVSlice> newSlices)
        {
            fovSlices = newSlices;
        }
    }

    public class FOVSlice
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