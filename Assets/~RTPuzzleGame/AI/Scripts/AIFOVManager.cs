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
        }

        public NavMeshHit[] NavMeshRaycastSample(int sampleNB, Transform sourceTransform, Vector3 inputRandomDirection, float raySampleDistance)
        {
            var navMeshHits = new NavMeshHit[sampleNB];
            /**
            var fovSlice = aiFov.FovSlices[0];

            var deltaAngle = 0f;

            deltaAngle = (fovSlice.EndAngleExcluded - aiFov.FovSlices[0].BeginAngleIncluded) / sampleNB;


            for (var i = 0; i < sampleNB; i++)
            {
                NavMeshRayCaster.CastNavMeshRayFOVAgentWorld(agent, deltaAngle * i + aiFov.FovSlices[0].BeginAngleIncluded, raySampleDistance, out navMeshHits[i]);
            }**/

            return navMeshHits;
        }

        public void SetAvailableFROVRange(float beginAngle, float endAngle)
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
                    cuttendSlices.Add(new FOVSlice(beginAngle + 360f, 0f));
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
                    cuttendSlices.Add(new FOVSlice(0f, endAngle + 360f));
                }
                else
                {
                    cuttendSlices.Add(new FOVSlice(beginAngle, endAngle));
                }
            }
            return cuttendSlices;
        }

        private static void ClampAnglesFrom0To360(ref float beginAngle, ref float endAngle)
        {

            if (Mathf.Sign(beginAngle) != Mathf.Sign(endAngle))
            {
                while ((beginAngle < 0 || beginAngle > 360) || (endAngle < 0 || endAngle > 360))
                {
                    if (beginAngle < 0 || endAngle < 0)
                    {
                        beginAngle += 180f;
                        endAngle += 180f;
                        var tmpBegin = beginAngle;
                    }
                    else if (beginAngle > 360 || endAngle > 360)
                    {
                        beginAngle -= 180f;
                        endAngle -= 180f;
                        var tmpBegin = beginAngle;
                    }
                }
            }
            else
            {
                while ((beginAngle < 0 || beginAngle > 360) || (endAngle < 0 || endAngle > 360))
                {
                    if (beginAngle < 0 || endAngle < 0)
                    {
                        beginAngle += 360f;
                        endAngle += 360f;
                    }
                    else if (beginAngle > 360 || endAngle > 360)
                    {
                        beginAngle -= 360f;
                        endAngle -= 360f;
                    }
                }
            }
        }

        public void ResetFOV()
        {
            SetAvailableFROVRange(0f, 360f);
        }

        public void IntersectFOV(float beginAngle, float endAngle)
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
        }

        public static FOVSlice IntersectSlice(FOVSlice sourceSlice, FOVSlice newSlice)
        {
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

        public void ReplaceFovSlices(List<FOVSlice> fovSclices)
        {
            this.fovSlices = fovSclices;
        }

        public bool Is360()
        {
            return fovSlices.Count == 1 && Mathf.Abs(fovSlices[0].EndAngleExcluded - fovSlices[0].BeginAngleIncluded) == 360f;
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
    }

}