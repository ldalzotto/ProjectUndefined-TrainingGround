using System.Collections.Generic;
using UnityEditor;
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
            ResetFOV();
        }

        public NavMeshHit[] NavMeshRaycastSample(int sampleNB, Transform sourceTransform, Vector3 inputRandomDirection, float raySampleDistance)
        {
            var navMeshHits = new NavMeshHit[sampleNB];

            float[] anglesRayCast = AIFOVManager.CalculateAnglesForRayCast(sampleNB, aiFov);

            for (var i = 0; i < sampleNB; i++)
            {
                NavMeshRayCaster.CastNavMeshRayFOVAgentWorld(agent, anglesRayCast[i], raySampleDistance, out navMeshHits[i]);
            }

            return navMeshHits;
        }

        public void SetAvailableFOVRange(float beginAngle, float endAngle)
        {
            aiFov.ReplaceFovSlices(CutInputAnglesToSlice(beginAngle, endAngle));
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

        public void ResetFOV()
        {
            SetAvailableFOVRange(0f, 360f);
        }

        public List<FOVSlice> IntersectFOV(float beginAngle, float endAngle)
        {
            Debug.Log("Intersect called b : " + beginAngle + " e : " + endAngle);
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
            return newSlices;
        }

        public static FOVSlice IntersectSlice(FOVSlice sourceSlice, FOVSlice newSlice)
        {
            Debug.Log("Calling intersect slice : " + sourceSlice.ToString() + "  " + newSlice.ToString());
            if (sourceSlice.Up())
            {
                if (sourceSlice.Contains(newSlice.BeginAngleIncluded))
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
                if (sourceSlice.Contains(newSlice.BeginAngleIncluded))
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

        public static float[] CalculateAnglesForRayCast(int sampleNB, FOV aiFov)
        {

            //(0) converting down slices
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

            //(3) Mapping from angle to raw range
            var anglesRayCast = new float[sampleNB];
            for (var i = 0; i < sampleNB; i++)
            {
                var currentDeltaAngle = deltaAngle * i;
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

        public void GizmoTick()
        {
            foreach (var fovSlice in aiFov.FovSlices)
            {
                Gizmos.color = Color.blue;
                var beginAxisDirection = Quaternion.AngleAxis(-fovSlice.BeginAngleIncluded, agent.transform.up) * (Vector3.forward * 10);
                Gizmos.DrawRay(agent.transform.position, beginAxisDirection);
                var style = new GUIStyle();
                style.normal.textColor = Color.blue;
                Handles.Label(agent.transform.position + beginAxisDirection, "Begin (" + aiFov.FovSlices.IndexOf(fovSlice) + ")", style);

                Gizmos.color = Color.red;
                style.normal.textColor = Color.red;
                var endAxisDirection = Quaternion.AngleAxis(-fovSlice.EndAngleExcluded, agent.transform.up) * (Vector3.forward * 10);
                Gizmos.DrawRay(agent.transform.position, endAxisDirection);
                Handles.Label(agent.transform.position + endAxisDirection, "End (" + aiFov.FovSlices.IndexOf(fovSlice) + ")", style);

                Gizmos.color = Color.white;
            }
        }

    }


    public class FOV
    {
        private List<FOVSlice> fovSlices;

        public FOV()
        {
            fovSlices = new List<FOVSlice>() {
              new FOVSlice(0f, 360f)
            };
        }

        internal List<FOVSlice> FovSlices { get => fovSlices; }

        public void ReplaceFovSlices(List<FOVSlice> fovSclices)
        {
            this.fovSlices = fovSclices;
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
                return a >= beginAngleIncluded && a < endAngleExcluded;
            }
            else
            {
                return a <= beginAngleIncluded && a > endAngleExcluded;
            }

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