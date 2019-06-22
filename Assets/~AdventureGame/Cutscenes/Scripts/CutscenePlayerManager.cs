using GameConfigurationID;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

namespace AdventureGame
{
    public class CutscenePlayerManager : MonoBehaviour
    {
        #region External Dependencies
        private AdventureGameConfigurationManager AdventureGameConfigurationManager;
        #endregion

        #region state
        private bool isCutscenePlaying = false;
        public bool IsCutscenePlaying { get => isCutscenePlaying; }
        #endregion

        private PlayableDirector playableDirector;

        public void Init()
        {
            this.playableDirector = GetComponent<PlayableDirector>();
            this.AdventureGameConfigurationManager = GameObject.FindObjectOfType<AdventureGameConfigurationManager>();
        }

        #region External Event
        public void OnCutsceneStart(CutsceneId cutsceneId)
        {
            Coroutiner.Instance.StartCoroutine(this.PlayCutscene(this.AdventureGameConfigurationManager.CutsceneConf()[cutsceneId].PlayableAsset));
        }
        #endregion

        public IEnumerator PlayCutscene(CutsceneId cutsceneId)
        {
            yield return Coroutiner.Instance.StartCoroutine(this.PlayCutscene(this.AdventureGameConfigurationManager.CutsceneConf()[cutsceneId].PlayableAsset));
        }

        private IEnumerator PlayCutscene(PlayableAsset playableAsset)
        {
            this.playableDirector.Stop();
            this.isCutscenePlaying = true;
            this.playableDirector.Play(playableAsset);
            yield return new WaitEndOfCutscene(this.playableDirector);
            this.isCutscenePlaying = false;
            this.playableDirector.playableAsset = null;
        }

    }
}
