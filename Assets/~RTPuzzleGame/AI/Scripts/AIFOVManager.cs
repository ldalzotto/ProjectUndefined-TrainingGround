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
            var deltaAngle = (aiFov.FovSlices[0].EndAngleExcluded - aiFov.FovSlices[0].BeginAngleIncluded) / sampleNB;
            for (var i = 0; i < sampleNB; i++)
            {
                NavMeshRayCaster.CastNavMeshRayFOVAgent(agent, deltaAngle * i + aiFov.FovSlices[0].BeginAngleIncluded, raySampleDistance, out navMeshHits[i]);
            }

            return navMeshHits;
        }

        public void SetAvailableFROVRange(float beginAngle, float endAngle)
        {
            Debug.Log("Setting Slices : ");
            foreach (var fovSclice in aiFov.FovSlices)
            {
                Debug.Log("b : " + beginAngle + " e : " + endAngle);
            }
            aiFov.ReplaceFovSlices(new FOVSlice(beginAngle, endAngle));
        }

        public void ResetFOV()
        {
            SetAvailableFROVRange(0f, 360f);
        }

        public void IntersectFOV(float beginAngle, float endAngle)
        {

        }

        public void GizmoTick()
        {
            foreach (var fovSlice in aiFov.FovSlices)
            {
                Gizmos.color = Color.blue;
                var beginAxisDirection = Quaternion.AngleAxis(-fovSlice.BeginAngleIncluded, agent.transform.up) * (agent.transform.forward * 10);
                Gizmos.DrawRay(agent.transform.position, beginAxisDirection);
                var style = new GUIStyle();
                style.normal.textColor = Color.blue;
                Handles.Label(agent.transform.position + beginAxisDirection, "Begin (" + aiFov.FovSlices.IndexOf(fovSlice) + ")", style);

                Gizmos.color = Color.red;
                style.normal.textColor = Color.red;
                var endAxisDirection = Quaternion.AngleAxis(-fovSlice.EndAngleExcluded, agent.transform.up) * (agent.transform.forward * 10);
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

        public void ReplaceFovSlices(FOVSlice fovSclice)
        {
            fovSlices.Clear();
            fovSlices.Add(fovSclice);
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

}