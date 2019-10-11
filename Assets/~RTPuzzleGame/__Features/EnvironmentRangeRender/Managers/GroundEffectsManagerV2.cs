using CoreGame;
using GameConfigurationID;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

namespace RTPuzzle
{

    public class GroundEffectsManagerV2 : GameSingleton<GroundEffectsManagerV2>
    {

        #region External Dependencies
        private PuzzleGameConfigurationManager PuzzleGameConfigurationManager;
        private CoreMaterialConfiguration CoreMaterialConfiguration;

        private ObstaclesListenerManager ObstaclesListenerManager = ObstaclesListenerManager.Get();
        #endregion

        #region Command Buffers
        public CommandBuffer RangeDrawCommand { get; private set; }
        #endregion

        private Material MasterRangeMaterial;

        private List<GroundEffectType> AffectedGroundEffectsType;

        private Dictionary<RangeTypeID, Dictionary<int, AbstractRangeRenderData>> rangeRenderDatas;

        private List<RangeTypeID> rangeEffectRenderOrder = new List<RangeTypeID>() {
            RangeTypeID.ATTRACTIVE_OBJECT_ACTIVE,
            RangeTypeID.ATTRACTIVE_OBJECT,
            RangeTypeID.SIGHT_VISION,
            RangeTypeID.LAUNCH_PROJECTILE,
            RangeTypeID.LAUNCH_PROJECTILE_CURSOR,
            RangeTypeID.TARGET_ZONE
        };

#if UNITY_EDITOR
        public Dictionary<RangeTypeID, Dictionary<int, AbstractRangeRenderData>> RangeRenderDatas { get => rangeRenderDatas; }
#endif

        public void Init(LevelZonesID currentLevelID)
        {
            #region Init Values
            this.rangeRenderDatas = new Dictionary<RangeTypeID, Dictionary<int, AbstractRangeRenderData>>();
            #endregion

            #region External Dependencies
            PuzzleGameConfigurationManager = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager;
            this.CoreMaterialConfiguration = CoreGameSingletonInstances.CoreStaticConfigurationContainer.CoreStaticConfiguration.CoreMaterialConfiguration;
            #endregion

            this.MasterRangeMaterial = PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.PuzzleStaticConfiguration.PuzzleMaterialConfiguration.MasterRangeMaterial;
            
            this.RangeDrawCommand = new CommandBuffer();
            this.RangeDrawCommand.name = this.GetType().Name + "." + nameof(this.RangeDrawCommand);

            AffectedGroundEffectsType = GameObject.FindObjectsOfType<GroundEffectType>().ToList();
            foreach (var affectedGroundEffectType in AffectedGroundEffectsType)
            {
                affectedGroundEffectType.Init();
            }


            //Do static batching of ground effects types
            StaticBatchingUtility.Combine(
                    AffectedGroundEffectsType.ConvertAll(groundEffectType => groundEffectType.gameObject)
                    .Union(AffectedGroundEffectsType.ConvertAll(groundEffectType => groundEffectType.AssociatedGroundEffectIgnoredGroundObjectType).SelectMany(s => s).ToList().ConvertAll(groundEffectIgnoredObject => groundEffectIgnoredObject.gameObject))
                    .ToArray(), GameObject.FindWithTag(TagConstants.ROOT_CHUNK_ENVIRONMENT));

            //master range shader color level adjuster
            var LevelRangeEffectInherentData = PuzzleGameConfigurationManager.LevelConfiguration()[currentLevelID].LevelRangeEffectInherentData;
            this.MasterRangeMaterial.SetFloat("_AlbedoBoost", 1f + LevelRangeEffectInherentData.DeltaIntensity);
            this.MasterRangeMaterial.SetFloat("_RangeMixFactor", 0.5f + LevelRangeEffectInherentData.DeltaMixFactor);
        }

        public void Tick(float d)
        {
            Profiler.BeginSample("GroundEffectsManagerV2Tick");
            this.ForEachRangeRenderData((rangeRenderData) =>
            {
                rangeRenderData.Tick(d, this.AffectedGroundEffectsType);
            });

            #region Buffer data set
            this.OnCommandBufferUpdate();
            #endregion

            Profiler.EndSample();

        }


