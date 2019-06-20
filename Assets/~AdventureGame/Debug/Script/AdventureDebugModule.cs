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

        private CutscenePlayerManager CutscenePlayerManager;

        public void Init()
        {
            this.CutscenePlayerManager = GameObject.FindObjectOfType<CutscenePlayerManager>();
        }

        public void Tick(float d)
        {
            if (this.play)
            {
                this.play = false;
                this.CutscenePlayerManager.OnCutsceneStart(this.cutsceneId);
            }
        }
    }
#endif
}

