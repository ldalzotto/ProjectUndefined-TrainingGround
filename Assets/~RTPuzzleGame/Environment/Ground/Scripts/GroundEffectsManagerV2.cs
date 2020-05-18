﻿using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

namespace RTPuzzle
{

    public class GroundEffectsManagerV2 : MonoBehaviour
    {
        public Material MasterRangeMaterial;

        #region External Dependencies
        private PuzzleGameConfigurationManager PuzzleGameConfigurationManager;
        private ObstaclesListenerManager ObstaclesListenerManager;
        private ObstacleFrustumCalculationManager ObstacleFrustumCalculationManager;
        #endregion

        private CommandBuffer command;
        private GroundEffectType[] AffectedGroundEffectsType;
        private HashSet<MeshRenderer> RenderedRenderers = new HashSet<MeshRenderer>();

        private Dictionary<RangeTypeID, Dictionary<int, IAbstractGroundEffectManager>> rangeEffectManagers = new Dictionary<RangeTypeID, Dictionary<int, IAbstractGroundEffectManager>>();

        private List<RangeTypeID> rangeEffectRenderOrder = new List<RangeTypeID>() {
            RangeTypeID.ATTRACTIVE_OBJECT_ACTIVE,
            RangeTypeID.ATTRACTIVE_OBJECT,
            RangeTypeID.SIGHT_VISION,
            RangeTypeID.LAUNCH_PROJECTILE,
            RangeTypeID.LAUNCH_PROJECTILE_CURSOR,
            RangeTypeID.TARGET_ZONE
        };

        private List<CircleRangeBufferData> CircleRangeBufferValues = new List<CircleRangeBufferData>();
        private DynamicComputeBufferManager<CircleRangeBufferData> CircleRangeBuffer;

        private List<BoxRangeBufferData> BoxRangeBufferValues = new List<BoxRangeBufferData>();
        private DynamicComputeBufferManager<BoxRangeBufferData> BoxRangeBuffer;

        private List<FrustumRangeBufferData> FrustumRangeBufferValues = new List<FrustumRangeBufferData>();
        private DynamicComputeBufferManager<FrustumRangeBufferData> FrustumRangeBuffer;

        private List<RangeExecutionOrderBufferData> RangeExecutionOrderBufferDataValues = new List<RangeExecutionOrderBufferData>();
        private DynamicComputeBufferManager<RangeExecutionOrderBufferData> RangeExecutionOrderBuffer;

        private DynamicComputeBufferManager<FrustumPointsPositions> FrustumBufferManager;
        private DynamicComputeBufferManager<RangeToFrustumBufferLink> RangeToFrustumBufferLinkManager;
        private List<RangeToFrustumBufferLink> RangeToFrustumBufferLinkValues = new List<RangeToFrustumBufferLink>();
        private Dictionary<ObstacleListener, List<int>> ComputedFrustumPointsWorldPositionsIndexes = new Dictionary<ObstacleListener, List<int>>();

        public void Init(LevelZonesID currentLevelID)
        {
            #region External Dependencies
            PuzzleGameConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            ObstaclesListenerManager = GameObject.FindObjectOfType<ObstaclesListenerManager>();
            ObstacleFrustumCalculationManager = GameObject.FindObjectOfType<ObstacleFrustumCalculationManager>();
            #endregion

            var camera = Camera.main;

            this.command = new CommandBuffer();
            this.command.name = "GroundEffectsManagerV2";
            camera.AddCommandBuffer(CameraEvent.AfterForwardOpaque, this.command);

            AffectedGroundEffectsType = GetComponentsInChildren<GroundEffectType>();
            for (var i = 0; i < AffectedGroundEffectsType.Length; i++)
            {
                AffectedGroundEffectsType[i].Init();
            }

            this.CircleRangeBuffer = new DynamicComputeBufferManager<CircleRangeBufferData>(CircleRangeBufferData.GetByteSize(), "CircleRangeBuffer", string.Empty, ref this.MasterRangeMaterial, BufferReAllocateStrategy.SUPERIOR_ONLY);
            this.BoxRangeBuffer = new DynamicComputeBufferManager<BoxRangeBufferData>(BoxRangeBufferData.GetByteSize(), "BoxRangeBuffer", string.Empty, ref this.MasterRangeMaterial, BufferReAllocateStrategy.SUPERIOR_ONLY);
            this.FrustumRangeBuffer = new DynamicComputeBufferManager<FrustumRangeBufferData>(FrustumRangeBufferData.GetByteSize(), "FrustumRangeBuffer", string.Empty, ref this.MasterRangeMaterial, BufferReAllocateStrategy.SUPERIOR_ONLY);
            this.RangeExecutionOrderBuffer = new DynamicComputeBufferManager<RangeExecutionOrderBufferData>(RangeExecutionOrderBufferData.GetByteSize(), "RangeExecutionOrderBuffer", string.Empty, ref this.MasterRangeMaterial, BufferReAllocateStrategy.SUPERIOR_ONLY);

            this.FrustumBufferManager = new DynamicComputeBufferManager<FrustumPointsPositions>(FrustumPointsPositions.GetByteSize(), "FrustumBufferDataBuffer", "_FrustumBufferDataBufferCount", ref this.MasterRangeMaterial);
            this.RangeToFrustumBufferLinkManager = new DynamicComputeBufferManager<RangeToFrustumBufferLink>(RangeToFrustumBufferLink.GetByteSize(), "RangeToFrustumBufferLinkBuffer", "_RangeToFrustumBufferLinkCount", ref this.MasterRangeMaterial);

            //master range shader color level adjuster
            var LevelRangeEffectInherentData = PuzzleGameConfigurationManager.LevelConfiguration()[currentLevelID].LevelRangeEffectInherentData;
            this.MasterRangeMaterial.SetFloat("_AlbedoBoost", 1f + LevelRangeEffectInherentData.DeltaIntensity);
            this.MasterRangeMaterial.SetFloat("_RangeMixFactor", 0.5f + LevelRangeEffectInherentData.DeltaMixFactor);
        }

