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

        public void Init()
        {
            ThrowRangeEffectManager = new ThrowRangeEffectManager(GroundEffectsManagerComponent);
            PuzzleEventsManager = GameObject.FindObjectOfType<PuzzleEventsManager>();
            PuzzleEventsManager.AddListener(this);
        }

        private void OnRenderObject()
        {
            ThrowRangeEffectManager.RenderObjectTick();
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
        private Transform ModelRootTransform;

        private bool throwRangeEnabled;
        private float currentRange;
        private float maxRange;
        private Transform throwerTransformRef;
        private MeshFilter meshFilter;

        public ThrowRangeEffectManager(GroundEffectsManagerComponent rTPuzzleGroundEffectsManagerComponent)
        {
            GroundEffectsManagerComponent = rTPuzzleGroundEffectsManagerComponent;
            ModelRootTransform = rTPuzzleGroundEffectsManagerComponent.GroundMeshRenderer.transform;
            meshFilter = rTPuzzleGroundEffectsManagerComponent.GroundMeshRenderer.GetComponent<MeshFilter>();
        }

        public void RenderObjectTick()
        {
            if (throwRangeEnabled)
            {
                GroundEffectsManagerComponent.GroundMaterial.SetFloat("_Radius", currentRange);
                GroundEffectsManagerComponent.GroundMaterial.SetVector("_CenterWorldPosition", throwerTransformRef.position);
                GroundEffectsManagerComponent.GroundMaterial.SetPass(0);
                Graphics.DrawMeshNow(meshFilter.mesh, ModelRootTransform.position, ModelRootTransform.rotation);
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
        public Material GroundMaterial;
        public MeshRenderer GroundMeshRenderer;
        public float RangeExpandSpeed;
    }
}