        public void ForEachRangeRenderData(Action<AbstractRangeRenderData> action)
        {
            if (this.rangeRenderDatas != null)
            {
                foreach (var rangeRenderDatasByCollider in this.rangeRenderDatas.Values)
                {
                    foreach (var rangeRenderData in rangeRenderDatasByCollider.Values)
                    {
                        if (rangeRenderData != null)
                        {
                            action.Invoke(rangeRenderData);
                        }
                    }
                }
            }
        }

        public void OnCommandBufferUpdate()
        {
            this.RangeDrawCommand.Clear();
            this.RangeDrawCommand.BeginSample("rangeDrawCommand");
            this.ForEachRangeRenderData((rangeRenderData) =>
            {
                rangeRenderData.ProcessCommandBuffer(this.RangeDrawCommand, ref this.AffectedGroundEffectsType, this.MasterRangeMaterial);
            });
            this.RangeDrawCommand.EndSample("rangeDrawCommand");
        }


        #region External events
        public void OnRangeAddedV2(RangeObjectV2 RangeObjectV2)
        {
            var rangeTypeID = RangeObjectV2.RangeObjectInitialization.RangeTypeID;
            if (rangeTypeID != RangeTypeID.NOT_DISPLAYED)
            {
                if (!this.rangeRenderDatas.ContainsKey(rangeTypeID))
                {
                    this.rangeRenderDatas[rangeTypeID] = new Dictionary<int, AbstractRangeRenderData>();
                }

                if (RangeObjectV2.GetType() == typeof(SphereRangeObjectV2))
                {
                    var SphereRangeObjectRenderingDataProvider = new SphereRangeObjectRenderingDataProvider((SphereRangeObjectV2)RangeObjectV2, rangeTypeID);
                    var addedRange = new SphereGroundEffectManager(PuzzleGameConfigurationManager.RangeTypeConfiguration()[rangeTypeID], SphereRangeObjectRenderingDataProvider);
                    addedRange.OnRangeCreated(SphereRangeObjectRenderingDataProvider);
                    this.rangeRenderDatas[rangeTypeID].Add(SphereRangeObjectRenderingDataProvider.BoundingCollider.GetInstanceID(), new CircleRangeRenderData(addedRange));
                }
                else if (RangeObjectV2.GetType() == typeof(BoxRangeObjectV2))
                {
                    var BoxRangeObjectRenderingDataProvider = new BoxRangeObjectRenderingDataProvider((BoxRangeObjectV2)RangeObjectV2, rangeTypeID);
                    var addedRange = new BoxGroundEffectManager(PuzzleGameConfigurationManager.RangeTypeConfiguration()[rangeTypeID], BoxRangeObjectRenderingDataProvider);
                    addedRange.OnRangeCreated(BoxRangeObjectRenderingDataProvider);
                    this.rangeRenderDatas[rangeTypeID].Add(BoxRangeObjectRenderingDataProvider.BoundingCollider.GetInstanceID(), new BoxRangeRenderData(addedRange));
                }
                else if (RangeObjectV2.GetType() == typeof(RoundedFrustumRangeObjectV2))
                {
                    var RoundedFrustumRangeObjectRenderingDataProvider = new FrustumRangeObjectRenderingDataProvider((RoundedFrustumRangeObjectV2)RangeObjectV2, rangeTypeID);
                    var addedRange = new RoundedFrustumGroundEffectManager(PuzzleGameConfigurationManager.RangeTypeConfiguration()[rangeTypeID], RoundedFrustumRangeObjectRenderingDataProvider);
                    addedRange.OnRangeCreated(RoundedFrustumRangeObjectRenderingDataProvider);
                    this.rangeRenderDatas[rangeTypeID].Add(RoundedFrustumRangeObjectRenderingDataProvider.BoundingCollider.GetInstanceID(), new RoundedFrustumRenderData(addedRange));
                }
            }

        }

