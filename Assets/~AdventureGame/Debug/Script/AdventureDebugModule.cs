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
        
        private CutscenePlayerManagerV2 CutscenePlayerManagerV2;

        public void Init()
        {
            this.CutscenePlayerManagerV2 = GameObject.FindObjectOfType<CutscenePlayerManagerV2>();
        }

        public void Tick(float d)
        {
            if (this.play)
            {
                this.play = false;
                this.CutscenePlayerManagerV2.OnCutsceneStart(this.cutsceneId);
            }
        }
    }
#endif
}