        public void Tick(float d)
        {
            Profiler.BeginSample("GroundEffectsManagerV2Tick");
            this.RenderedRenderers.Clear();
            foreach (var groundEffectManager in this.rangeEffectManagers.Values.SelectMany(kv => kv.Values))
            {
                if (groundEffectManager != null)
                {
                    groundEffectManager.Tick(d);
                    groundEffectManager.MeshToRender(ref this.RenderedRenderers, this.AffectedGroundEffectsType);
                }
            }

            this.CircleRangeBufferValues.Clear();
            this.BoxRangeBufferValues.Clear();
            this.FrustumRangeBufferValues.Clear();
            this.RangeExecutionOrderBufferDataValues.Clear();
            this.RangeToFrustumBufferLinkValues.Clear();
            this.ComputedFrustumPointsWorldPositionsIndexes.Clear();

            Profiler.BeginSample("FrustumBufferManagerTick");
            this.FrustumBufferManager.Tick(d, (List<FrustumPointsPositions> frustumBufferDatas) =>
            {
                //(WARNING) - Obstacle frustum calculation for display is multithreaded. Thus, calculation result may not be available even though the obstacle has been detected
                foreach (var testSphere in this.ObstaclesListenerManager.GetAllObstacleListeners())
                {
                    int startIndex = frustumBufferDatas.Count;
                    foreach (var nearObstable in testSphere.NearSquareObstacles)
                    {
                        var frustumCalculationResults = this.ObstacleFrustumCalculationManager.GetResult(testSphere, nearObstable).CalculatedFrustumPositions;
                        frustumBufferDatas.AddRange(frustumCalculationResults);
                    }
                    if (startIndex != frustumBufferDatas.Count)
                    {
                        this.ComputedFrustumPointsWorldPositionsIndexes[testSphere] = Enumerable.Range(startIndex, frustumBufferDatas.Count).ToList();
                    }
                }
            });
            Profiler.EndSample();

            #region RangeBufferManagerTick 
            Profiler.BeginSample("RangeBufferManagerTick");
            foreach (var rangeEffectId in this.rangeEffectRenderOrder)
            {
                if (this.rangeEffectManagers.ContainsKey(rangeEffectId))
                {
                    foreach (var rangeEffectManager in this.rangeEffectManagers[rangeEffectId].Values)
                    {
                        RangeExecutionOrderBufferData addedRangeExecutionOrderBufferData;

                        if (rangeEffectManager.GetType() == typeof(SphereGroundEffectManager))
                        {
                            var SphereGroundEffectManager = (SphereGroundEffectManager)rangeEffectManager;
                            var circleRangeBufferData = SphereGroundEffectManager.ToSphereBuffer();
                            this.CircleRangeBufferValues.Add(circleRangeBufferData);
                            addedRangeExecutionOrderBufferData = new RangeExecutionOrderBufferData(0, this.CircleRangeBufferValues.Count - 1);
                            this.RangeExecutionOrderBufferDataValues.Add(addedRangeExecutionOrderBufferData);
                        }
                        else if (rangeEffectManager.GetType() == typeof(BoxGroundEffectManager))
                        {
                            this.BoxRangeBufferValues.Add(((BoxGroundEffectManager)rangeEffectManager).ToBoxBuffer());
                            addedRangeExecutionOrderBufferData = new RangeExecutionOrderBufferData(1, this.BoxRangeBufferValues.Count - 1);
                            this.RangeExecutionOrderBufferDataValues.Add(addedRangeExecutionOrderBufferData);
                        }
                        else
                        {
                            var FrustumGroundEffectManager = ((FrustumGroundEffectManager)rangeEffectManager);
                            var frustumRangeBufferData = FrustumGroundEffectManager.ToFrustumBuffer();
                            this.FrustumRangeBufferValues.Add(frustumRangeBufferData);
                            addedRangeExecutionOrderBufferData = new RangeExecutionOrderBufferData(2, this.FrustumRangeBufferValues.Count - 1);
                            this.RangeExecutionOrderBufferDataValues.Add(addedRangeExecutionOrderBufferData);
                        }

                        //Range to obstacle occlusion frustum link
                        if (rangeEffectManager.GetAssociatedRangeObject().IsOccludedByFrustum())
                        {
                            //(WARNING) - Obstacle frustum calculation for display is multithreaded. Thus, calculation result may not be available even though the obstacle has been detected
                            if (this.ComputedFrustumPointsWorldPositionsIndexes.ContainsKey(rangeEffectManager.GetAssociatedRangeObject().RangeObstacleListener.ObstacleListener))
                            {
                                foreach (var computedFrustumPointsWorldPositionsIndexe in this.ComputedFrustumPointsWorldPositionsIndexes[rangeEffectManager.GetAssociatedRangeObject().RangeObstacleListener.ObstacleListener])
                                {
                                    this.RangeToFrustumBufferLinkValues.Add(new RangeToFrustumBufferLink(addedRangeExecutionOrderBufferData.Index, addedRangeExecutionOrderBufferData.RangeType, computedFrustumPointsWorldPositionsIndexe));
                                }
                            }
                        }
                    }
                }
            }
            Profiler.EndSample();
            #endregion

            #region Buffer data set

            this.CircleRangeBuffer.Tick(d, (List<CircleRangeBufferData> CircleRangeBufferDatas) => { CircleRangeBufferDatas.AddRange(this.CircleRangeBufferValues); });
            this.BoxRangeBuffer.Tick(d, (List<BoxRangeBufferData> BoxRangeBufferDatas) => { BoxRangeBufferDatas.AddRange(this.BoxRangeBufferValues); });
            this.FrustumRangeBuffer.Tick(d, (List<FrustumRangeBufferData> FrustumRangeBufferDatas) => { FrustumRangeBufferDatas.AddRange(this.FrustumRangeBufferValues); });

            this.RangeExecutionOrderBuffer.Tick(d, (List<RangeExecutionOrderBufferData> RangeExecutionOrderBufferDatas) => { RangeExecutionOrderBufferDatas.AddRange(this.RangeExecutionOrderBufferDataValues); });

            this.RangeToFrustumBufferLinkManager.Tick(d, (List<RangeToFrustumBufferLink> RangeToFrustumBufferLinkDatas) => { RangeToFrustumBufferLinkDatas.AddRange(this.RangeToFrustumBufferLinkValues); });

            this.OnCommandBufferUpdate();
            #endregion

            Profiler.EndSample();
        }

