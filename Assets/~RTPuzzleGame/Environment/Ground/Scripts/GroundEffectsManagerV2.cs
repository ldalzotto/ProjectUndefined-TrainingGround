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
        #region Materials
        public Material MasterRangeMaterial;
        #endregion

        #region External Dependencies
        private PuzzleGameConfigurationManager PuzzleGameConfigurationManager;
        private CoreMaterialConfiguration CoreMaterialConfiguration;

        private ObstaclesListenerManager ObstaclesListenerManager;
        private ObstacleFrustumCalculationManager ObstacleFrustumCalculationManager;
        #endregion

        #region Command Buffers
        private CommandBuffer rangeDrawCommand;
        #endregion

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
            ObstaclesListenerManager = PuzzleGameSingletonInstances.ObstaclesListenerManager; ;
            ObstacleFrustumCalculationManager = PuzzleGameSingletonInstances.ObstacleFrustumCalculationManager;
            this.CoreMaterialConfiguration = CoreGameSingletonInstances.CoreStaticConfigurationContainer.CoreStaticConfiguration.CoreMaterialConfiguration;
            #endregion

            var camera = Camera.main;

            this.rangeDrawCommand = new CommandBuffer();
            this.rangeDrawCommand.name = this.GetType().Name + "." + nameof(this.rangeDrawCommand);
            camera.AddCommandBuffer(CameraEvent.AfterForwardOpaque, this.rangeDrawCommand);

            AffectedGroundEffectsType = GetComponentsInChildren<GroundEffectType>().ToList();
            foreach (var affectedGroundEffectType in AffectedGroundEffectsType)
            {
                affectedGroundEffectType.Init();
            }


            //Do static batching of ground effects types
            StaticBatchingUtility.Combine(
                    AffectedGroundEffectsType.ConvertAll(groundEffectType => groundEffectType.gameObject)
                    .Union(AffectedGroundEffectsType.ConvertAll(groundEffectType => groundEffectType.AssociatedGroundEffectIgnoredGroundObjectType).SelectMany(s => s).ToList().ConvertAll(groundEffectIgnoredObject => groundEffectIgnoredObject.gameObject))
                    .ToArray(), this.gameObject);

            //master range shader color level adjuster
            var LevelRangeEffectInherentData = PuzzleGameConfigurationManager.LevelConfiguration()[currentLevelID].LevelRangeEffectInherentData;
            this.MasterRangeMaterial.SetFloat("_AlbedoBoost", 1f + LevelRangeEffectInherentData.DeltaIntensity);
            this.MasterRangeMaterial.SetFloat("_RangeMixFactor", 0.5f + LevelRangeEffectInherentData.DeltaMixFactor);
        }

        public void Tick(float d)
        {
            Profiler.BeginSample("GroundEffectsManagerV2Tick");

            foreach (var RangeRenderData in this.rangeRenderDatas.Values.SelectMany(kv => kv.Values))
            {
                if (RangeRenderData != null)
                {
                    RangeRenderData.Tick(d, this.AffectedGroundEffectsType);
                }
            }

            #region Buffer data set
            this.OnCommandBufferUpdate();
            #endregion

            Profiler.EndSample();

        }


        private void OnCommandBufferUpdate()
        {
            this.rangeDrawCommand.Clear();
            this.rangeDrawCommand.BeginSample("rangeDrawCommand");

            foreach (var RangeRenderData in this.rangeRenderDatas.Values.SelectMany(kv => kv.Values))
            {
                if (RangeRenderData != null)
                {
                    RangeRenderData.ProcessCommandBuffer(this.rangeDrawCommand, ref this.AffectedGroundEffectsType, this.MasterRangeMaterial);
                }
            }

            this.rangeDrawCommand.EndSample("rangeDrawCommand");
        }


        #region External events
        public void OnRangeAdded(RangeTypeObject rangeTypeObject)
        {
            if (rangeTypeObject.RangeType.IsRangeConfigurationDefined())
            {
                if (!this.rangeRenderDatas.ContainsKey(rangeTypeObject.RangeType.RangeTypeID))
                {
                    this.rangeRenderDatas[rangeTypeObject.RangeType.RangeTypeID] = new Dictionary<int, AbstractRangeRenderData>();
                }

                if (rangeTypeObject.RangeType.GetType() == typeof(SphereRangeType))
                {
                    var sphereRangeType = (SphereRangeType)rangeTypeObject.RangeType;
                    var addedRange = new SphereGroundEffectManager(PuzzleGameConfigurationManager.RangeTypeConfiguration()[rangeTypeObject.RangeType.RangeTypeID]);
                    addedRange.OnRangeCreated(rangeTypeObject);
                    this.rangeRenderDatas[rangeTypeObject.RangeType.RangeTypeID].Add(rangeTypeObject.GetInstanceID(), new CircleRangeRenderData(addedRange));
                }
                else if (rangeTypeObject.RangeType.GetType() == typeof(BoxRangeType))
                {
                    var boxRangeType = (BoxRangeType)rangeTypeObject.RangeType;
                    var addedRange = new BoxGroundEffectManager(PuzzleGameConfigurationManager.RangeTypeConfiguration()[rangeTypeObject.RangeType.RangeTypeID]);
                    addedRange.OnRangeCreated(rangeTypeObject);
                    this.rangeRenderDatas[rangeTypeObject.RangeType.RangeTypeID].Add(rangeTypeObject.GetInstanceID(), new BoxRangeRenderData(addedRange));
                }
                /*
                else if (rangeTypeObject.RangeType.GetType() == typeof(FrustumRangeType))
                {
                    var frustumRangeType = (FrustumRangeType)rangeTypeObject.RangeType;
                    var addedRange = (IAbstractGroundEffectManager)new FrustumGroundEffectManager(PuzzleGameConfigurationManager.RangeTypeConfiguration()[rangeTypeObject.RangeType.RangeTypeID]);
                    this.rangeEffectManagers[rangeTypeObject.RangeType.RangeTypeID].Add(rangeTypeObject.GetInstanceID(), addedRange);
                    addedRange.OnRangeCreated(rangeTypeObject);
                }
                */
                else if (rangeTypeObject.RangeType.GetType() == typeof(RoundedFrustumRangeType))
                {
                    var roundedFrustumRangeType = (RoundedFrustumRangeType)rangeTypeObject.RangeType;
                    var addedRange = new RoundedFrustumGroundEffectManager(PuzzleGameConfigurationManager.RangeTypeConfiguration()[rangeTypeObject.RangeType.RangeTypeID]);
                    addedRange.OnRangeCreated(rangeTypeObject);
                    this.rangeRenderDatas[rangeTypeObject.RangeType.RangeTypeID].Add(rangeTypeObject.GetInstanceID(), new RoundedFrustumRenderData(addedRange));
                }
            }
        }

        public void OnLevelExit()
        {
            //release buffers
            if (this.rangeRenderDatas != null)
            {
                foreach (var RangeRenderData in this.rangeRenderDatas.Values.SelectMany(kv => kv.Values))
                {
                    if (RangeRenderData != null)
                    {
                        RangeRenderData.Dispose();
                    }
                }
            }
            
        }

        public void OnRangeDestroy(RangeTypeObject rangeTypeObject)
        {
            if (rangeTypeObject.IsRangeConfigurationDefined())
            {
                this.rangeRenderDatas[rangeTypeObject.RangeType.RangeTypeID][rangeTypeObject.GetInstanceID()].OnRangeDestroyed();
                this.rangeRenderDatas.Remove(rangeTypeObject.RangeType.RangeTypeID);
            }
        }

        private void OnDestroy()
        {
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
            var rangeObjectListener = this.GroundEffectManager.GetAssociatedRangeObject().RangeObstacleListener;
            if (rangeObjectListener != null)
            {
                var obstacleFrustums = rangeObjectListener.GetCalculatedFrustums();
                this.obstacleFrustumBuffer.Tick((bufferData) => bufferData.AddRange(obstacleFrustums));
            }
        }

        public void Tick(float d, List<GroundEffectType> affectedGroundEffectsType)
        {
            this.GroundEffectManager.Tick(d, affectedGroundEffectsType);
        }

        public void OnRangeDestroyed()
        {
            this.GroundEffectManager.OnRangeDestroyed();
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
