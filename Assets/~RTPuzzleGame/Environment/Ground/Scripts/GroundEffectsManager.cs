using System;
using UnityEngine;

namespace RTPuzzle
{
    public class GroundEffectsManager : MonoBehaviour, PuzzleEventsListener
    {
        #region External Dependencies
        private PuzzleEventsManager PuzzleEventsManager;
        #endregion

        public GroundEffectsManagerComponent GroundEffectsManagerComponent;
        public ThrowCursorRangeEffectManagerComponent ThrowCursorRangeEffectManagerComponent;

        private ThrowRangeEffectManager ThrowRangeEffectManager;
        private ThrowCursorRangeEffectManager ThrowCursorRangeEffectManager;

        private GroundEffectType[] AffectedGroundEffectsType;

        public void Init()
        {
            ThrowRangeEffectManager = new ThrowRangeEffectManager(GroundEffectsManagerComponent);
            ThrowCursorRangeEffectManager = new ThrowCursorRangeEffectManager(ThrowCursorRangeEffectManagerComponent);
            PuzzleEventsManager = GameObject.FindObjectOfType<PuzzleEventsManager>();
            AffectedGroundEffectsType = GetComponentsInChildren<GroundEffectType>();
            for (var i = 0; i < AffectedGroundEffectsType.Length; i++)
            {
                AffectedGroundEffectsType[i].Init();
            }
            PuzzleEventsManager.AddListener(this);
        }

        private void OnRenderObject()
        {
            ThrowRangeEffectManager.RenderObjectTick(AffectedGroundEffectsType);
            ThrowCursorRangeEffectManager.RenderObjectTick(AffectedGroundEffectsType);
        }

        public void Tick(float d)
        {
            ThrowRangeEffectManager.Tick(d);
        }

        #region External Events

        public void ReceivedEvend(PuzzleEvent puzzleEvent)
        {
            if (puzzleEvent.IsEventOfType<ThrowProjectileActionStartEvent>())
            {
                var puzzEv = puzzleEvent as ThrowProjectileActionStartEvent;
                ThrowRangeEffectManager.OnThrowProjectileActionStart(puzzEv.ThrowerTransform, puzzEv.MaxRange);
                ThrowCursorRangeEffectManager.OnThrowProjectileActionStart(puzzEv.CurrentCursorPositionRetriever, LaunchProjectileInherentDataConfiguration.conf[puzzEv.ProjectileInvolved].EffectRange);
            }
            else if (puzzleEvent.IsEventOfType<ProjectileThrowedEvent>())
            {
                ThrowRangeEffectManager.OnThrowProjectileThrowed();
                ThrowCursorRangeEffectManager.OnThrowProjectileThrowed();
            }
        }
        #endregion
    }

    #region Throw range effect manager
    class ThrowRangeEffectManager
    {
        private GroundEffectsManagerComponent GroundEffectsManagerComponent;

        private bool throwRangeEnabled;
        private float currentRange;
        private float maxRange;
        private Transform throwerTransformRef;

        public ThrowRangeEffectManager(GroundEffectsManagerComponent rTPuzzleGroundEffectsManagerComponent)
        {
            GroundEffectsManagerComponent = rTPuzzleGroundEffectsManagerComponent;
        }

        public void RenderObjectTick(GroundEffectType[] affectedGroundEffectsType)
        {
            if (throwRangeEnabled)
            {
                for (var i = 0; i < affectedGroundEffectsType.Length; i++)
                {
                    GroundEffectsManagerComponent.RangeEffectMaterial.SetFloat("_Radius", currentRange);
                    GroundEffectsManagerComponent.RangeEffectMaterial.SetVector("_CenterWorldPosition", throwerTransformRef.position);
                    GroundEffectsManagerComponent.RangeEffectMaterial.SetPass(0);
                    Graphics.DrawMeshNow(affectedGroundEffectsType[i].MeshFilter.mesh, affectedGroundEffectsType[i].transform.position, affectedGroundEffectsType[i].transform.rotation);
                }

            }
        }

        public void Tick(float d)
        {
            if (throwRangeEnabled)
            {
                currentRange += (d * GroundEffectsManagerComponent.RangeExpandSpeed);
                currentRange = Mathf.Min(currentRange, maxRange);
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

        public void RenderObjectTick(GroundEffectType[] affectedGroundEffectsType)
        {
            if (throwRangeEnabled)
            {
                var currentCursorPosition = cursorPositionRetriever.Invoke();
                if (currentCursorPosition.HasValue)
                {
                    for (var i = 0; i < affectedGroundEffectsType.Length; i++)
                    {
                        ThrowCursorRangeEffectManagerComponent.ProjectileEffectRangeMaterial.SetFloat("_Radius", projectileRange);
                        ThrowCursorRangeEffectManagerComponent.ProjectileEffectRangeMaterial.SetVector("_CenterWorldPosition", currentCursorPosition.Value);
                        ThrowCursorRangeEffectManagerComponent.ProjectileEffectRangeMaterial.SetPass(0);
                        Graphics.DrawMeshNow(affectedGroundEffectsType[i].MeshFilter.mesh, affectedGroundEffectsType[i].transform.position, affectedGroundEffectsType[i].transform.rotation);
                    }
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
    }

    [System.Serializable]
    public class ThrowCursorRangeEffectManagerComponent
    {
        public Material ProjectileEffectRangeMaterial;
    }
    #endregion
}