        public void OnLevelExit()
        {
            //release buffers
            this.ForEachRangeRenderData((rangeRenderData) =>
            {
                rangeRenderData.Dispose();
            });

        }

        public void OnRangeDestroy(RangeObjectV2 RangeObjectV2)
        {
            if (RangeObjectV2.RangeObjectInitialization.RangeTypeID != RangeTypeID.NOT_DISPLAYED)
            {
                this.rangeRenderDatas[RangeObjectV2.RangeObjectInitialization.RangeTypeID][RangeObjectV2.RangeGameObjectV2.BoundingCollider.GetInstanceID()].OnRangeDestroyed();
                this.rangeRenderDatas[RangeObjectV2.RangeObjectInitialization.RangeTypeID].Remove(RangeObjectV2.RangeGameObjectV2.BoundingCollider.GetInstanceID());
                if (this.rangeRenderDatas[RangeObjectV2.RangeObjectInitialization.RangeTypeID].Count == 0)
                {
                    this.rangeRenderDatas.Remove(RangeObjectV2.RangeObjectInitialization.RangeTypeID);
                }
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            this.OnLevelExit();
        }
        #endregion

    }

    public abstract class AbstractRangeRenderData
    {
        protected IAbstractGroundEffectManager GroundEffectManager;

        private DynamicComputeBufferManager<FrustumPointsPositions> obstacleFrustumBuffer;
        protected MaterialPropertyBlock matPro;

        protected AbstractRangeRenderData()
        {
            this.matPro = new MaterialPropertyBlock();
            this.obstacleFrustumBuffer = new DynamicComputeBufferManager<FrustumPointsPositions>(FrustumPointsPositions.GetByteSize(), "FrustumBufferDataBuffer", "_FrustumBufferDataBufferCount", this.matPro);
        }

        private Mesh combinedMesh;

#if UNITY_EDITOR
        public DynamicComputeBufferManager<FrustumPointsPositions> ObstacleFrustumBuffer { get => obstacleFrustumBuffer; }
#endif

        public abstract void ProcessCommandBuffer(CommandBuffer commandBuffer, ref List<GroundEffectType> AffectedGroundEffectsType, Material MasterRangeMaterial);
        public virtual void Dispose()
        {
            this.obstacleFrustumBuffer.Dispose();
        }
        protected Mesh CombineMesh(ref List<GroundEffectType> AffectedGroundEffectsType)
        {
            if (this.GroundEffectManager.MeshMustBeRebuild() || this.combinedMesh == null)
            {
                this.combinedMesh = new Mesh();
                this.combinedMesh.CombineMeshes(
                    this.GroundEffectManager.GroundEffectTypeToRender().ConvertAll(groundEffectType => new CombineInstance() { mesh = groundEffectType.GroundEffectMesh, transform = groundEffectType.transform.localToWorldMatrix }).ToArray(),
                    true);
            }

            return combinedMesh;
        }

        protected void ProcessObstacleFrustums(CommandBuffer commandBuffer, MaterialPropertyBlock matPro)
        {
            var rangeObjectListener = this.GroundEffectManager.GetObstacleListener();
            if (rangeObjectListener != null)
            {
                this.obstacleFrustumBuffer.Tick((bufferData) =>
                {
                    rangeObjectListener.ForEachCalculatedFrustum((calculatedFrustums) =>
                    {
                        bufferData.Add(calculatedFrustums);
                    });
                });
            }
        }

        public void Tick(float d, List<GroundEffectType> affectedGroundEffectsType)
        {
            this.GroundEffectManager.Tick(d, affectedGroundEffectsType);
        }

        public void OnRangeDestroyed()
        {
            this.Dispose();
        }
    }

    public class CircleRangeRenderData : AbstractRangeRenderData
    {
        private DynamicComputeBufferManager<CircleRangeBufferData> circleRangeBuffer;

        public DynamicComputeBufferManager<CircleRangeBufferData> CircleRangeBuffer { get => circleRangeBuffer; }

