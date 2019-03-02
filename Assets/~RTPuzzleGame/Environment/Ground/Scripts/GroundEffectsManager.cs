using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace RTPuzzle
{
    public class GroundEffectsManager : MonoBehaviour
    {
        #region External Dependencies
        private PuzzleEventsManager PuzzleEventsManager;
        #endregion

        public GroundEffectsManagerComponent GroundEffectsManagerComponent;
        public ThrowCursorRangeEffectManagerComponent ThrowCursorRangeEffectManagerComponent;

        private GroundEffectsCommandBufferManager GroundEffectsCommandBufferManager;
        private ThrowRangeEffectManager ThrowRangeEffectManager;
        private ThrowCursorRangeEffectManager ThrowCursorRangeEffectManager;

        private GroundEffectType[] AffectedGroundEffectsType;

        public void Init()
        {
            var camera = Camera.main;

            this.GroundEffectsCommandBufferManager = new GroundEffectsCommandBufferManager(camera);

            ThrowRangeEffectManager = new ThrowRangeEffectManager(GroundEffectsManagerComponent);
            ThrowCursorRangeEffectManager = new ThrowCursorRangeEffectManager(ThrowCursorRangeEffectManagerComponent);
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
            ThrowCursorRangeEffectManager.OnThrowProjectileActionStart(throwProjectileActionStartEvent.CurrentCursorPositionRetriever, LaunchProjectileInherentDataConfiguration.conf[throwProjectileActionStartEvent.ProjectileInvolved].EffectRange);
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
        #endregion

        #region Internal Events
        private void OnCommandBufferUpdate()
        {
            GroundEffectsCommandBufferManager.ClearCommandBuffer();
            ThrowRangeEffectManager.OnCommandBufferUpdate(GroundEffectsCommandBufferManager.CommandBuffer, AffectedGroundEffectsType);
            ThrowCursorRangeEffectManager.OnCommandBufferUpdate(GroundEffectsCommandBufferManager.CommandBuffer, AffectedGroundEffectsType);
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
        private float currentRange;
        private float maxRange;
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
                currentRange += (d * GroundEffectsManagerComponent.RangeExpandSpeed);
                currentRange = Mathf.Min(currentRange, maxRange);
                GroundEffectsManagerComponent.RangeEffectMaterial.SetFloat("_Radius", currentRange);
                GroundEffectsManagerComponent.RangeEffectMaterial.SetVector("_CenterWorldPosition", throwerTransformRef.position);
                GroundEffectsManagerComponent.RangeEffectMaterial.SetPass(0);
            }
        }

        public void OnThrowProjectileActionStart(Transform throwerTransform, float maxRange)
        {
            this.maxRange = maxRange;
            this.currentRange = 0f;
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

        public ThrowCursorRangeEffectManager(ThrowCursorRangeEffectManagerComponent throwCursorRangeEffectManagerComponent)
        {
            ThrowCursorRangeEffectManagerComponent = throwCursorRangeEffectManagerComponent;
        }

        private Func<Nullable<Vector3>> cursorPositionRetriever;
        private float projectileRange;

        private bool throwRangeEnabled;

        public bool ThrowRangeEnabled { get => throwRangeEnabled; }

        public void Tick(float d)
        {
            if (throwRangeEnabled)
            {
                var currentCursorPosition = cursorPositionRetriever.Invoke();
                if (currentCursorPosition.HasValue)
                {
                    ThrowCursorRangeEffectManagerComponent.ProjectileEffectRangeMaterial.SetFloat("_Radius", projectileRange);
                    ThrowCursorRangeEffectManagerComponent.ProjectileEffectRangeMaterial.SetVector("_CenterWorldPosition", currentCursorPosition.Value);
                    ThrowCursorRangeEffectManagerComponent.ProjectileEffectRangeMaterial.SetPass(0);
                }else
                {
                    //TODO set event to update the command buffer instead of setting a radius of 0f
                    ThrowCursorRangeEffectManagerComponent.ProjectileEffectRangeMaterial.SetFloat("_Radius", 0f);
                }
            }
        }

        public void OnThrowProjectileActionStart(Func<Nullable<Vector3>> cursorPositionRetriever, float projectileRange)
        {
            this.cursorPositionRetriever = cursorPositionRetriever;
            this.projectileRange = projectileRange;
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

    }

    [System.Serializable]
    public class ThrowCursorRangeEffectManagerComponent
    {
        public Material ProjectileEffectRangeMaterial;
    }
    #endregion
}

