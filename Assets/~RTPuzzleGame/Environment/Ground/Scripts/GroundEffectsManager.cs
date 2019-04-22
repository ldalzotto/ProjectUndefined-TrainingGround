using CoreGame;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace RTPuzzle
{
    public class GroundEffectsManager : MonoBehaviour
    {

        public const string AURA_RADIUS_MATERIAL_PROPERTY = "_Radius";
        public const string AURA_CENTER_MATERIAL_PROPERTY = "_CenterWorldPosition";
        public const string AURA_COLOR_MATERIAL_PROPERTY = "_AuraColor";

        #region External Dependencies
        private PuzzleGameConfigurationManager PuzzleGameConfigurationManager;
        #endregion

        public GroundEffectsManagerComponent GroundEffectsManagerComponent;
        public ThrowCursorRangeEffectManagerComponent ThrowCursorRangeEffectManagerComponent;
        public AttractiveObjectRangeManagerComponent AttractiveObjectRangeManagerComponent;

        private GroundEffectsCommandBufferManager GroundEffectsCommandBufferManager;
        private ThrowRangeEffectManager ThrowRangeEffectManager;
        private ThrowCursorRangeEffectManager ThrowCursorRangeEffectManager;
        private AttractiveObjectRangeManager AttractiveObjectRangeManager;


        private GroundEffectType[] AffectedGroundEffectsType;

        public void Init()
        {

            #region External Dependencies
            PuzzleGameConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            #endregion

            var camera = Camera.main;

            this.GroundEffectsCommandBufferManager = new GroundEffectsCommandBufferManager(camera);

            ThrowRangeEffectManager = new ThrowRangeEffectManager(GroundEffectsManagerComponent);
            ThrowCursorRangeEffectManager = new ThrowCursorRangeEffectManager(ThrowCursorRangeEffectManagerComponent, this);
            AttractiveObjectRangeManager = new AttractiveObjectRangeManager(AttractiveObjectRangeManagerComponent);

            AffectedGroundEffectsType = GetComponentsInChildren<GroundEffectType>();
            for (var i = 0; i < AffectedGroundEffectsType.Length; i++)
            {
                AffectedGroundEffectsType[i].Init();
            }
        }

        public void Tick(float d)
        {
            ThrowRangeEffectManager.Tick(d);
            ThrowCursorRangeEffectManager.Tick(d);
            AttractiveObjectRangeManager.Tick(d);
        }

        #region External Events
        internal void OnProjectileThrowedEvent()
        {
            ThrowRangeEffectManager.OnThrowProjectileThrowed();
            ThrowCursorRangeEffectManager.OnThrowProjectileThrowed();
            OnCommandBufferUpdate();
        }

        internal void OnThrowProjectileActionStart(ThrowProjectileActionStartEvent throwProjectileActionStartEvent)
        {
            ThrowRangeEffectManager.OnThrowProjectileActionStart(throwProjectileActionStartEvent.ThrowerTransform, throwProjectileActionStartEvent.MaxRange);
            ThrowCursorRangeEffectManager.OnThrowProjectileActionStart(throwProjectileActionStartEvent.CurrentCursorPositionRetriever, PuzzleGameConfigurationManager.ProjectileConf()[throwProjectileActionStartEvent.ProjectileInvolved].EffectRange);
            OnCommandBufferUpdate();
        }
        internal void OnThrowProjectileCursorAvailable()
        {
            ThrowCursorRangeEffectManager.OnThrowProjectileCursorAvailable();
            OnCommandBufferUpdate();
        }

        internal void OnThrowProjectileCursorNotAvailable()
        {
            ThrowCursorRangeEffectManager.OnThrowProjectileCursorNotAvailable();
            OnCommandBufferUpdate();
        }
        public void OnThrowProjectileCursorOnProjectileRange()
        {
            ThrowCursorRangeEffectManager.OnThrowProjectileCursorOnProjectileRange();
        }
        public void OnThrowProjectileCursorOutOfProjectileRange()
        {
            ThrowCursorRangeEffectManager.OnThrowProjectileCursorOutOfProjectileRange();
        }
        internal void OnAttractiveObjectActionStart(AttractiveObjectInherentConfigurationData attractiveObjectConfigurationData, Transform playerTransform)
        {
            AttractiveObjectRangeManager.OnAttractiveObjectActionStart(attractiveObjectConfigurationData, playerTransform);
            OnCommandBufferUpdate();
        }
        internal void OnAttractiveObjectActionEnd()
        {
            AttractiveObjectRangeManager.OnAttractiveObjectActionEnd();
            OnCommandBufferUpdate();
        }
        internal void OnCommandBufferUpdate()
        {
            GroundEffectsCommandBufferManager.ClearCommandBuffer();
            ThrowRangeEffectManager.OnCommandBufferUpdate(GroundEffectsCommandBufferManager.CommandBuffer, AffectedGroundEffectsType);
            ThrowCursorRangeEffectManager.OnCommandBufferUpdate(GroundEffectsCommandBufferManager.CommandBuffer, AffectedGroundEffectsType);
            AttractiveObjectRangeManager.OnCommandBufferUpdate(GroundEffectsCommandBufferManager.CommandBuffer, AffectedGroundEffectsType);
        }
        #endregion
    }

    #region Effect Command Buffer Manager
    class GroundEffectsCommandBufferManager
    {
        private Camera camera;

        public GroundEffectsCommandBufferManager(Camera camera)
        {
            this.camera = camera;
            this.commandBuffer = new CommandBuffer();
            this.commandBuffer.name = "GoundEffectsRender";
            this.camera.AddCommandBuffer(CameraEvent.AfterForwardOpaque, this.commandBuffer);
        }

        private CommandBuffer commandBuffer;

        public CommandBuffer CommandBuffer { get => commandBuffer; }

        public void ClearCommandBuffer()
        {
            commandBuffer.Clear();
        }

    }
    #endregion

    #region Throw range effect manager
    class ThrowRangeEffectManager
    {
        private GroundEffectsManagerComponent GroundEffectsManagerComponent;


        private bool throwRangeEnabled;
        private FloatAnimation rangeAnimation;
        private Transform throwerTransformRef;

        public bool ThrowRangeEnabled { get => throwRangeEnabled; }

        public ThrowRangeEffectManager(GroundEffectsManagerComponent rTPuzzleGroundEffectsManagerComponent)
        {
            GroundEffectsManagerComponent = rTPuzzleGroundEffectsManagerComponent;

        }

        public void Tick(float d)
        {
            if (throwRangeEnabled)
            {
                this.rangeAnimation.Tick(d);
                GroundEffectsManagerComponent.RangeEffectMaterial.SetFloat(GroundEffectsManager.AURA_RADIUS_MATERIAL_PROPERTY, this.rangeAnimation.CurrentValue);
                GroundEffectsManagerComponent.RangeEffectMaterial.SetVector(GroundEffectsManager.AURA_CENTER_MATERIAL_PROPERTY, throwerTransformRef.position);
            }
        }

        public void OnThrowProjectileActionStart(Transform throwerTransform, float maxRange)
        {
            this.rangeAnimation = new FloatAnimation(maxRange, GroundEffectsManagerComponent.RangeExpandSpeed, 0f);
            throwerTransformRef = throwerTransform;
            throwRangeEnabled = true;
        }

        public void OnThrowProjectileThrowed()
        {
            throwRangeEnabled = false;
        }

        internal void OnCommandBufferUpdate(CommandBuffer commandBuffer, GroundEffectType[] affectedGroundEffectsType)
        {
            if (throwRangeEnabled)
            {
                foreach (var affectedGroundEffectType in affectedGroundEffectsType)
                {
                    commandBuffer.DrawRenderer(affectedGroundEffectType.MeshRenderer, GroundEffectsManagerComponent.RangeEffectMaterial, 0, 0);
                }
            }
        }
    }

    [System.Serializable]
    public class GroundEffectsManagerComponent
    {
        public Material RangeEffectMaterial;
        public float RangeExpandSpeed;
    }

    #endregion

    #region Throw cursor range effect manager
    class ThrowCursorRangeEffectManager
    {

        private ThrowCursorRangeEffectManagerComponent ThrowCursorRangeEffectManagerComponent;
        private GroundEffectsManager GroundEffectsManagerRef;

        public ThrowCursorRangeEffectManager(ThrowCursorRangeEffectManagerComponent throwCursorRangeEffectManagerComponent, GroundEffectsManager GroundEffectsManagerRef)
        {
            ThrowCursorRangeEffectManagerComponent = throwCursorRangeEffectManagerComponent;
            this.GroundEffectsManagerRef = GroundEffectsManagerRef;
        }

        private Func<Nullable<Vector3>> cursorPositionRetriever;

        private FloatAnimation rangeAnimation;
        private bool throwRangeEnabled;

        public bool ThrowRangeEnabled { get => throwRangeEnabled; }

        public void Tick(float d)
        {
            if (throwRangeEnabled)
            {
                this.rangeAnimation.Tick(d);
                var currentCursorPosition = cursorPositionRetriever.Invoke();
                if (currentCursorPosition.HasValue)
                {
                    ThrowCursorRangeEffectManagerComponent.ProjectileEffectRangeMaterial.SetFloat(GroundEffectsManager.AURA_RADIUS_MATERIAL_PROPERTY, this.rangeAnimation.CurrentValue);
                    ThrowCursorRangeEffectManagerComponent.ProjectileEffectRangeMaterial.SetVector("_CenterWorldPosition", currentCursorPosition.Value);
                }
                else
                {
                    this.throwRangeEnabled = false;
                    this.GroundEffectsManagerRef.OnCommandBufferUpdate();
                }
            }
        }

        public void OnThrowProjectileActionStart(Func<Nullable<Vector3>> cursorPositionRetriever, float projectileRange)
        {
            this.cursorPositionRetriever = cursorPositionRetriever;
            this.rangeAnimation = new FloatAnimation(projectileRange, ThrowCursorRangeEffectManagerComponent.RangeExpandSpeed, 0f);
            throwRangeEnabled = true;
        }

        public void OnThrowProjectileThrowed()
        {
            throwRangeEnabled = false;
        }

        internal void OnThrowProjectileCursorAvailable()
        {
            throwRangeEnabled = true;
        }

        internal void OnThrowProjectileCursorNotAvailable()
        {
            throwRangeEnabled = false;
        }

        internal void OnCommandBufferUpdate(CommandBuffer commandBuffer, GroundEffectType[] affectedGroundEffectsType)
        {
            if (throwRangeEnabled)
            {
                foreach (var affectedGroundEffectType in affectedGroundEffectsType)
                {
                    commandBuffer.DrawRenderer(affectedGroundEffectType.MeshRenderer, ThrowCursorRangeEffectManagerComponent.ProjectileEffectRangeMaterial, 0, 0);
                }
            }
        }

        internal void OnThrowProjectileCursorOnProjectileRange()
        {
            ThrowCursorRangeEffectManagerComponent.ProjectileEffectRangeMaterial.SetColor(GroundEffectsManager.AURA_COLOR_MATERIAL_PROPERTY, ThrowCursorRangeEffectManagerComponent.CursorOnRangeAuraColor);
        }

        internal void OnThrowProjectileCursorOutOfProjectileRange()
        {
            ThrowCursorRangeEffectManagerComponent.ProjectileEffectRangeMaterial.SetColor(GroundEffectsManager.AURA_COLOR_MATERIAL_PROPERTY, ThrowCursorRangeEffectManagerComponent.CursorOutOfRangeAuraColor);
        }
    }

    [System.Serializable]
    public class ThrowCursorRangeEffectManagerComponent
    {
        public Material ProjectileEffectRangeMaterial;
        [ColorUsage(true, true)]
        public Color CursorOnRangeAuraColor;
        [ColorUsage(true, true)]
        public Color CursorOutOfRangeAuraColor;
        public float RangeExpandSpeed;
    }
    #endregion


    #region Attractive Object Range
    class AttractiveObjectRangeManager
    {
        private AttractiveObjectRangeManagerComponent attractiveObjectRangeManagerComponent;
        private FloatAnimation rangeAnimation;
        private Transform playerTransform;

        public AttractiveObjectRangeManager(AttractiveObjectRangeManagerComponent attractiveObjectRangeManagerComponent)
        {
            this.attractiveObjectRangeManagerComponent = attractiveObjectRangeManagerComponent;
            this.isAttractiveObjectRangeEnabled = false;
        }

        private bool isAttractiveObjectRangeEnabled;

        internal void OnCommandBufferUpdate(CommandBuffer commandBuffer, GroundEffectType[] affectedGroundEffectsType)
        {
            if (isAttractiveObjectRangeEnabled)
            {
                foreach (var affectedGroundEffectType in affectedGroundEffectsType)
                {
                    commandBuffer.DrawRenderer(affectedGroundEffectType.MeshRenderer, attractiveObjectRangeManagerComponent.AttractiveObjectRangeMaterial, 0, 0);
                }
            }
        }

        public void Tick(float d)
        {
            if (isAttractiveObjectRangeEnabled)
            {
                this.rangeAnimation.Tick(d);
                this.attractiveObjectRangeManagerComponent.AttractiveObjectRangeMaterial.SetVector(GroundEffectsManager.AURA_CENTER_MATERIAL_PROPERTY, playerTransform.transform.position);
                this.attractiveObjectRangeManagerComponent.AttractiveObjectRangeMaterial.SetFloat(GroundEffectsManager.AURA_RADIUS_MATERIAL_PROPERTY, this.rangeAnimation.CurrentValue);
            }
        }

        internal void OnAttractiveObjectActionStart(AttractiveObjectInherentConfigurationData attractiveObjectConfigurationData, Transform playerTransform)
        {
            this.rangeAnimation = new FloatAnimation(attractiveObjectConfigurationData.EffectRange, attractiveObjectRangeManagerComponent.RangeAnimationSpeed, 0f);
            this.isAttractiveObjectRangeEnabled = true;
            this.playerTransform = playerTransform;
            this.Tick(0);
        }

        internal void OnAttractiveObjectActionEnd()
        {
            this.isAttractiveObjectRangeEnabled = false;
        }
    }

    [System.Serializable]
    public class AttractiveObjectRangeManagerComponent
    {
        public Material AttractiveObjectRangeMaterial;

        public float RangeAnimationSpeed;
    }
    #endregion
}

