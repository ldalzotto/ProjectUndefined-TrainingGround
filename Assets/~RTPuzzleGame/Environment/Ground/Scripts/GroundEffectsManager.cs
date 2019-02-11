using UnityEngine;

namespace RTPuzzle
{
    public class GroundEffectsManager : MonoBehaviour, PuzzleEventsListener
    {
        #region External Dependencies
        private PuzzleEventsManager PuzzleEventsManager;
        #endregion

        public GroundEffectsManagerComponent GroundEffectsManagerComponent;

        private ThrowRangeEffectManager ThrowRangeEffectManager;

        private GroundEffectType[] AffectedGroundEffectsType;

        public void Init()
        {
            ThrowRangeEffectManager = new ThrowRangeEffectManager(GroundEffectsManagerComponent);
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
        }

        public void Tick(float d)
        {
            ThrowRangeEffectManager.Tick(d);
        }

        #region External Events
        private void OnThrowProjectileActionStart(Transform throwerTransform, float maxRange)
        {
            ThrowRangeEffectManager.OnThrowProjectileActionStart(throwerTransform, maxRange);
        }

        private void OnThrowProjectileThrowed()
        {
            ThrowRangeEffectManager.OnThrowProjectileThrowed();
        }

        public void ReceivedEvend(PuzzleEvent puzzleEvent)
        {
            if (puzzleEvent.IsEventOfType<ThrowProjectileActionStartEvent>())
            {
                var puzzEv = puzzleEvent as ThrowProjectileActionStartEvent;
                OnThrowProjectileActionStart(puzzEv.ThrowerTransform, puzzEv.MaxRange);
            }
            else if (puzzleEvent.IsEventOfType<ProjectileThrowedEvent>())
            {
                OnThrowProjectileThrowed();
            }
        }
        #endregion
    }

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
}