        #region External events
        public void OnRangeAdded(RangeTypeObject rangeTypeObject)
        {
            if (rangeTypeObject.RangeType.IsRangeConfigurationDefined())
            {
                if (!this.rangeEffectManagers.ContainsKey(rangeTypeObject.RangeType.RangeTypeID))
                {
                    this.rangeEffectManagers[rangeTypeObject.RangeType.RangeTypeID] = new Dictionary<int, IAbstractGroundEffectManager>();
                }

                if (rangeTypeObject.RangeType.GetType() == typeof(SphereRangeType))
                {
                    var sphereRangeType = (SphereRangeType)rangeTypeObject.RangeType;
                    var addedRange = (IAbstractGroundEffectManager)new SphereGroundEffectManager(PuzzleGameConfigurationManager.RangeTypeConfiguration()[rangeTypeObject.RangeType.RangeTypeID]);
                    this.rangeEffectManagers[rangeTypeObject.RangeType.RangeTypeID].Add(rangeTypeObject.GetInstanceID(), addedRange);
                    addedRange.OnRangeCreated(rangeTypeObject);
                }
                else if (rangeTypeObject.RangeType.GetType() == typeof(BoxRangeType))
                {
                    var boxRangeType = (BoxRangeType)rangeTypeObject.RangeType;
                    var addedRange = (IAbstractGroundEffectManager)new BoxGroundEffectManager(PuzzleGameConfigurationManager.RangeTypeConfiguration()[rangeTypeObject.RangeType.RangeTypeID]);
                    this.rangeEffectManagers[rangeTypeObject.RangeType.RangeTypeID].Add(rangeTypeObject.GetInstanceID(), addedRange);
                    addedRange.OnRangeCreated(rangeTypeObject);
                }
                else if (rangeTypeObject.RangeType.GetType() == typeof(FrustumRangeType))
                {
                    var frustumRangeType = (FrustumRangeType)rangeTypeObject.RangeType;
                    var addedRange = (IAbstractGroundEffectManager)new FrustumGroundEffectManager(PuzzleGameConfigurationManager.RangeTypeConfiguration()[rangeTypeObject.RangeType.RangeTypeID]);
                    this.rangeEffectManagers[rangeTypeObject.RangeType.RangeTypeID].Add(rangeTypeObject.GetInstanceID(), addedRange);
                    addedRange.OnRangeCreated(rangeTypeObject);
                }
            }
        }

