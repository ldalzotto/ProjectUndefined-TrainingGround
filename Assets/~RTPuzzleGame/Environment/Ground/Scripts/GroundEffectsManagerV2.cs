using CoreGame;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace RTPuzzle
{
    public class GroundEffectsManagerV2 : MonoBehaviour
    {

        public Material MasterRangeMaterial;

        #region External Dependencies
        private PuzzleGameConfigurationManager PuzzleGameConfigurationManager;
        #endregion

        private CommandBuffer command;
        private GroundEffectType[] AffectedGroundEffectsType;
        private HashSet<MeshRenderer> RenderedRenderers = new HashSet<MeshRenderer>();

        private Dictionary<RangeTypeID, SphereGroundEffectManager> rangeEffectManagers = new Dictionary<RangeTypeID, SphereGroundEffectManager>();

        private const int RangeEffectCount = 4;
        private List<RangeTypeID> rangeEffectRenderOrder = new List<RangeTypeID>() {
            RangeTypeID.ATTRACTIVE_OBJECT_ACTIVE,
            RangeTypeID.ATTRACTIVE_OBJECT,
            RangeTypeID.LAUNCH_PROJECTILE,
            RangeTypeID.LAUNCH_PROJECTILE_CURSOR
        };

        private List<CircleRangeBufferData> CircleRangeBufferValues = new List<CircleRangeBufferData>();
        private ComputeBuffer CircleRangeBuffer;

        private List<RangeExecutionOrderBufferData> RangeExecutionOrderBufferDataValues = new List<RangeExecutionOrderBufferData>();
        private ComputeBuffer RangeExecutionOrderBuffer;

        public void Init()
        {

            #region External Dependencies
            PuzzleGameConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
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
            
            this.CircleRangeBuffer = new ComputeBuffer(RangeEffectCount, ((3 + 1 + 4 + 1 + 1) * sizeof(float)));
            this.CircleRangeBuffer.SetData(this.CircleRangeBufferValues);
            this.MasterRangeMaterial.SetBuffer("CircleRangeBuffer", this.CircleRangeBuffer);

            this.RangeExecutionOrderBuffer = new ComputeBuffer(RangeEffectCount, 3 * sizeof(int));
            this.RangeExecutionOrderBuffer.SetData(this.RangeExecutionOrderBufferDataValues);
            this.MasterRangeMaterial.SetBuffer("RangeExecutionOrderBuffer", this.RangeExecutionOrderBuffer);
        }

        public void Tick(float d)
        {
            this.RenderedRenderers.Clear();
            foreach (var groundEffectManager in this.rangeEffectManagers.Values)
            {
                if (groundEffectManager != null)
                {
                    groundEffectManager.Tick(d);
                    groundEffectManager.MeshToRender(ref this.RenderedRenderers, this.AffectedGroundEffectsType);
                }
            }

            this.CircleRangeBufferValues.Clear();
            this.RangeExecutionOrderBufferDataValues.Clear();
            foreach (var rangeEffectId in this.rangeEffectRenderOrder)
            {
                if (this.rangeEffectManagers.ContainsKey(rangeEffectId))
                {
                    this.CircleRangeBufferValues.Add(this.rangeEffectManagers[rangeEffectId].ToSphereBuffer());
                    this.RangeExecutionOrderBufferDataValues.Add(new RangeExecutionOrderBufferData(1,0, this.CircleRangeBufferValues.Count -1));
                }
            }

            this.CircleRangeBuffer.SetData(this.CircleRangeBufferValues);
            this.RangeExecutionOrderBuffer.SetData(this.RangeExecutionOrderBufferDataValues);
            
            this.OnCommandBufferUpdate();
        }

        #region External events
        public void OnRangeAdded(RangeType rangeType)
        {
            if (rangeType.IsRangeConfigurationDefined())
            {
                var sphereRangeType = (SphereRangeType)rangeType;
                this.rangeEffectManagers[rangeType.RangeTypeID] = new SphereGroundEffectManager(PuzzleGameConfigurationManager.RangeTypeConfiguration()[rangeType.RangeTypeID]);
                this.rangeEffectManagers[rangeType.RangeTypeID].OnAttractiveObjectActionStart(sphereRangeType);
            }
        }

        internal void OnRangeDestroy(RangeType rangeType)
        {
            if (rangeType.IsRangeConfigurationDefined())
            {
                this.rangeEffectManagers[rangeType.RangeTypeID].OnAttractiveObjectActionEnd();
                this.rangeEffectManagers.Remove(rangeType.RangeTypeID);
            }
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
                    this.rangeEffectManagers[rangeEffectId].MeshToRender(ref this.RenderedRenderers, this.AffectedGroundEffectsType);
                }
            }

            foreach (var meshToRender in this.RenderedRenderers)
            {
                this.command.DrawRenderer(meshToRender, this.MasterRangeMaterial);
            }
        }
    }

    class SphereGroundEffectManager
    {
        private RangeTypeInherentConfigurationData rangeTypeInherentConfigurationData;
        private FloatAnimation rangeAnimation;
        private SphereRangeType associatedSphereRange;

        public SphereGroundEffectManager(RangeTypeInherentConfigurationData rangeTypeInherentConfigurationData)
        {
            this.rangeTypeInherentConfigurationData = rangeTypeInherentConfigurationData;
            this.isAttractiveObjectRangeEnabled = false;
        }

        private bool isAttractiveObjectRangeEnabled;

        public bool IsAttractiveObjectRangeEnabled { get => isAttractiveObjectRangeEnabled; }
        public RangeTypeID GetRangeTypeID()
        {
            return this.associatedSphereRange.RangeTypeID;
        }

        public void Tick(float d)
        {
            if (isAttractiveObjectRangeEnabled)
            {
                this.rangeAnimation.Tick(d);
            }
        }

        public void MeshToRender(ref HashSet<MeshRenderer> renderers, GroundEffectType[] affectedGroundEffectsType)
        {
            foreach (var affectedGroundEffectType in affectedGroundEffectsType)
            {
                if (affectedGroundEffectType.MeshRenderer.isVisible
                    && this.associatedSphereRange.GetCollider().bounds.Intersects(affectedGroundEffectType.MeshRenderer.bounds)) //render only intersected geometry
                {
                    renderers.Add(affectedGroundEffectType.MeshRenderer);
                }
            }
        }

        public CircleRangeBufferData ToSphereBuffer()
        {
            CircleRangeBufferData CircleRangeBufferData = new CircleRangeBufferData();
            CircleRangeBufferData.CenterWorldPosition = this.associatedSphereRange.GetCenterWorldPos();
            CircleRangeBufferData.Radius = this.rangeAnimation.CurrentValue;
            if (this.rangeTypeInherentConfigurationData.RangeColorProvider != null)
            {
                CircleRangeBufferData.AuraColor = this.rangeTypeInherentConfigurationData.RangeColorProvider.Invoke();
            }
            else
            {
                CircleRangeBufferData.AuraColor = this.rangeTypeInherentConfigurationData.RangeBaseColor;
            }
            CircleRangeBufferData.AuraTextureAlbedoBoost = 0.2f;
            CircleRangeBufferData.AuraAnimationSpeed = 20f;
            return CircleRangeBufferData;
        }

        internal void OnAttractiveObjectActionStart(SphereRangeType sphereRangeType)
        {
            this.rangeAnimation = new FloatAnimation(sphereRangeType.GetRadiusRange(), rangeTypeInherentConfigurationData.RangeAnimationSpeed, 0f);
            this.isAttractiveObjectRangeEnabled = true;
            this.associatedSphereRange = sphereRangeType;
            this.Tick(0);
        }

        internal void OnAttractiveObjectActionEnd()
        {
            this.isAttractiveObjectRangeEnabled = false;
        }
    }

    struct RangeExecutionOrderBufferData
    {
        public int IsSphere;
        public int IsCube;
        public int Index;

        public RangeExecutionOrderBufferData(int isSphere, int isCube, int index)
        {
            IsSphere = isSphere;
            IsCube = isCube;
            Index = index;
        }
    }

    struct CircleRangeBufferData
    {
        public Vector3 CenterWorldPosition;
        public float Radius;
        public Vector4 AuraColor;
        public float AuraTextureAlbedoBoost;
        public float AuraAnimationSpeed;
    }
}
