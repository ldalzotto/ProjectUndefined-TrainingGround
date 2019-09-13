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

        private Dictionary<RangeTypeID, Dictionary<int, IAbstractGroundEffectManager>> rangeEffectManagers;

        private List<RangeTypeID> rangeEffectRenderOrder = new List<RangeTypeID>() {
            RangeTypeID.ATTRACTIVE_OBJECT_ACTIVE,
            RangeTypeID.ATTRACTIVE_OBJECT,
            RangeTypeID.SIGHT_VISION,
            RangeTypeID.LAUNCH_PROJECTILE,
            RangeTypeID.LAUNCH_PROJECTILE_CURSOR,
            RangeTypeID.TARGET_ZONE
        };

        private List<CircleRangeRenderData> CircleRangeRenderDatas;
        private List<RoundedFrustumRenderData> RoundedFrustumRenderDatas;

        public void Init(LevelZonesID currentLevelID)
        {
            #region Init Values
            this.rangeEffectManagers = new Dictionary<RangeTypeID, Dictionary<int, IAbstractGroundEffectManager>>();
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

            this.CircleRangeRenderDatas = new List<CircleRangeRenderData>();
            this.RoundedFrustumRenderDatas = new List<RoundedFrustumRenderData>();

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
            foreach (var CircleRangeRenderData in this.CircleRangeRenderDatas)
            {
                CircleRangeRenderData.Dispose();
            }
            this.CircleRangeRenderDatas.Clear();

            foreach (var RoundedFrustumRenderData in this.RoundedFrustumRenderDatas)
            {
                RoundedFrustumRenderData.Dispose();
            }
            this.RoundedFrustumRenderDatas.Clear();

#if UNITY_EDITOR
            Profiler.BeginSample("GroundEffectsManagerV2Tick");
#endif
            foreach (var groundEffectManager in this.rangeEffectManagers.Values.SelectMany(kv => kv.Values))
            {
                if (groundEffectManager != null)
                {
                    groundEffectManager.Tick(d);
                }
            }

            #region RangeBufferManagerTick 
#if UNITY_EDITOR
            Profiler.BeginSample("RangeBufferManagerTick");
#endif

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

                            var rangeObjectListener = SphereGroundEffectManager.GetAssociatedRangeObject().RangeObstacleListener;
                            if (rangeObjectListener != null)
                            {
                                this.CircleRangeRenderDatas.Add(new CircleRangeRenderData(circleRangeBufferData, rangeObjectListener.GetCalculatedFrustums(), SphereGroundEffectManager));
                            }
                        }
                        /*
                        else if (rangeEffectManager.GetType() == typeof(BoxGroundEffectManager))
                        {
                            this.boxRangeBufferValues.Add(((BoxGroundEffectManager)rangeEffectManager).ToBoxBuffer());
                            addedRangeExecutionOrderBufferData = new RangeExecutionOrderBufferData(1, this.boxRangeBufferValues.Count - 1);
                            this.rangeExecutionOrderBufferDataValues.Add(addedRangeExecutionOrderBufferData);
                        }
                        else if (rangeEffectManager.GetType() == typeof(FrustumGroundEffectManager))
                        {
                            var FrustumGroundEffectManager = ((FrustumGroundEffectManager)rangeEffectManager);
                            var frustumRangeBufferData = FrustumGroundEffectManager.ToFrustumBuffer();
                            this.frustumRangeBufferValues.Add(frustumRangeBufferData);
                            addedRangeExecutionOrderBufferData = new RangeExecutionOrderBufferData(2, this.frustumRangeBufferValues.Count - 1);
                            this.rangeExecutionOrderBufferDataValues.Add(addedRangeExecutionOrderBufferData);
                        }
                        */
                        else if (rangeEffectManager.GetType() == typeof(RoundedFrustumGroundEffectManager))
                        {
                            var roundedFrustumGroundEffectManager = ((RoundedFrustumGroundEffectManager)rangeEffectManager);
                            var roundedFrustumRangeBufferData = roundedFrustumGroundEffectManager.ToFrustumBuffer();

                            var rangeObjectListener = roundedFrustumGroundEffectManager.GetAssociatedRangeObject().RangeObstacleListener;
                            if (rangeObjectListener != null)
                            {
                                this.RoundedFrustumRenderDatas.Add(new RoundedFrustumRenderData(roundedFrustumRangeBufferData, rangeObjectListener.GetCalculatedFrustums(), roundedFrustumGroundEffectManager));
                            }
                        }
                    }
                }
            }
