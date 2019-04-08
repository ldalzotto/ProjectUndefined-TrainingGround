using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class AIFOVManager
    {

        private NavMeshAgent agent;
        private FOV aiFov;

        public AIFOVManager(NavMeshAgent agent, Action<FOV> onFOVChange)
        {
            this.agent = agent;
            aiFov = new FOV(onFOVChange);
            ResetFOV();
        }

        public NavMeshHit[] NavMeshRaycastSample(int sampleNB, Transform sourceTransform, float raySampleDistance)
        {
            var navMeshHits = new NavMeshHit[sampleNB];

            float[] anglesRayCast = AIFOVManager.CalculateAnglesForRayCast(sampleNB, aiFov, true);

            for (var i = 0; i < sampleNB; i++)
            {
                NavMeshRayCaster.CastNavMeshRayFOVAgentWorld(agent, anglesRayCast[i], raySampleDistance, out navMeshHits[i]);
            }

            return navMeshHits;
        }

        public NavMeshHit[] NavMeshRaycastEndOfRanges(Transform sourceTransform, float raySampleDistance)
        {
            var anglesRayCast = AIFOVManager.GetEndAnglesForRayCast(aiFov);
            var navMeshHits = new NavMeshHit[anglesRayCast.Length];
            for (var i = 0; i < navMeshHits.Length; i++)
            {
                NavMeshRayCaster.CastNavMeshRayFOVAgentWorld(agent, anglesRayCast[i], raySampleDistance, out navMeshHits[i]);
            }
            return navMeshHits;
        }

        public List<FOVSlice> IntersectFOV(float beginAngle, float endAngle)
        {
            Debug.Log("Intersect FOV. Intersection angle : " + beginAngle + " , " + endAngle);
            var inputSlices = CutInputAnglesToSlice(beginAngle, endAngle);

            var newSlices = new List<FOVSlice>();
            foreach (var fovSlice in aiFov.FovSlices)
            {
                foreach (var inputSlice in inputSlices)
                {
                    var intersectSlice = IntersectSlice(fovSlice, inputSlice);
                    if (intersectSlice != null)
                    {
                        newSlices.Add(intersectSlice);
                    }
                }
            }

            aiFov.ReplaceFovSlices(newSlices);
            Debug.Log("Intersect FOV. Result : " + aiFov);
            return newSlices;
        }

        public static FOVSlice IntersectSlice(FOVSlice sourceSlice, FOVSlice newSlice)
        {
            if (sourceSlice.Up())
            {
                if (newSlice.Contains(sourceSlice.BeginAngleIncluded) && newSlice.Contains(sourceSlice.EndAngleExcluded))
                {
                    return sourceSlice;
                }
                else if (sourceSlice.Contains(newSlice.BeginAngleIncluded))
                {
                    if (sourceSlice.Contains(newSlice.EndAngleExcluded))
                    {
                        return newSlice;
                    }
                    else
                    {
                        if (newSlice.Up())
                        {
                            return new FOVSlice(newSlice.BeginAngleIncluded, sourceSlice.EndAngleExcluded);
                        }
                        else
                        {
                            return new FOVSlice(sourceSlice.EndAngleExcluded, newSlice.BeginAngleIncluded);
                        }

                    }
                }
                else if (sourceSlice.Contains(newSlice.EndAngleExcluded))
                {
                    if (newSlice.Up())
                    {
                        return new FOVSlice(sourceSlice.BeginAngleIncluded, newSlice.EndAngleExcluded);
                    }
                    else
                    {
                        return new FOVSlice(newSlice.EndAngleExcluded, sourceSlice.BeginAngleIncluded);
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (newSlice.Contains(sourceSlice.BeginAngleIncluded) && newSlice.Contains(sourceSlice.EndAngleExcluded))
                {
                    return sourceSlice;
                }
                else if (sourceSlice.Contains(newSlice.BeginAngleIncluded))
                {
                    if (sourceSlice.Contains(newSlice.EndAngleExcluded))
                    {
                        return newSlice;
                    }
                    else
                    {
                        if (newSlice.Up())
                        {
                            return new FOVSlice(newSlice.BeginAngleIncluded, sourceSlice.BeginAngleIncluded);
                        }
                        else
                        {
                            return new FOVSlice(sourceSlice.BeginAngleIncluded, newSlice.BeginAngleIncluded);
                        }

                    }
                }
                else if (sourceSlice.Contains(newSlice.EndAngleExcluded))
                {
                    if (newSlice.Up())
                    {
                        return new FOVSlice(sourceSlice.EndAngleExcluded, newSlice.EndAngleExcluded);
                    }
                    else
                    {
                        return new FOVSlice(newSlice.EndAngleExcluded, sourceSlice.EndAngleExcluded);
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public static float[] CalculateAnglesForRayCast(int sampleNB, FOV aiFov, bool withRandomness)
        {
            //(0) converting down slices
            List<FOVSlice> rawFOV = ConvertDownSlices(aiFov);

            //(1) mapping raw data
            var mappedFOVSlices = new List<FOVSlice>();
            for (var i = 0; i < rawFOV.Count; i++)
            {
                var currentFovSlice = rawFOV[i];
                var deltaBetweenSlices = 0f;
                if (i != 0)
                {
                    deltaBetweenSlices = mappedFOVSlices[i - 1].EndAngleExcluded;
                }
                var deltaForCurrentSlice = currentFovSlice.EndAngleExcluded - currentFovSlice.BeginAngleIncluded;
                mappedFOVSlices.Add(new FOVSlice(deltaBetweenSlices, deltaForCurrentSlice + deltaBetweenSlices));
            }

            //(2) Total angle range
            var deltaAngleRange = 0f;
            foreach (var mappedFOVSlice in mappedFOVSlices)
            {
                deltaAngleRange += (mappedFOVSlice.EndAngleExcluded - mappedFOVSlice.BeginAngleIncluded);
            }

            var deltaAngle = deltaAngleRange / sampleNB;
            var randomnessDeltaAngle = UnityEngine.Random.Range(0, deltaAngleRange);

            //(3) Mapping from angle to raw range
            var anglesRayCast = new float[sampleNB];
            for (var i = 0; i < sampleNB; i++)
            {
                var currentDeltaAngle = 0f;
                if (withRandomness)
                {
                    currentDeltaAngle = Mathf.Repeat((deltaAngle * i) + randomnessDeltaAngle, deltaAngleRange);
                }
                else
                {
                    currentDeltaAngle = deltaAngle * i;
                }


                foreach (var mappedFOVSlice in mappedFOVSlices)
                {
                    if (mappedFOVSlice.Contains(currentDeltaAngle))
                    {
                        var associatedRawFOV = rawFOV[mappedFOVSlices.IndexOf(mappedFOVSlice)];
                        var mappedRatio = (currentDeltaAngle - mappedFOVSlice.BeginAngleIncluded) / (mappedFOVSlice.EndAngleExcluded - mappedFOVSlice.BeginAngleIncluded);
                        var rawAngle = (mappedRatio * (associatedRawFOV.EndAngleExcluded - associatedRawFOV.BeginAngleIncluded)) + associatedRawFOV.BeginAngleIncluded;
                        anglesRayCast[i] = rawAngle;
                    }
                }

            }

            return anglesRayCast;
        }

        public static float[] GetEndAnglesForRayCast(FOV aiFov)
        {
            //(0) Merge adjacent FOV range
            List<FOVSlice> mergedFOVSlices = MergeAdjacentFOVRange(aiFov.FovSlices);

            //(1) Set end angles
            var anglesRayCast = new float[mergedFOVSlices.Count * 2];
            for (var i = 0; i < mergedFOVSlices.Count; i++)
            {
                var currentRange = mergedFOVSlices[i];
                anglesRayCast[i * 2] = currentRange.BeginAngleIncluded;
                anglesRayCast[(i * 2) + 1] = currentRange.EndAngleExcluded;
            }
            return anglesRayCast;
        }

        private static List<FOVSlice> MergeAdjacentFOVRange(List<FOVSlice> rawFOV)
        {
            var mergedFOVSlices = new List<FOVSlice>();
            if (rawFOV.Count > 1)
            {
                for (var i = 0; i < rawFOV.Count; i++)
                {
                    if (i != 0)
                    {
                        if (rawFOV[i - 1].EndAngleExcluded == rawFOV[i].BeginAngleIncluded)
                        {
                            mergedFOVSlices.Add(new FOVSlice(rawFOV[i - 1].BeginAngleIncluded, rawFOV[i].EndAngleExcluded));
                        }
                        else
                        {
                            mergedFOVSlices.Add(rawFOV[i - 1]);
                            if (i == rawFOV.Count - 1)
                            {
                                mergedFOVSlices.Add(rawFOV[i]);
                            }
                        }
                    }
                }
            }
            else
            {
                return rawFOV;
            }


            return mergedFOVSlices;
        }

        private static List<FOVSlice> ConvertDownSlices(FOV aiFov)
        {
            var rawFOV = new List<FOVSlice>();
            foreach (var fovSlice in aiFov.FovSlices)
            {
                if (fovSlice.Down())
                {
                    rawFOV.Add(new FOVSlice(fovSlice.EndAngleExcluded, fovSlice.BeginAngleIncluded));
                }
                else
                {
                    rawFOV.Add(fovSlice);
                }
            }

            return rawFOV;
        }

        private static List<FOVSlice> CutInputAnglesToSlice(float beginAngle, float endAngle)
        {
            List<FOVSlice> cuttendSlices = new List<FOVSlice>();
            if (beginAngle < 0)
            {
                if (endAngle > 0)
                {
                    cuttendSlices.Add(new FOVSlice(beginAngle + 360f, 360f));
                    cuttendSlices.Add(new FOVSlice(0f, endAngle));
                }
                else
                {
                    cuttendSlices.Add(new FOVSlice(beginAngle + 360f, endAngle + 360f));
                }
            }
            else if (beginAngle == 0f)
            {
                if (endAngle < 0)
                {
                    cuttendSlices.Add(new FOVSlice(beginAngle, endAngle + 360f));
                }
                else
                {
                    cuttendSlices.Add(new FOVSlice(beginAngle, endAngle));
                }

            }
            else
            {
                if (endAngle < 0f)
                {
                    cuttendSlices.Add(new FOVSlice(beginAngle, 0f));
                    cuttendSlices.Add(new FOVSlice(360f, endAngle + 360f));
                }
                else
                {
                    cuttendSlices.Add(new FOVSlice(beginAngle, endAngle));
                }
            }
            return cuttendSlices;
        }

        private void SetAvailableFOVRange(float beginAngle, float endAngle)
        {
            aiFov.ReplaceFovSlices(CutInputAnglesToSlice(beginAngle, endAngle));
        }

        public void ResetFOV()
        {
            SetAvailableFOVRange(0f, 360f);
        }


        public void GizmoTick()
        {
            foreach (var fovSlice in aiFov.FovSlices)
            {
                Gizmos.color = Color.blue;
                var beginAxisDirection = Quaternion.AngleAxis(-fovSlice.BeginAngleIncluded, agent.transform.up) * (Vector3.forward * 10);
                Gizmos.DrawRay(agent.transform.position, beginAxisDirection);
                var style = new GUIStyle();
                style.normal.textColor = Color.blue;
#if UNITY_EDITOR
                Handles.Label(agent.transform.position + beginAxisDirection, "Begin (" + aiFov.FovSlices.IndexOf(fovSlice) + ")", style);
#endif
                Gizmos.color = Color.red;
                style.normal.textColor = Color.red;
                var endAxisDirection = Quaternion.AngleAxis(-fovSlice.EndAngleExcluded, agent.transform.up) * (Vector3.forward * 10);
#if UNITY_EDITOR
                Gizmos.DrawRay(agent.transform.position, endAxisDirection);
                Handles.Label(agent.transform.position + endAxisDirection, "End (" + aiFov.FovSlices.IndexOf(fovSlice) + ")", style);
#endif
                Gizmos.color = Color.white;
            }
        }

        #region Data Retrieval
        public FOV GetFOV()
        {
            return aiFov;
        }
        public float GetFOVAngleSum()
        {
            var angleSum = 0f;
            foreach (var fovSlice in aiFov.FovSlices)
            {
                angleSum += fovSlice.AngleDiffNotSigned();
            }
            return angleSum;
        }
        #endregion

    }

    public enum AnglesCalculationMethod
    {
        RANDOM = 0,
        WITH_END_ANGLES = 1
    }


    public class FOV
    {
        private List<FOVSlice> fovSlices;
        private Action<FOV> onFOVChange;

        public FOV(Action<FOV> onFovChange)
        {
            this.onFOVChange = onFovChange;
            fovSlices = new List<FOVSlice>() {
              new FOVSlice(0f, 360f)
            };
        }

        public List<FOVSlice> FovSlices { get => fovSlices; }

        public void ReplaceFovSlices(List<FOVSlice> fovSclices)
        {
            this.fovSlices = fovSclices;
            if (this.onFOVChange != null)
            {
                this.onFOVChange.Invoke(this);
            }
        }

        public float GetSumOfAvailableAngleDeg()
        {
            return fovSlices.ConvertAll((FOVSlice slice) => slice.AngleDiffNotSigned()).Sum();
        }

        public override string ToString()
        {
            var str = "FOVSlices : ";
            foreach (var fovSlice in fovSlices)
            {
                str += fovSlice.ToString();
            }
            return str;
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

        public bool Up()
        {
            return beginAngleIncluded <= endAngleExcluded;
        }

        public bool Down()
        {
            return beginAngleIncluded >= endAngleExcluded;
        }

        public bool Contains(float a)
        {
            if (Up())
            {
                if (endAngleExcluded == 360f)
                {
                    return a >= beginAngleIncluded && a <= endAngleExcluded;
                }
                else
                {
                    return a >= beginAngleIncluded && a < endAngleExcluded;
                }
            }
            else
            {
                if (endAngleExcluded == 360f)
                {
                    return a <= beginAngleIncluded && a >= endAngleExcluded;
                }
                else
                {
                    return a < beginAngleIncluded && a >= endAngleExcluded;
                }
            }

        }

        public float AngleDiffNotSigned()
        {
            return Mathf.Abs(endAngleExcluded - beginAngleIncluded);
        }

        public override string ToString()
        {
            return "b : " + beginAngleIncluded + " e : " + endAngleExcluded;
        }

        public override bool Equals(object obj)
        {
            var slice = obj as FOVSlice;
            return slice != null &&
                   beginAngleIncluded == slice.beginAngleIncluded &&
                   endAngleExcluded == slice.endAngleExcluded;
        }

        public override int GetHashCode()
        {
            var hashCode = -8683044;
            hashCode = hashCode * -1521134295 + beginAngleIncluded.GetHashCode();
            hashCode = hashCode * -1521134295 + endAngleExcluded.GetHashCode();
            return hashCode;
        }
    }

}