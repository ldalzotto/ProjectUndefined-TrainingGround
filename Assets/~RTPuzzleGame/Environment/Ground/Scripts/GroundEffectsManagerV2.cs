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
        public ComputeShader RangeEdgeImageEffectComputeShader;

        public Material MasterRangeMaterial;
        public Material RangeBufferToMeshMaterial;
        #endregion

        #region External Dependencies
        private PuzzleGameConfigurationManager PuzzleGameConfigurationManager;
        private ObstaclesListenerManager ObstaclesListenerManager;
        private ObstacleFrustumCalculationManager ObstacleFrustumCalculationManager;
        #endregion

        #region Command Buffers
        private CommandBuffer rangeDrawCommand;
        private CommandBuffer releaseCommand;
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

        private List<CircleRangeBufferData> circleRangeBufferValues;
        private DynamicComputeBufferManager<CircleRangeBufferData> CircleRangeBuffer;

        private List<BoxRangeBufferData> boxRangeBufferValues;
        private DynamicComputeBufferManager<BoxRangeBufferData> BoxRangeBuffer;

        private List<FrustumRangeBufferData> frustumRangeBufferValues;
        private DynamicComputeBufferManager<FrustumRangeBufferData> FrustumRangeBuffer;

        private List<RoundedFrustumRangeBufferData> roundedFrustumRangeBufferValues;
        private DynamicComputeBufferManager<RoundedFrustumRangeBufferData> RoundedRangeBuffer;

        private List<RangeExecutionOrderBufferData> rangeExecutionOrderBufferDataValues;
        private DynamicComputeBufferManager<RangeExecutionOrderBufferData> RangeExecutionOrderBuffer;

        private DynamicComputeBufferManager<FrustumPointsPositions> FrustumBufferManager;
        private DynamicComputeBufferManager<RangeToFrustumBufferLink> RangeToFrustumBufferLinkManager;
        private List<RangeToFrustumBufferLink> rangeToFrustumBufferLinkValues;
        private Dictionary<ObstacleListener, List<int>> ComputedFrustumPointsWorldPositionsIndexes;
        private Dictionary<RangeExecutionOrderBufferData, IAbstractGroundEffectManager> rangeExecutionOrderToGroundEffectManager;
#if UNITY_EDITOR
        //buffer values data retrieval
        public List<CircleRangeBufferData> CircleRangeBufferValues { get => circleRangeBufferValues; }
        public List<BoxRangeBufferData> BoxRangeBufferValues { get => boxRangeBufferValues; }
        public List<FrustumRangeBufferData> FrustumRangeBufferValues { get => frustumRangeBufferValues; }
        public List<RoundedFrustumRangeBufferData> RoundedFrustumRangeBufferValues { get => roundedFrustumRangeBufferValues; }
        public List<RangeExecutionOrderBufferData> RangeExecutionOrderBufferDataValues { get => rangeExecutionOrderBufferDataValues; }
        public List<RangeToFrustumBufferLink> RangeToFrustumBufferLinkValues { get => rangeToFrustumBufferLinkValues; }
