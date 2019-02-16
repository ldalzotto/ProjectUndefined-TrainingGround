using UnityEngine;

namespace RTPuzzle
{
    public class PuzzleDebugModule : MonoBehaviour
    {
        public bool InstantProjectileHit;
#if UNITY_EDITOR
        private void Start()
        {
            if (InstantProjectileHit)
            {
                foreach (var launchProjectileConfiguration in LaunchProjectileInherentDataConfiguration.conf)
                {
                    launchProjectileConfiguration.Value.SetTravelDistanceDebug(99999f);
                }
            }
        }
#endif
    }


}