        public CircleRangeRenderData(SphereGroundEffectManager sphereGroundEffectManager) : base()
        {
            this.GroundEffectManager = sphereGroundEffectManager;
            this.circleRangeBuffer = new DynamicComputeBufferManager<CircleRangeBufferData>(CircleRangeBufferData.GetByteSize(), "CircleRangeBuffer", string.Empty, this.matPro);
        }

        public override void ProcessCommandBuffer(CommandBuffer commandBuffer, ref List<GroundEffectType> AffectedGroundEffectsType, Material MasterRangeMaterial)
        {
            var combinedMesh = this.CombineMesh(ref AffectedGroundEffectsType);
            this.circleRangeBuffer.Tick((bufferData) => bufferData.Add(((SphereGroundEffectManager)this.GroundEffectManager).ToSphereBuffer()));
            this.ProcessObstacleFrustums(commandBuffer, matPro);
            commandBuffer.DrawMesh(combinedMesh, Matrix4x4.identity, MasterRangeMaterial, 0, 0, matPro);
        }

        public override void Dispose()
        {
            base.Dispose();
            this.circleRangeBuffer.Dispose();
        }
    }

    public class RoundedFrustumRenderData : AbstractRangeRenderData
    {
        private DynamicComputeBufferManager<RoundedFrustumRangeBufferData> roundedFrustumRangeBuffer;

        public DynamicComputeBufferManager<RoundedFrustumRangeBufferData> RoundedFrustumRangeBuffer { get => roundedFrustumRangeBuffer; }

        public RoundedFrustumRenderData(RoundedFrustumGroundEffectManager roundedFrustumGroundEffectManager) : base()
        {
            this.GroundEffectManager = roundedFrustumGroundEffectManager;
            this.roundedFrustumRangeBuffer = new DynamicComputeBufferManager<RoundedFrustumRangeBufferData>(RoundedFrustumRangeBufferData.GetByteSize(), "RoundedFrustumRangeBuffer", string.Empty, this.matPro);
        }

        public override void ProcessCommandBuffer(CommandBuffer commandBuffer, ref List<GroundEffectType> AffectedGroundEffectsType, Material MasterRangeMaterial)
        {
            Mesh combinedMesh = this.CombineMesh(ref AffectedGroundEffectsType);

            this.roundedFrustumRangeBuffer.Tick((bufferData) => bufferData.Add(((RoundedFrustumGroundEffectManager)this.GroundEffectManager).ToFrustumBuffer()));
            this.ProcessObstacleFrustums(commandBuffer, matPro);
            commandBuffer.DrawMesh(combinedMesh, Matrix4x4.identity, MasterRangeMaterial, 0, 3, matPro);
        }

        public override void Dispose()
        {
            base.Dispose();
            this.roundedFrustumRangeBuffer.Dispose();
        }
    }

    public class BoxRangeRenderData : AbstractRangeRenderData
    {
        private DynamicComputeBufferManager<BoxRangeBufferData> BoxRangeBuffer;

        public BoxRangeRenderData(BoxGroundEffectManager BoxGroundEffectManager) : base()
        {
            this.GroundEffectManager = BoxGroundEffectManager;
            this.BoxRangeBuffer = new DynamicComputeBufferManager<BoxRangeBufferData>(BoxRangeBufferData.GetByteSize(), "BoxRangeBuffer", string.Empty, this.matPro);
        }

        public override void ProcessCommandBuffer(CommandBuffer commandBuffer, ref List<GroundEffectType> AffectedGroundEffectsType, Material MasterRangeMaterial)
        {
            var combinedMesh = this.CombineMesh(ref AffectedGroundEffectsType);
            this.BoxRangeBuffer.Tick((bufferData) => bufferData.Add(((BoxGroundEffectManager)this.GroundEffectManager).ToBoxBuffer()));
            this.ProcessObstacleFrustums(commandBuffer, matPro);
            commandBuffer.DrawMesh(combinedMesh, Matrix4x4.identity, MasterRangeMaterial, 0, 1, matPro);
        }

        public override void Dispose()
        {
            base.Dispose();
            this.BoxRangeBuffer.Dispose();
        }
    }
}
