using CoreGame;
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

        /// <summary>
        /// Navmesh raycast by taking random angles but evenly spreaded across the FOV.
        /// </summary>
        /// <param name="sampleNB"></param>
        /// <param name="sourceTransform"></param>
        /// <param name="raySampleDistance"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Navmesh raycast by forcing the extreme angles of the FOV. Then, the remaining samples are shoces random and evenly spread across the FOV.
        /// </summary>
        /// <param name="sourceTransform"></param>
        /// <param name="raySampleDistance"></param>
        /// <returns></returns>
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

        public List<StartEndSlice> IntersectFOV_FromEscapeDirection(Vector3 from, Vector3 to, float escapeSemiAngle)
        {
            var localEscapeDirection = (to - from).normalized;
            var worldEscapeDirectionAngle = FOVLocalToWorldTransformations.AngleFromDirectionInFOVSpace(localEscapeDirection);
            return this.IntersectFOV(worldEscapeDirectionAngle - escapeSemiAngle, worldEscapeDirectionAngle + escapeSemiAngle);
        }

        public List<StartEndSlice> IntersectFOV(float beginAngle, float endAngle)
        {
            Debug.Log("Intersect FOV. Intersection angle : " + beginAngle + " , " + endAngle);
            var inputSlices = CutInputAnglesToSlice(beginAngle, endAngle);

            var newSlices = new List<StartEndSlice>();
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

        public static StartEndSlice IntersectSlice(StartEndSlice sourceSlice, StartEndSlice newSlice)
        {
            if (sourceSlice.Up())
            {
                if (newSlice.AngleDegreeContains(sourceSlice.BeginIncluded) && newSlice.AngleDegreeContains(sourceSlice.EndExcluded))
                {
                    return sourceSlice;
                }
                else if (sourceSlice.AngleDegreeContains(newSlice.BeginIncluded))
                {
                    if (sourceSlice.AngleDegreeContains(newSlice.EndExcluded))
                    {
                        return newSlice;
                    }
                    else
                    {
                        if (newSlice.Up())
                        {
                            return new StartEndSlice(newSlice.BeginIncluded, sourceSlice.EndExcluded);
                        }
                        else
                        {
                            return new StartEndSlice(sourceSlice.EndExcluded, newSlice.BeginIncluded);
                        }

                    }
                }
                else if (sourceSlice.AngleDegreeContains(newSlice.EndExcluded))
                {
                    if (newSlice.Up())
                    {
                        return new StartEndSlice(sourceSlice.BeginIncluded, newSlice.EndExcluded);
                    }
                    else
                    {
                        return new StartEndSlice(newSlice.EndExcluded, sourceSlice.BeginIncluded);
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (newSlice.AngleDegreeContains(sourceSlice.BeginIncluded) && newSlice.AngleDegreeContains(sourceSlice.EndExcluded))
                {
                    return sourceSlice;
                }
                else if (sourceSlice.AngleDegreeContains(newSlice.BeginIncluded))
                {
                    if (sourceSlice.AngleDegreeContains(newSlice.EndExcluded))
                    {
                        return newSlice;
                    }
                    else
                    {
                        if (newSlice.Up())
                        {
                            return new StartEndSlice(newSlice.BeginIncluded, sourceSlice.BeginIncluded);
                        }
                        else
                        {
                            return new StartEndSlice(sourceSlice.BeginIncluded, newSlice.BeginIncluded);
                        }

                    }
                }
                else if (sourceSlice.AngleDegreeContains(newSlice.EndExcluded))
                {
                    if (newSlice.Up())
                    {
                        return new StartEndSlice(sourceSlice.EndExcluded, newSlice.EndExcluded);
                    }
                    else
                    {
                        return new StartEndSlice(newSlice.EndExcluded, sourceSlice.EndExcluded);
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
            List<StartEndSlice> rawFOV = ConvertDownSlices(aiFov);

            //(1) mapping raw data
            var mappedFOVSlices = new List<StartEndSlice>();
            for (var i = 0; i < rawFOV.Count; i++)
            {
                var currentFovSlice = rawFOV[i];
                var deltaBetweenSlices = 0f;
                if (i != 0)
                {
                    deltaBetweenSlices = mappedFOVSlices[i - 1].EndExcluded;
                }
                var deltaForCurrentSlice = currentFovSlice.EndExcluded - currentFovSlice.BeginIncluded;
                mappedFOVSlices.Add(new StartEndSlice(deltaBetweenSlices, deltaForCurrentSlice + deltaBetweenSlices));
            }

            //(2) Total angle range
            var deltaAngleRange = 0f;
            foreach (var mappedFOVSlice in mappedFOVSlices)
            {
                deltaAngleRange += (mappedFOVSlice.EndExcluded - mappedFOVSlice.BeginIncluded);
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
                    if (mappedFOVSlice.AngleDegreeContains(currentDeltaAngle))
                    {
                        var associatedRawFOV = rawFOV[mappedFOVSlices.IndexOf(mappedFOVSlice)];
                        var mappedRatio = (currentDeltaAngle - mappedFOVSlice.BeginIncluded) / (mappedFOVSlice.EndExcluded - mappedFOVSlice.BeginIncluded);
                        var rawAngle = (mappedRatio * (associatedRawFOV.EndExcluded - associatedRawFOV.BeginIncluded)) + associatedRawFOV.BeginIncluded;
                        anglesRayCast[i] = rawAngle;
                    }
                }

            }

            return anglesRayCast;
        }

        public static float[] GetEndAnglesForRayCast(FOV aiFov)
        {
            //(0) Merge adjacent FOV range
            List<StartEndSlice> mergedFOVSlices = MergeAdjacentFOVRange(aiFov.FovSlices);

            //(1) Set end angles
            var anglesRayCast = new float[mergedFOVSlices.Count * 2];
            for (var i = 0; i < mergedFOVSlices.Count; i++)
            {
                var currentRange = mergedFOVSlices[i];
                anglesRayCast[i * 2] = currentRange.BeginIncluded;
                anglesRayCast[(i * 2) + 1] = currentRange.EndExcluded;
            }
            return anglesRayCast;
        }

        private static List<StartEndSlice> MergeAdjacentFOVRange(List<StartEndSlice> rawFOV)
        {
            var mergedFOVSlices = new List<StartEndSlice>();
            if (rawFOV.Count > 1)
            {
                for (var i = 0; i < rawFOV.Count; i++)
                {
                    if (i != 0)
                    {
                        if (rawFOV[i - 1].EndExcluded == rawFOV[i].BeginIncluded)
                        {
                            mergedFOVSlices.Add(new StartEndSlice(rawFOV[i - 1].BeginIncluded, rawFOV[i].EndExcluded));
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

        private static List<StartEndSlice> ConvertDownSlices(FOV aiFov)
        {
            var rawFOV = new List<StartEndSlice>();
            foreach (var fovSlice in aiFov.FovSlices)
            {
                if (fovSlice.Down())
                {
                    rawFOV.Add(new StartEndSlice(fovSlice.EndExcluded, fovSlice.BeginIncluded));
                }
                else
                {
                    rawFOV.Add(fovSlice);
                }
            }

            return rawFOV;
        }

        private static List<StartEndSlice> CutInputAnglesToSlice(float beginAngle, float endAngle)
        {
            List<StartEndSlice> cuttendSlices = new List<StartEndSlice>();
            if (beginAngle < 0)
            {
                if (endAngle > 0)
                {
                    cuttendSlices.Add(new StartEndSlice(beginAngle + 360f, 360f));
                    cuttendSlices.Add(new StartEndSlice(0f, endAngle));
                }
                else
                {
                    cuttendSlices.Add(new StartEndSlice(beginAngle + 360f, endAngle + 360f));
                }
            }
            else if (beginAngle == 0f)
            {
                if (endAngle < 0)
                {
                    cuttendSlices.Add(new StartEndSlice(beginAngle, endAngle + 360f));
                }
                else
                {
                    cuttendSlices.Add(new StartEndSlice(beginAngle, endAngle));
                }

            }
            else
            {
                if (endAngle < 0f)
                {
                    cuttendSlices.Add(new StartEndSlice(beginAngle, 0f));
                    cuttendSlices.Add(new StartEndSlice(360f, endAngle + 360f));
                }
                else
                {
                    cuttendSlices.Add(new StartEndSlice(beginAngle, endAngle));
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
                var beginAxisDirection = Quaternion.AngleAxis(-fovSlice.BeginIncluded, agent.transform.up) * (Vector3.forward * 10);
                Gizmos.DrawRay(agent.transform.position, beginAxisDirection);
                var style = new GUIStyle();
                style.normal.textColor = Color.blue;
#if UNITY_EDITOR
                Handles.Label(agent.transform.position + beginAxisDirection, "Begin (" + aiFov.FovSlices.IndexOf(fovSlice) + ")", style);
#endif
                Gizmos.color = Color.red;
                style.normal.textColor = Color.red;
                var endAxisDirection = Quaternion.AngleAxis(-fovSlice.EndExcluded, agent.transform.up) * (Vector3.forward * 10);
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
        private List<StartEndSlice> fovSlices;
        private Action<FOV> onFOVChange;

        public FOV(Action<FOV> onFovChange)
        {
            this.onFOVChange = onFovChange;
            fovSlices = new List<StartEndSlice>() {
              new StartEndSlice(0f, 360f)
            };
        }

        public List<StartEndSlice> FovSlices { get => fovSlices; }

        public void ReplaceFovSlices(List<StartEndSlice> fovSclices)
        {
            this.fovSlices = fovSclices;
            if (this.onFOVChange != null)
            {
                this.onFOVChange.Invoke(this);
            }
        }

        public float GetSumOfAvailableAngleDeg()
        {
            return fovSlices.ConvertAll((StartEndSlice slice) => slice.AngleDiffNotSigned()).Sum();
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

}