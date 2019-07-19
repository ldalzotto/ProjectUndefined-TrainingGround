using CoreGame;
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
            RangeTypeID.SIGHT_VISION,
            RangeTypeID.ATTRACTIVE_OBJECT_ACTIVE,
            RangeTypeID.ATTRACTIVE_OBJECT,
            RangeTypeID.LAUNCH_PROJECTILE,
            RangeTypeID.LAUNCH_PROJECTILE_CURSOR,
            RangeTypeID.TARGET_ZONE
        };

        private List<CircleRangeBufferData> CircleRangeBufferValues = new List<CircleRangeBufferData>();
        private ComputeBuffer CircleRangeBuffer;

        private List<BoxRangeBufferData> BoxRangeBufferValues = new List<BoxRangeBufferData>();
        private ComputeBuffer BoxRangeBuffer;

        private List<FrustumRangeBufferData> FrustumRangeBufferValues = new List<FrustumRangeBufferData>();
        private ComputeBuffer FrustumRangeBuffer;

        private List<RangeExecutionOrderBufferData> RangeExecutionOrderBufferDataValues = new List<RangeExecutionOrderBufferData>();
        private ComputeBuffer RangeExecutionOrderBuffer;

        private DynamicComputeBufferManager<FrustumPointsPositions> FrustumBufferManager;
        private DynamicComputeBufferManager<RangeToFrustumBufferLink> RangeToFrustumBufferLinkManager;
        private List<RangeToFrustumBufferLink> RangeToFrustumBufferLinkValues = new List<RangeToFrustumBufferLink>();
        private Dictionary<ObstacleListener, List<int>> ComputedFrustumPointsWorldPositionsIndexes = new Dictionary<ObstacleListener, List<int>>();

        public void Init()
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

            this.CircleRangeBuffer = new ComputeBuffer(this.rangeEffectRenderOrder.Count, CircleRangeBufferData.GetByteSize());
            this.CircleRangeBuffer.SetData(this.CircleRangeBufferValues);
            this.MasterRangeMaterial.SetBuffer("CircleRangeBuffer", this.CircleRangeBuffer);

            this.BoxRangeBuffer = new ComputeBuffer(this.rangeEffectRenderOrder.Count, BoxRangeBufferData.GetByteSize());
            this.BoxRangeBuffer.SetData(this.BoxRangeBufferValues);
            this.MasterRangeMaterial.SetBuffer("BoxRangeBuffer", this.BoxRangeBuffer);

            this.FrustumRangeBuffer = new ComputeBuffer(this.rangeEffectRenderOrder.Count, FrustumRangeBufferData.GetByteSize());
            this.FrustumRangeBuffer.SetData(this.FrustumRangeBufferValues);
            this.MasterRangeMaterial.SetBuffer("FrustumRangeBuffer", this.FrustumRangeBuffer);

            this.RangeExecutionOrderBuffer = new ComputeBuffer(this.rangeEffectRenderOrder.Count, RangeExecutionOrderBufferData.GetByteSize());
            this.RangeExecutionOrderBuffer.SetData(this.RangeExecutionOrderBufferDataValues);
            this.MasterRangeMaterial.SetBuffer("RangeExecutionOrderBuffer", this.RangeExecutionOrderBuffer);

            this.FrustumBufferManager = new DynamicComputeBufferManager<FrustumPointsPositions>(FrustumPointsPositions.GetByteSize(), "FrustumBufferDataBuffer", "_FrustumBufferDataBufferCount", ref this.MasterRangeMaterial);
            this.RangeToFrustumBufferLinkManager = new DynamicComputeBufferManager<RangeToFrustumBufferLink>(RangeToFrustumBufferLink.GetByteSize(), "RangeToFrustumBufferLinkBuffer", "_RangeToFrustumBufferLinkCount", ref this.MasterRangeMaterial);
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
                foreach (var testSphere in this.ObstaclesListenerManager.GetAllObstacleListeners())
                {
                    int startIndex = frustumBufferDatas.Count;
                    foreach (var nearObstable in testSphere.NearSquereObstacles)
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

            Profiler.BeginSample("RangeBufferManagerTick");
            foreach (var rangeEffectId in this.rangeEffectRenderOrder)
            {
                if (this.rangeEffectManagers.ContainsKey(rangeEffectId))
                {
                    foreach (var rangeEffectManager in this.rangeEffectManagers[rangeEffectId].Values)
                    {
                        if (rangeEffectManager.GetType() == typeof(SphereGroundEffectManager))
                        {
                            var SphereGroundEffectManager = (SphereGroundEffectManager)rangeEffectManager;
                            var circleRangeBufferData = SphereGroundEffectManager.ToSphereBuffer();
                            this.CircleRangeBufferValues.Add(circleRangeBufferData);
                            this.RangeExecutionOrderBufferDataValues.Add(new RangeExecutionOrderBufferData(1, 0,0, this.CircleRangeBufferValues.Count - 1));

                            if (circleRangeBufferData.OccludedByFrustums == 1)
                            {
                                foreach (var computedFrustumPointsWorldPositionsIndexe in this.ComputedFrustumPointsWorldPositionsIndexes[SphereGroundEffectManager.AssociatedRangeObject.RangeObstacleListener.ObstacleListener])
                                {
                                    this.RangeToFrustumBufferLinkValues.Add(new RangeToFrustumBufferLink(this.CircleRangeBufferValues.Count - 1, computedFrustumPointsWorldPositionsIndexe));
                                }
                            }
                        }
                        else if (rangeEffectManager.GetType() == typeof(BoxGroundEffectManager))
                        {
                            this.BoxRangeBufferValues.Add(((BoxGroundEffectManager)rangeEffectManager).ToBoxBuffer());
                            this.RangeExecutionOrderBufferDataValues.Add(new RangeExecutionOrderBufferData(0, 1,0, this.BoxRangeBufferValues.Count - 1));
                        }
                        else if (rangeEffectManager.GetType() == typeof(FrustumGroundEffectManager))
                        {
                            var FrustumGroundEffectManager = ((FrustumGroundEffectManager)rangeEffectManager);
                            var frustumRangeBufferData = FrustumGroundEffectManager.ToFrustumBuffer();
                            this.FrustumRangeBufferValues.Add(frustumRangeBufferData);
                            this.RangeExecutionOrderBufferDataValues.Add(new RangeExecutionOrderBufferData(0, 0, 1, this.FrustumRangeBufferValues.Count - 1));

                            if (frustumRangeBufferData.OccludedByFrustums == 1)
                            {
                                foreach (var computedFrustumPointsWorldPositionsIndexe in this.ComputedFrustumPointsWorldPositionsIndexes[FrustumGroundEffectManager.AssociatedRangeObject.RangeObstacleListener.ObstacleListener])
                                {
                                    this.RangeToFrustumBufferLinkValues.Add(new RangeToFrustumBufferLink(this.FrustumRangeBufferValues.Count - 1, computedFrustumPointsWorldPositionsIndexe));
                                }
                            }
                        }
                    }
                }
            }
            Profiler.EndSample();

            if (this.CircleRangeBuffer.IsValid())
            {
                this.CircleRangeBuffer.SetData(this.CircleRangeBufferValues);
            }
            if (this.BoxRangeBuffer.IsValid())
            {
                this.BoxRangeBuffer.SetData(this.BoxRangeBufferValues);
            }
            if (this.FrustumRangeBuffer.IsValid())
            {
                this.FrustumRangeBuffer.SetData(this.FrustumRangeBufferValues);
            }
            if (this.RangeExecutionOrderBuffer.IsValid())
            {
                this.RangeExecutionOrderBuffer.SetData(this.RangeExecutionOrderBufferDataValues);
            }

            this.RangeToFrustumBufferLinkManager.Tick(d, (List<RangeToFrustumBufferLink> RangeToFrustumBufferLinkDatas) =>
            {
                RangeToFrustumBufferLinkDatas.AddRange(this.RangeToFrustumBufferLinkValues);
            });

            this.OnCommandBufferUpdate();

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
            ComputeBufferHelper.SafeCommandBufferReleaseAndDispose(this.CircleRangeBuffer);
            ComputeBufferHelper.SafeCommandBufferReleaseAndDispose(this.BoxRangeBuffer);
            ComputeBufferHelper.SafeCommandBufferReleaseAndDispose(this.FrustumRangeBuffer);
            ComputeBufferHelper.SafeCommandBufferReleaseAndDispose(this.RangeExecutionOrderBuffer);
            if (this.FrustumBufferManager != null)
            {
                this.FrustumBufferManager.Dispose();
            }
            if (this.RangeToFrustumBufferLinkManager != null)
            {
                this.RangeToFrustumBufferLinkManager.Dispose();
            }
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

        internal void OnCommandBufferUpdate()
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
        public int IsSphere;
        public int IsCube;
        public int IsFrustum;
        public int Index;

        public RangeExecutionOrderBufferData(int isSphere, int isCube, int isFrustum, int index)
        {
            IsSphere = isSphere;
            IsCube = isCube;
            Index = index;
            IsFrustum = isFrustum;
        }

        public static int GetByteSize()
        {
            return (1 + 1 + 1 + 1) * sizeof(int);
        }
    }

    struct RangeToFrustumBufferLink
    {
        public int RangeIndex;
        public int FrustumIndex;

        public RangeToFrustumBufferLink(int rangeIndex, int frustumIndex)
        {
            RangeIndex = rangeIndex;
            FrustumIndex = frustumIndex;
        }

        public static int GetByteSize()
        {
            return (1 + 1) * sizeof(int);
        }
    }
}
