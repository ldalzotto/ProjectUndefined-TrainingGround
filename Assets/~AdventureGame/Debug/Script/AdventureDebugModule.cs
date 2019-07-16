using GameConfigurationID;
using UnityEngine;

namespace AdventureGame
{

#if UNITY_EDITOR
    public class AdventureDebugModule : MonoBehaviour
    {

        [Header("Cutscene")]
        [CustomEnum()]
        public CutsceneId cutsceneId;
        public bool play;

        [Header("Camera Follow")]
        [CustomEnum()]
        public PointOfInterestId poiToFollow;
        public bool follow;

        private CutscenePlayerManagerV2 CutscenePlayerManagerV2;
        private CutsceneGlobalController CutsceneGlobalController;

        public void Init()
        {
            this.CutscenePlayerManagerV2 = GameObject.FindObjectOfType<CutscenePlayerManagerV2>();
            this.CutsceneGlobalController = GameObject.FindObjectOfType<CutsceneGlobalController>();
        }

        public void Tick(float d)
        {
            if (this.play)
            {
                this.play = false;
                this.CutscenePlayerManagerV2.OnCutsceneStart(this.cutsceneId);
            }
            if (this.follow)
            {
                this.follow = false;
                this.CutsceneGlobalController.SetCameraFollow(this.poiToFollow);
            }
        }
    }
#endif
}