#endif

        public void Init(LevelZonesID currentLevelID)
        {
            #region Init Values
            this.rangeEffectManagers = new Dictionary<RangeTypeID, Dictionary<int, IAbstractGroundEffectManager>>();
            this.rangeExecutionOrderToGroundEffectManager = new Dictionary<RangeExecutionOrderBufferData, IAbstractGroundEffectManager>();
            this.circleRangeBufferValues = new List<CircleRangeBufferData>();
            this.boxRangeBufferValues = new List<BoxRangeBufferData>();
            this.frustumRangeBufferValues = new List<FrustumRangeBufferData>();
            this.roundedFrustumRangeBufferValues = new List<RoundedFrustumRangeBufferData>();
            this.rangeExecutionOrderBufferDataValues = new List<RangeExecutionOrderBufferData>();
            this.rangeToFrustumBufferLinkValues = new List<RangeToFrustumBufferLink>();
            this.ComputedFrustumPointsWorldPositionsIndexes = new Dictionary<ObstacleListener, List<int>>();
            #endregion

            #region External Dependencies
            PuzzleGameConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            ObstaclesListenerManager = GameObject.FindObjectOfType<ObstaclesListenerManager>();
            ObstacleFrustumCalculationManager = GameObject.FindObjectOfType<ObstacleFrustumCalculationManager>();
            #endregion

            var camera = Camera.main;

            this.rangeDrawCommand = new CommandBuffer();
            this.rangeDrawCommand.name = this.GetType().Name + "." + nameof(this.rangeDrawCommand);
            camera.AddCommandBuffer(CameraEvent.AfterForwardOpaque, this.rangeDrawCommand);

            this.releaseCommand = new CommandBuffer();
            this.releaseCommand.name = this.GetType().Name + "." + nameof(this.releaseCommand);
            camera.AddCommandBuffer(CameraEvent.AfterForwardOpaque, this.releaseCommand);

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

            this.CircleRangeBuffer = new DynamicComputeBufferManager<CircleRangeBufferData>(CircleRangeBufferData.GetByteSize(), "CircleRangeBuffer", string.Empty, new List<Material>() { this.MasterRangeMaterial }, BufferReAllocateStrategy.SUPERIOR_ONLY);
            this.BoxRangeBuffer = new DynamicComputeBufferManager<BoxRangeBufferData>(BoxRangeBufferData.GetByteSize(), "BoxRangeBuffer", string.Empty, new List<Material>() { this.MasterRangeMaterial }, BufferReAllocateStrategy.SUPERIOR_ONLY);
            this.FrustumRangeBuffer = new DynamicComputeBufferManager<FrustumRangeBufferData>(FrustumRangeBufferData.GetByteSize(), "FrustumRangeBuffer", string.Empty, new List<Material>() { this.MasterRangeMaterial }, BufferReAllocateStrategy.SUPERIOR_ONLY);
            this.RoundedRangeBuffer = new DynamicComputeBufferManager<RoundedFrustumRangeBufferData>(RoundedFrustumRangeBufferData.GetByteSize(), "RoundedFrustumRangeBuffer", string.Empty, new List<Material>() { this.MasterRangeMaterial }, BufferReAllocateStrategy.SUPERIOR_ONLY);
            this.RangeExecutionOrderBuffer = new DynamicComputeBufferManager<RangeExecutionOrderBufferData>(RangeExecutionOrderBufferData.GetByteSize(), "RangeExecutionOrderBuffer", string.Empty, new List<Material>() { this.MasterRangeMaterial }, BufferReAllocateStrategy.SUPERIOR_ONLY);

            this.FrustumBufferManager = new DynamicComputeBufferManager<FrustumPointsPositions>(FrustumPointsPositions.GetByteSize(), "FrustumBufferDataBuffer", "_FrustumBufferDataBufferCount", new List<Material>() { this.MasterRangeMaterial });
            this.RangeToFrustumBufferLinkManager = new DynamicComputeBufferManager<RangeToFrustumBufferLink>(RangeToFrustumBufferLink.GetByteSize(), "RangeToFrustumBufferLinkBuffer", "_RangeToFrustumBufferLinkCount", new List<Material>() { this.MasterRangeMaterial });

            //master range shader color level adjuster
            var LevelRangeEffectInherentData = PuzzleGameConfigurationManager.LevelConfiguration()[currentLevelID].LevelRangeEffectInherentData;
            this.MasterRangeMaterial.SetFloat("_AlbedoBoost", 1f + LevelRangeEffectInherentData.DeltaIntensity);
            this.MasterRangeMaterial.SetFloat("_RangeMixFactor", 0.5f + LevelRangeEffectInherentData.DeltaMixFactor);
        }

        public void Tick(float d)
        {
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

            this.circleRangeBufferValues.Clear();
            this.boxRangeBufferValues.Clear();
            this.frustumRangeBufferValues.Clear();
            this.roundedFrustumRangeBufferValues.Clear();
            this.rangeExecutionOrderBufferDataValues.Clear();
            this.rangeToFrustumBufferLinkValues.Clear();
            this.ComputedFrustumPointsWorldPositionsIndexes.Clear();
            this.rangeExecutionOrderToGroundEffectManager.Clear();

#if UNITY_EDITOR
            Profiler.BeginSample("FrustumBufferManagerTick");
#endif

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

#if UNITY_EDITOR
            Profiler.EndSample();
#endif

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
                        RangeExecutionOrderBufferData addedRangeExecutionOrderBufferData;

                        if (rangeEffectManager.GetType() == typeof(SphereGroundEffectManager))
                        {
                            var SphereGroundEffectManager = (SphereGroundEffectManager)rangeEffectManager;
                            var circleRangeBufferData = SphereGroundEffectManager.ToSphereBuffer();
                            this.circleRangeBufferValues.Add(circleRangeBufferData);
                            addedRangeExecutionOrderBufferData = new RangeExecutionOrderBufferData(0, this.circleRangeBufferValues.Count - 1);
                            this.rangeExecutionOrderBufferDataValues.Add(addedRangeExecutionOrderBufferData);
                        }
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
                        else
                        {
                            var roundedFrustumGroundEffectManager = ((RoundedFrustumGroundEffectManager)rangeEffectManager);
                            var roundedFrustumRangeBufferData = roundedFrustumGroundEffectManager.ToFrustumBuffer();
                            this.roundedFrustumRangeBufferValues.Add(roundedFrustumRangeBufferData);
                            addedRangeExecutionOrderBufferData = new RangeExecutionOrderBufferData(3, this.roundedFrustumRangeBufferValues.Count - 1);
                            this.rangeExecutionOrderBufferDataValues.Add(addedRangeExecutionOrderBufferData);
                        }

                        this.rangeExecutionOrderToGroundEffectManager[addedRangeExecutionOrderBufferData] = rangeEffectManager;

                        //Range to obstacle occlusion frustum link
                        if (rangeEffectManager.GetAssociatedRangeObject().IsOccludedByFrustum())
                        {
                            //(WARNING) - Obstacle frustum calculation for display is multithreaded. Thus, calculation result may not be available even though the obstacle has been detected
                            if (this.ComputedFrustumPointsWorldPositionsIndexes.ContainsKey(rangeEffectManager.GetAssociatedRangeObject().RangeObstacleListener.ObstacleListener))
                            {
                                foreach (var computedFrustumPointsWorldPositionsIndexe in this.ComputedFrustumPointsWorldPositionsIndexes[rangeEffectManager.GetAssociatedRangeObject().RangeObstacleListener.ObstacleListener])
                                {
                                    this.rangeToFrustumBufferLinkValues.Add(new RangeToFrustumBufferLink(addedRangeExecutionOrderBufferData.Index, addedRangeExecutionOrderBufferData.RangeType, computedFrustumPointsWorldPositionsIndexe));
                                }
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

            this.CircleRangeBuffer.Tick(d, (List<CircleRangeBufferData> CircleRangeBufferDatas) => { CircleRangeBufferDatas.AddRange(this.circleRangeBufferValues); });
            this.BoxRangeBuffer.Tick(d, (List<BoxRangeBufferData> BoxRangeBufferDatas) => { BoxRangeBufferDatas.AddRange(this.boxRangeBufferValues); });
            this.FrustumRangeBuffer.Tick(d, (List<FrustumRangeBufferData> FrustumRangeBufferDatas) => { FrustumRangeBufferDatas.AddRange(this.frustumRangeBufferValues); });
            this.RoundedRangeBuffer.Tick(d, (List<RoundedFrustumRangeBufferData> RoundedFrustumRangeBufferDatas) => { RoundedFrustumRangeBufferDatas.AddRange(this.roundedFrustumRangeBufferValues); });

            this.RangeExecutionOrderBuffer.Tick(d, (List<RangeExecutionOrderBufferData> RangeExecutionOrderBufferDatas) => { RangeExecutionOrderBufferDatas.AddRange(this.rangeExecutionOrderBufferDataValues); });

            this.RangeToFrustumBufferLinkManager.Tick(d, (List<RangeToFrustumBufferLink> RangeToFrustumBufferLinkDatas) => { RangeToFrustumBufferLinkDatas.AddRange(this.rangeToFrustumBufferLinkValues); });

            this.OnCommandBufferUpdate();
            #endregion

#if UNITY_EDITOR
            Profiler.EndSample();
#endif
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
            this.CircleRangeBuffer.IfNotNull(CircleRangeBuffer => CircleRangeBuffer.Dispose());
            this.BoxRangeBuffer.IfNotNull(BoxRangeBuffer => BoxRangeBuffer.Dispose());
            this.FrustumRangeBuffer.IfNotNull(FrustumRangeBuffer => FrustumRangeBuffer.Dispose());
            this.RoundedRangeBuffer.IfNotNull(FrustumRoundedRangeBuffer => FrustumRoundedRangeBuffer.Dispose());
            this.RangeExecutionOrderBuffer.IfNotNull(RangeExecutionOrderBuffer => RangeExecutionOrderBuffer.Dispose());
            this.FrustumBufferManager.IfNotNull(FrustumBufferManager => FrustumBufferManager.Dispose());
            this.RangeToFrustumBufferLinkManager.IfNotNull(RangeToFrustumBufferLinkManager => RangeToFrustumBufferLinkManager.Dispose());
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

        private void OnCommandBufferUpdate()
        {
            this.rangeDrawCommand.Clear();
            this.releaseCommand.Clear();

            if (this.rangeExecutionOrderBufferDataValues.Count > 0)
            {
                var rangeRenderBuffer = Shader.PropertyToID("_RangeRenderBuffer");
                this.rangeDrawCommand.GetTemporaryRT(rangeRenderBuffer, new RenderTextureDescriptor(Camera.main.pixelWidth, Camera.main.pixelHeight, RenderTextureFormat.ARGB64) { sRGB = false, autoGenerateMips = false, enableRandomWrite = true });
                this.rangeDrawCommand.SetRenderTarget(rangeRenderBuffer);
                this.rangeDrawCommand.ClearRenderTarget(true, true, MyColors.TransparentBlack);

                var tmpRangeRenderArrayBuffer = Shader.PropertyToID("_TmpRangeRenderArrayBuffer");
                this.rangeDrawCommand.GetTemporaryRTArray(tmpRangeRenderArrayBuffer, Camera.main.pixelWidth, Camera.main.pixelHeight, this.rangeExecutionOrderBufferDataValues.Count, 0, FilterMode.Point, RenderTextureFormat.ARGB64, RenderTextureReadWrite.Linear, 1, true);

                HashSet<GroundEffectType> invovledGroundEffectTypes = new HashSet<GroundEffectType>();
                Mesh combinedMesh = null;

                this.rangeDrawCommand.SetComputeTextureParam(this.RangeEdgeImageEffectComputeShader, this.RangeEdgeImageEffectComputeShader.FindKernel("CSMain"), rangeRenderBuffer, new RenderTargetIdentifier(rangeRenderBuffer));
                this.rangeDrawCommand.SetComputeTextureParam(this.RangeEdgeImageEffectComputeShader, this.RangeEdgeImageEffectComputeShader.FindKernel("CSMain"), tmpRangeRenderArrayBuffer, new RenderTargetIdentifier(tmpRangeRenderArrayBuffer));

                this.rangeDrawCommand.SetComputeIntParam(this.RangeEdgeImageEffectComputeShader, "TextureWidth", Camera.main.pixelWidth);
                this.rangeDrawCommand.SetComputeIntParam(this.RangeEdgeImageEffectComputeShader, "TextureHeight", Camera.main.pixelHeight);

                foreach (var rangeExecution in this.rangeExecutionOrderBufferDataValues)
                {
                    var groundEffectTypesToRender = this.rangeExecutionOrderToGroundEffectManager[rangeExecution].GroundEffectTypeToRender(this.AffectedGroundEffectsType);
                    this.rangeDrawCommand.SetRenderTarget(new RenderTargetIdentifier(tmpRangeRenderArrayBuffer, depthSlice: this.rangeExecutionOrderBufferDataValues.IndexOf(rangeExecution)));
                    this.rangeDrawCommand.ClearRenderTarget(true, true, MyColors.TransparentBlack);

                    this.rangeDrawCommand.SetGlobalInt("_ExecutionOrderIndex", this.rangeExecutionOrderBufferDataValues.IndexOf(rangeExecution));

                    combinedMesh = new Mesh();
                    combinedMesh.CombineMeshes(groundEffectTypesToRender.ConvertAll(groundEffectType => new CombineInstance() { mesh = groundEffectType.GroundEffectMesh, transform = groundEffectType.transform.localToWorldMatrix }).ToArray(), true);

                    this.rangeDrawCommand.DrawMesh(combinedMesh, Matrix4x4.identity, this.MasterRangeMaterial, 0, rangeExecution.RangeType);

                    foreach (var meshToRender in groundEffectTypesToRender)
                    {
                        invovledGroundEffectTypes.Add(meshToRender);
                    }
                }

                this.rangeDrawCommand.DispatchCompute(this.RangeEdgeImageEffectComputeShader, this.RangeEdgeImageEffectComputeShader.FindKernel("CSMain"), Camera.main.pixelWidth / 8, Camera.main.pixelHeight / 8, this.rangeExecutionOrderBufferDataValues.Count);

                this.rangeDrawCommand.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
                combinedMesh = new Mesh();

                //We combine the ground and ignored ground objects
                combinedMesh.CombineMeshes(
                    invovledGroundEffectTypes.ToList().ConvertAll(groundEffectType => groundEffectType.GetCombineInstances()).SelectMany(s => s)
                    .ToArray(), true);

                this.rangeDrawCommand.DrawMesh(combinedMesh, Matrix4x4.identity, this.RangeBufferToMeshMaterial);

                this.releaseCommand.ReleaseTemporaryRT(rangeRenderBuffer);
                this.releaseCommand.ReleaseTemporaryRT(tmpRangeRenderArrayBuffer);
            }
        }

    }

    public struct RangeExecutionOrderBufferData
    {
        public int RangeType; //0 -> sphere, 1 -> box, 2 -> frustum
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

    public struct RangeToFrustumBufferLink
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
