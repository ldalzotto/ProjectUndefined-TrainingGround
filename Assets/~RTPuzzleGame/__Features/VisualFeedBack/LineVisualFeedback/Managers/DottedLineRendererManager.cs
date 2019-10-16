﻿using CoreGame;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace RTPuzzle
{
    public class DottedLineRendererManager : GameSingleton<DottedLineRendererManager>, IDottedLineRendererManagerEvent
    {
        #region External Dependencies
        private PuzzleMaterialConfiguration PuzzleMaterialConfiguration;
        #endregion

        private DottedLineManagerThread DottedLineManagerThreadObject;

        private Queue<ComputeBeziersInnerPointResponse> ComputeBeziersInnerPointResponses = new Queue<ComputeBeziersInnerPointResponse>();

        private Dictionary<int, DottedLine> dottedLines = new Dictionary<int, DottedLine>();

        public Dictionary<int, DottedLine> DottedLines { get => dottedLines; }

        public DottedLineRendererManager()
        {
            this.PuzzleMaterialConfiguration = PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.PuzzleStaticConfiguration.PuzzleMaterialConfiguration;
            this.DottedLineManagerThreadObject = new DottedLineManagerThread(this.OnComputeBeziersInnerPointResponse);
        }

        public virtual void Tick()
        {
            while (this.ComputeBeziersInnerPointResponses.Count > 0)
            {
                ComputeBeziersInnerPointResponse ComputeBeziersInnerPointResponse;
                lock (this.ComputeBeziersInnerPointResponses)
                {
                    ComputeBeziersInnerPointResponse = this.ComputeBeziersInnerPointResponses.Dequeue();
                }

                if (this.DottedLines.ContainsKey(ComputeBeziersInnerPointResponse.ID))
                {
                    CombineInstance[] combine = new CombineInstance[ComputeBeziersInnerPointResponse.Transforms.Count];
                    for (var i = 0; i < ComputeBeziersInnerPointResponse.Transforms.Count; i++)
                    {
                        combine[i].mesh = this.PuzzleMaterialConfiguration.RangeDiamondMesh;
                        combine[i].transform = ComputeBeziersInnerPointResponse.Transforms[i];
                    }
                    this.DottedLines[ComputeBeziersInnerPointResponse.ID].GetMesh().CombineMeshes(combine, true, true);
                }
            }
        }

        protected virtual void OnComputeBeziersInnerPointResponse(ComputeBeziersInnerPointResponse ComputeBeziersInnerPointResponse)
        {
            lock (this.ComputeBeziersInnerPointResponses)
            {
                this.ComputeBeziersInnerPointResponses.Enqueue(ComputeBeziersInnerPointResponse);
            }
        }

        #region External Events
        public virtual void OnDottedLineDestroyed(DottedLine dottedLine)
        {
            if (this.dottedLines.ContainsKey(dottedLine.GetUniqueID()))
            {
                this.dottedLines.Remove(dottedLine.GetUniqueID());
            }
        }

        public virtual void OnComputeBeziersInnerPointEvent(DottedLine DottedLine)
        {
            this.dottedLines[DottedLine.GetUniqueID()] = DottedLine;
            this.DottedLineManagerThreadObject.OnComputeBeziersInnerPointEvent(DottedLine.BuildComputeBeziersInnerPointEvent());
        }

        public virtual void OnLevelExit()
        {
            lock (this.ComputeBeziersInnerPointResponses)
            {
                this.ComputeBeziersInnerPointResponses.Clear();
            }
        }
        #endregion
    }

    public class DottedLineManagerThread
    {
        private Action<ComputeBeziersInnerPointResponse> SendComputeBeziersInnerPointResponse;

        public DottedLineManagerThread(Action<ComputeBeziersInnerPointResponse> sendComputeBeziersInnerPointResponse)
        {
            SendComputeBeziersInnerPointResponse = sendComputeBeziersInnerPointResponse;
        }

        private Dictionary<int, CancellationTokenSource> computeBeziersInnerPointTasks = new Dictionary<int, CancellationTokenSource>();

        public void OnComputeBeziersInnerPointEvent(ComputeBeziersInnerPointEvent ComputeBeziersInnerPointEvent)
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            lock (this.computeBeziersInnerPointTasks)
            {
                if (this.computeBeziersInnerPointTasks.ContainsKey(ComputeBeziersInnerPointEvent.ID))
                {
                    this.computeBeziersInnerPointTasks[ComputeBeziersInnerPointEvent.ID].Cancel();
                }
                this.computeBeziersInnerPointTasks[ComputeBeziersInnerPointEvent.ID] = tokenSource;
            }

            Task.Factory.StartNew(() => this.ComputeEvent(ComputeBeziersInnerPointEvent), token);
        }

        private void ComputeEvent(ComputeBeziersInnerPointEvent ComputeBeziersInnerPointEvent)
        {
            var ComputeBeziersInnerPointEventResponse = new ComputeBeziersInnerPointResponse();
            ComputeBeziersInnerPointEventResponse.ID = ComputeBeziersInnerPointEvent.ID;
            ComputeBeziersInnerPointEventResponse.Transforms = new List<Matrix4x4>();

            int pointNumber = Convert.ToInt32(ComputeBeziersInnerPointEvent.BeziersControlPoints.GetPointsRawDistance() * (ComputeBeziersInnerPointEvent.PointNumberPerUnitDistance));
            for (var i = 0; i <= pointNumber; i++)
            {
                Vector3 worldPosition = ComputeBeziersInnerPointEvent.BeziersControlPoints.ResolvePoint((float)i / (float)pointNumber);
                ComputeBeziersInnerPointEventResponse.Transforms.Add(Matrix4x4.TRS(worldPosition, Quaternion.identity, ComputeBeziersInnerPointEvent.MeshScale));
            }

            lock (this.computeBeziersInnerPointTasks)
            {
                this.computeBeziersInnerPointTasks.Remove(ComputeBeziersInnerPointEvent.ID);
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

        public ComputeBeziersInnerPointEvent(int iD, BeziersControlPoints beziersControlPoints, float meshScale, float pointNumberPerUnitDistance)
        {
            ID = iD;
            BeziersControlPoints = beziersControlPoints;
            MeshScale = new Vector3(meshScale, meshScale, meshScale);
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