#if UNITY_EDITOR
            Profiler.EndSample();
#endif
            #endregion

            #region Buffer data set
            this.OnCommandBufferUpdate();
            #endregion

#if UNITY_EDITOR
            Profiler.EndSample();
#endif
        }


        private void OnCommandBufferUpdate()
        {
            this.rangeDrawCommand.Clear();
            this.rangeDrawCommand.BeginSample("rangeDrawCommand");


            foreach (var CircleRangeRenderData in this.CircleRangeRenderDatas)
            {
                CircleRangeRenderData.ProcessCommandBuffer(this.rangeDrawCommand, ref this.AffectedGroundEffectsType, this.MasterRangeMaterial);
            }
            foreach (var RoundedFrustumRenderData in this.RoundedFrustumRenderDatas)
            {
                RoundedFrustumRenderData.ProcessCommandBuffer(this.rangeDrawCommand, ref this.AffectedGroundEffectsType, this.MasterRangeMaterial);
            }

            this.rangeDrawCommand.EndSample("rangeDrawCommand");
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
                else if (rangeTypeObject.RangeType.GetType() == typeof(RoundedFrustumRangeType))
                {
                    var roundedFrustumRangeType = (RoundedFrustumRangeType)rangeTypeObject.RangeType;
                    var addedRange = (IAbstractGroundEffectManager)new RoundedFrustumGroundEffectManager(PuzzleGameConfigurationManager.RangeTypeConfiguration()[rangeTypeObject.RangeType.RangeTypeID]);
                    this.rangeEffectManagers[rangeTypeObject.RangeType.RangeTypeID].Add(rangeTypeObject.GetInstanceID(), addedRange);
                    addedRange.OnRangeCreated(rangeTypeObject);
                }
            }
        }

        public void OnLevelExit()
        {
            //release buffers
            //TODO
        }

        public void OnRangeDestroy(RangeTypeObject rangeTypeObject)
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

    }
    
    public class CircleRangeRenderData
    {
        private CircleRangeBufferData CircleRangeBufferData;
        private List<FrustumPointsPositions> ObstacleFrustums;
        private SphereGroundEffectManager SphereGroundEffectManager;

        private ComputeBuffer CircleRangeBuffer;
        private ComputeBuffer ObstacleFrustumBuffer;

        public CircleRangeRenderData(CircleRangeBufferData circleRangeBufferData, List<FrustumPointsPositions> obstacleFrustums, SphereGroundEffectManager sphereGroundEffectManager)
        {
            CircleRangeBufferData = circleRangeBufferData;
            ObstacleFrustums = obstacleFrustums;
            SphereGroundEffectManager = sphereGroundEffectManager;
        }

        public void ProcessCommandBuffer(CommandBuffer commandBuffer, ref List<GroundEffectType> AffectedGroundEffectsType, Material MasterRangeMaterial)
        {
            Mesh combinedMesh = new Mesh();
            combinedMesh.CombineMeshes(
                    this.SphereGroundEffectManager.GroundEffectTypeToRender(AffectedGroundEffectsType).ConvertAll(groundEffectType => new CombineInstance() { mesh = groundEffectType.GroundEffectMesh, transform = groundEffectType.transform.localToWorldMatrix }).ToArray(),
                    true
            );
            var matPro = new MaterialPropertyBlock();

            this.CircleRangeBuffer = new ComputeBuffer(1, CircleRangeBufferData.GetByteSize());
            this.CircleRangeBuffer.SetData(new List<CircleRangeBufferData>() { this.CircleRangeBufferData });
            matPro.SetBuffer("CircleRangeBuffer", this.CircleRangeBuffer);

            if (this.ObstacleFrustums != null && this.ObstacleFrustums.Count > 0)
            {
                this.ObstacleFrustumBuffer = new ComputeBuffer(this.ObstacleFrustums.Count, FrustumPointsPositions.GetByteSize());
                this.ObstacleFrustumBuffer.SetData(this.ObstacleFrustums);
                matPro.SetBuffer("FrustumBufferDataBuffer", this.ObstacleFrustumBuffer);
            }

            matPro.SetInt("_FrustumBufferDataBufferCount", this.ObstacleFrustums.Count);

            commandBuffer.DrawMesh(combinedMesh, Matrix4x4.identity, MasterRangeMaterial, 0, 0, matPro);

        }

        public void Dispose()
        {
            if (this.CircleRangeBuffer != null && this.CircleRangeBuffer.IsValid())
            {
                this.CircleRangeBuffer.Release();
            }
            if (this.ObstacleFrustumBuffer != null && this.ObstacleFrustumBuffer.IsValid())
            {
                this.ObstacleFrustumBuffer.Release();
            }
        }
    }

    public class RoundedFrustumRenderData
    {
        private RoundedFrustumRangeBufferData RoundedFrustumRangeBufferData;
        private List<FrustumPointsPositions> ObstacleFrustums;
        private RoundedFrustumGroundEffectManager RoundedFrustumGroundEffectManager;

        private ComputeBuffer RoundedFrustumRangeBuffer;
        private ComputeBuffer ObstacleFrustumBuffer;

        public RoundedFrustumRenderData(RoundedFrustumRangeBufferData roundedFrustumRangeBufferData, List<FrustumPointsPositions> obstacleFrustums, RoundedFrustumGroundEffectManager roundedFrustumGroundEffectManager)
        {
            RoundedFrustumRangeBufferData = roundedFrustumRangeBufferData;
            ObstacleFrustums = obstacleFrustums;
            RoundedFrustumGroundEffectManager = roundedFrustumGroundEffectManager;
        }

        public void ProcessCommandBuffer(CommandBuffer commandBuffer, ref List<GroundEffectType> AffectedGroundEffectsType, Material MasterRangeMaterial)
        {
            Mesh combinedMesh = new Mesh();
            combinedMesh.CombineMeshes(
                    this.RoundedFrustumGroundEffectManager.GroundEffectTypeToRender(AffectedGroundEffectsType).ConvertAll(groundEffectType => new CombineInstance() { mesh = groundEffectType.GroundEffectMesh, transform = groundEffectType.transform.localToWorldMatrix }).ToArray(),
                    true
            );
            var matPro = new MaterialPropertyBlock();

            this.RoundedFrustumRangeBuffer = new ComputeBuffer(1, RoundedFrustumRangeBufferData.GetByteSize());
            this.RoundedFrustumRangeBuffer.SetData(new List<RoundedFrustumRangeBufferData>() { this.RoundedFrustumRangeBufferData });
            matPro.SetBuffer("RoundedFrustumRangeBuffer", this.RoundedFrustumRangeBuffer);

            if (this.ObstacleFrustums != null && this.ObstacleFrustums.Count > 0)
            {
                this.ObstacleFrustumBuffer = new ComputeBuffer(this.ObstacleFrustums.Count, FrustumPointsPositions.GetByteSize());
                this.ObstacleFrustumBuffer.SetData(this.ObstacleFrustums);
                matPro.SetBuffer("FrustumBufferDataBuffer", this.ObstacleFrustumBuffer);
            }

            matPro.SetInt("_FrustumBufferDataBufferCount", this.ObstacleFrustums.Count);

            commandBuffer.DrawMesh(combinedMesh, Matrix4x4.identity, MasterRangeMaterial, 0, 3, matPro);
        }

        public void Dispose()
        {
            if (this.RoundedFrustumRangeBuffer != null && this.RoundedFrustumRangeBuffer.IsValid())
            {
                this.RoundedFrustumRangeBuffer.Release();
            }
            if (this.ObstacleFrustumBuffer != null && this.ObstacleFrustumBuffer.IsValid())
            {
                this.ObstacleFrustumBuffer.Release();
            }
        }
    }
}
