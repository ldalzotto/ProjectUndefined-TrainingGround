using UnityEngine;

namespace RTPuzzle
{
    public class GroundEffectsEventManager : MonoBehaviour
    {

        private GroundEffectsManager GroundEffectsManager;

        public void Init()
        {
            GroundEffectsManager = GameObject.FindObjectOfType<GroundEffectsManager>();
        }

        public void OnThrowProjectileActionStart(Transform throwerTransform, float maxRange)
        {
            GroundEffectsManager.OnThrowProjectileActionStart(throwerTransform, maxRange);
        }

        public void OnThrowProjectileThrowed()
        {
            GroundEffectsManager.OnThrowProjectileThrowed();
        }

    }

}
