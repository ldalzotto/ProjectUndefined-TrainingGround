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

        private List<RangeTypeID> rangeEffectRenderOrder = new List<RangeTypeID>() {
            RangeTypeID.ATTRACTIVE_OBJECT_ACTIVE,
            RangeTypeID.ATTRACTIVE_OBJECT,
            RangeTypeID.LAUNCH_PROJECTILE,
            RangeTypeID.LAUNCH_PROJECTILE_CURSOR
        };

        private CircleRangeBufferData[] CircleRangeBufferValues = new CircleRangeBufferData[4];
        private ComputeBuffer CircleRangeBuffer;

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

            for (var i = 0; i < this.rangeEffectRenderOrder.Count; i++)
            {
                this.CircleRangeBufferValues[i] = new CircleRangeBufferData();
            }

            this.CircleRangeBuffer = new ComputeBuffer(4, (1 * sizeof(int)) + ((3 + 1 + 4 + 1 + 1) * sizeof(float)));
            this.CircleRangeBuffer.SetData(this.CircleRangeBufferValues);
            this.MasterRangeMaterial.SetBuffer("CircleRangeBuffer", this.CircleRangeBuffer);
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

            foreach (var rangeEffectId in this.rangeEffectRenderOrder)
            {
                if (this.rangeEffectManagers.ContainsKey(rangeEffectId))
                {
                    this.rangeEffectManagers[rangeEffectId].UpdateCircleRangeBufferData(ref this.CircleRangeBufferValues[this.rangeEffectRenderOrder.IndexOf(rangeEffectId)]);
                }
            }

            this.CircleRangeBuffer.SetData(this.CircleRangeBufferValues);
            this.OnCommandBufferUpdate();
        }

        #region External events
        public void OnRangeAdded(RangeType rangeType)
        {
            if (rangeType.IsRangeConfigurationDefined())
            {
                var sphereRangeType = (SphereRangeType)rangeType;
                this.rangeEffectManagers[rangeType.RangeTypeID] = new SphereGroundEffectManager(PuzzleGameConfigurationManager.RangeTypeConfiguration()[rangeType.RangeTypeID]);
                this.rangeEffectManagers[rangeType.RangeTypeID].OnAttractiveObjectActionStart(sphereRangeType, ref this.CircleRangeBufferValues[this.rangeEffectRenderOrder.IndexOf(rangeType.RangeTypeID)]);
            }
        }

        internal void OnRangeDestroy(RangeType rangeType)
        {
            if (rangeType.IsRangeConfigurationDefined())
            {
                this.rangeEffectManagers[rangeType.RangeTypeID].OnAttractiveObjectActionEnd(ref this.CircleRangeBufferValues[this.rangeEffectRenderOrder.IndexOf(rangeType.RangeTypeID)]);
                this.rangeEffectManagers.Remove(rangeType.RangeTypeID);
            }
        }
        #endregion

        internal void OnCommandBufferUpdate()
        {
            this.command.Clear();
            this.MasterRangeMaterial.SetInt("_CountSize", this.rangeEffectRenderOrder.Count);
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

        public void UpdateCircleRangeBufferData(ref CircleRangeBufferData OldCircleRangeBufferData)
        {
            OldCircleRangeBufferData.Enabled = Convert.ToInt32(this.isAttractiveObjectRangeEnabled);
            OldCircleRangeBufferData.CenterWorldPosition = this.associatedSphereRange.GetCenterWorldPos();
            OldCircleRangeBufferData.Radius = this.rangeAnimation.CurrentValue;
            if (this.rangeTypeInherentConfigurationData.RangeColorProvider != null)
            {
                OldCircleRangeBufferData.AuraColor = this.rangeTypeInherentConfigurationData.RangeColorProvider.Invoke();
            }
            else
            {
                OldCircleRangeBufferData.AuraColor = this.rangeTypeInherentConfigurationData.RangeBaseColor;
            }
            OldCircleRangeBufferData.AuraTextureAlbedoBoost = 0.2f;
            OldCircleRangeBufferData.AuraAnimationSpeed = 20f;
        }

        internal void OnAttractiveObjectActionStart(SphereRangeType sphereRangeType, ref CircleRangeBufferData circleRangeBufferData)
        {
            this.rangeAnimation = new FloatAnimation(sphereRangeType.GetRadiusRange(), rangeTypeInherentConfigurationData.RangeAnimationSpeed, 0f);
            this.isAttractiveObjectRangeEnabled = true;
            circleRangeBufferData.Enabled = 1;
            this.associatedSphereRange = sphereRangeType;
            this.Tick(0);
        }

        internal void OnAttractiveObjectActionEnd(ref CircleRangeBufferData circleRangeBufferData)
        {
            circleRangeBufferData.Enabled = 0;
            this.isAttractiveObjectRangeEnabled = false;
        }
    }

    struct CircleRangeBufferData
    {
        public int Enabled;
        public Vector3 CenterWorldPosition;
        public float Radius;
        public Vector4 AuraColor;
        public float AuraTextureAlbedoBoost;
        public float AuraAnimationSpeed;

        public CircleRangeBufferData(int enabled, Vector3 centerWorldPosition, float radius, Vector4 auraColor, float auraTextureAlbedoBoost, float auraAnimationSpeed)
        {
            Enabled = enabled;
            CenterWorldPosition = centerWorldPosition;
            Radius = radius;
            AuraColor = auraColor;
            AuraTextureAlbedoBoost = auraTextureAlbedoBoost;
            AuraAnimationSpeed = auraAnimationSpeed;
        }
    }
}
