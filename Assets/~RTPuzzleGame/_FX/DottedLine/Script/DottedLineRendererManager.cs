using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;

namespace RTPuzzle
{
    public class DottedLineRendererManager : MonoBehaviour
    {
        private DottedLineManagerThread DottedLineManagerThreadObject;
        private Thread DottedLineManagerThread;

        public Mesh DiamondMesh;
        private Queue<ComputeBeziersInnerPointResponse> ComputeBeziersInnerPointResponses = new Queue<ComputeBeziersInnerPointResponse>();

        private Dictionary<int, DottedLine> dottedLines = new Dictionary<int, DottedLine>();

        public Dictionary<int, DottedLine> DottedLines { get => dottedLines; }


        public void Init()
        {
            this.DottedLineManagerThreadObject = new DottedLineManagerThread(this.OnComputeBeziersInnerPointResponse);
            this.DottedLineManagerThread = new Thread(new ThreadStart(this.DottedLineManagerThreadObject.Main));
            this.DottedLineManagerThread.IsBackground = true;
            this.DottedLineManagerThread.Name = "DottedLineManagerThread";
            this.DottedLineManagerThread.Start();
        }

        public void Tick()
        {
            while (this.ComputeBeziersInnerPointResponses.Count > 0)
            {
                var ComputeBeziersInnerPointResponse = this.ComputeBeziersInnerPointResponses.Dequeue();
                if (this.DottedLines.ContainsKey(ComputeBeziersInnerPointResponse.ID))
                {
                    CombineInstance[] combine = new CombineInstance[ComputeBeziersInnerPointResponse.Transforms.Count];
                    for (var i = 0; i < ComputeBeziersInnerPointResponse.Transforms.Count; i++)
                    {
                        combine[i].mesh = this.DiamondMesh;
                        combine[i].transform = ComputeBeziersInnerPointResponse.Transforms[i];
                    }
                    this.DottedLines[ComputeBeziersInnerPointResponse.ID].MeshFilter.mesh.CombineMeshes(combine, true, true);
                }
            }
        }

        public void OnDottedLineDestroyed(DottedLine dottedLine)
        {
            if (this.dottedLines.ContainsKey(dottedLine.GetInstanceID()))
            {
                this.dottedLines.Remove(dottedLine.GetInstanceID());
            }
        }


        #region External Events
        public void OnComputeBeziersInnerPointEvent(DottedLine DottedLine)
        {
            this.dottedLines[DottedLine.GetInstanceID()] = DottedLine;
            this.DottedLineManagerThreadObject.OnComputeBeziersInnerPointEvent(DottedLine.BuilComputeBeziersInnerPointEvent());
        }
        public void OnComputeBeziersInnerPointResponse(ComputeBeziersInnerPointResponse ComputeBeziersInnerPointResponse)
        {
            this.ComputeBeziersInnerPointResponses.Enqueue(ComputeBeziersInnerPointResponse);
        }
        #endregion
    }

    public class DottedLineManagerThread
    {
        private Action<ComputeBeziersInnerPointResponse> SendComputeBeziersInnerPointResponse;
        private CustomSampler sampler;

        public DottedLineManagerThread(Action<ComputeBeziersInnerPointResponse> sendComputeBeziersInnerPointResponse)
        {
            SendComputeBeziersInnerPointResponse = sendComputeBeziersInnerPointResponse;
            this.sampler = CustomSampler.Create("DottedLineManagerThreadTick");
        }

        private Queue<ComputeBeziersInnerPointEvent> computeBeziersInnerPointEvents = new Queue<ComputeBeziersInnerPointEvent>();

        public void OnComputeBeziersInnerPointEvent(ComputeBeziersInnerPointEvent ComputeBeziersInnerPointEvent)
        {
            this.computeBeziersInnerPointEvents.Enqueue(ComputeBeziersInnerPointEvent);
        }

        public void Main()
        {
            Profiler.BeginThreadProfiling("My threads", "DottedLineManagerThread");
            while (true)
            {
                while (this.computeBeziersInnerPointEvents.Count > 0)
                {
                    this.sampler.Begin();
                    ComputeEvent();
                    this.sampler.End();
                }
            }
        }

        private void ComputeEvent()
        {
            var ComputeBeziersInnerPointEvent = this.computeBeziersInnerPointEvents.Dequeue();
            var ComputeBeziersInnerPointEventResponse = new ComputeBeziersInnerPointResponse();
            ComputeBeziersInnerPointEventResponse.ID = ComputeBeziersInnerPointEvent.ID;
            ComputeBeziersInnerPointEventResponse.Transforms = new List<Matrix4x4>();

            int pointNumber = Convert.ToInt32(ComputeBeziersInnerPointEvent.BeziersControlPoints.GetPointsRawDistance() * (ComputeBeziersInnerPointEvent.PointNumberPerUnitDistance));
            for (var i = 0; i <= pointNumber; i++)
            {
                Vector3 worldPosition = ComputeBeziersInnerPointEvent.BeziersControlPoints.ResolvePoint((float)i / (float)pointNumber);
                ComputeBeziersInnerPointEventResponse.Transforms.Add(Matrix4x4.TRS(worldPosition, Quaternion.identity, ComputeBeziersInnerPointEvent.MeshScale));
            }

            this.SendComputeBeziersInnerPointResponse.Invoke(ComputeBeziersInnerPointEventResponse);
        }
    }


    #region Events
    public class ComputeBeziersInnerPointEvent
    {
        public int ID;
        public BeziersControlPoints BeziersControlPoints;
        public Vector3 MeshScale;
        public float PointNumberPerUnitDistance;

        public ComputeBeziersInnerPointEvent(int iD, BeziersControlPoints beziersControlPoints, Vector3 meshScale, float pointNumberPerUnitDistance)
        {
            ID = iD;
            BeziersControlPoints = beziersControlPoints;
            MeshScale = meshScale;
            PointNumberPerUnitDistance = pointNumberPerUnitDistance;
        }
    }

    public class ComputeBeziersInnerPointResponse
    {
        public int ID;
        public List<Matrix4x4> Transforms;
    }
    #endregion
}
