using UnityEngine;

namespace RTPuzzle
{
    public class PuzzleDebugModule : MonoBehaviour
    {

#if UNITY_EDITOR
        public bool InstantProjectileHit;
        public bool TriggerGameOverEvent;
        public LevelZonesID NextZone;

        #region External Dependencies
        private PuzzleEventsManager PuzzleEventsManager;
        #endregion

        private void Start()
        {
            this.PuzzleEventsManager = GameObject.FindObjectOfType<PuzzleEventsManager>();
            if (InstantProjectileHit)
            {
              //  foreach (var launchProjectileConfiguration in puzzleConfigurationmanager.PuzzleGameConfiguration.ProjectileConfiguration.LaunchProjectileInherentDatas)
                //{
                  //  launchProjectileConfiguration.Value.SetTravelDistanceDebug(99999f);
                //}
            }
        }

        private void Update()
        {
            if (TriggerGameOverEvent)
            {
                this.PuzzleEventsManager.OnGameOver(NextZone);
            }
        }
#endif
    }


}