        internal void OnLevelExit()
        {
            //release buffers
            this.CircleRangeBuffer.IfNotNull(CircleRangeBuffer => CircleRangeBuffer.Dispose());
            this.BoxRangeBuffer.IfNotNull(BoxRangeBuffer => BoxRangeBuffer.Dispose());
            this.FrustumRangeBuffer.IfNotNull(FrustumRangeBuffer => FrustumRangeBuffer.Dispose());
            this.RangeExecutionOrderBuffer.IfNotNull(RangeExecutionOrderBuffer => RangeExecutionOrderBuffer.Dispose());
            this.FrustumBufferManager.IfNotNull(FrustumBufferManager => FrustumBufferManager.Dispose());
            this.RangeToFrustumBufferLinkManager.IfNotNull(RangeToFrustumBufferLinkManager => RangeToFrustumBufferLinkManager.Dispose());
        }

        internal void OnRangeDestroy(RangeTypeObject rangeTypeObject)
        {
            if (rangeTypeObject.IsRangeConfigurationDefined())
            {
                this.rangeEffectManagers[rangeTypeObject.RangeType.RangeTypeID][rangeTypeObject.GetInstanceID()].OnRangeDestroyed();
                this.rangeEffectManagers.Remove(rangeTypeObject.RangeType.RangeTypeID);
            }
        }

        private void OnDestroy()
        {
            this.OnLevelExit();
        }
        #endregion

        private void OnCommandBufferUpdate()
        {
            this.command.Clear();
            this.MasterRangeMaterial.SetInt("_CountSize", this.RangeExecutionOrderBufferDataValues.Count);

            foreach (var rangeEffectId in this.rangeEffectRenderOrder)
            {
                if (this.rangeEffectManagers.ContainsKey(rangeEffectId))
                {
                    foreach (var rangeEffectManager in this.rangeEffectManagers[rangeEffectId].Values)
                    {
                        rangeEffectManager.MeshToRender(ref this.RenderedRenderers, this.AffectedGroundEffectsType);
                    }
                }
            }

            foreach (var meshToRender in this.RenderedRenderers)
            {
                this.command.DrawRenderer(meshToRender, this.MasterRangeMaterial);
            }
        }

    }

    struct RangeExecutionOrderBufferData
    {
        public int RangeType; //0 -> sphere, 1 -> box, 3 -> frustum
        public int Index;

        public RangeExecutionOrderBufferData(int rangeType, int index)
        {
            RangeType = rangeType;
            Index = index;
        }

        public static int GetByteSize()
        {
            return (1 + 1) * sizeof(int);
        }
    }

    struct RangeToFrustumBufferLink
    {
        public int RangeIndex;
        public int RangeType; //0 -> sphere, 1 -> box, 3 -> frustum
        public int FrustumIndex;

        public RangeToFrustumBufferLink(int rangeIndex, int rangeType, int frustumIndex)
        {
            RangeIndex = rangeIndex;
            RangeType = rangeType;
            FrustumIndex = frustumIndex;
        }

        public static int GetByteSize()
        {
            return (1 + 1 + 1) * sizeof(int);
        }
    }
}
