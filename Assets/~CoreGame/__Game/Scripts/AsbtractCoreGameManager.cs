using System.Collections;
using UnityEngine;

namespace CoreGame
{
    public abstract class AsbtractCoreGameManager : MonoBehaviour
    {

        private ATimelinesManager ATimelinesManager;
        private GameInputManager GameInputManager;

        protected void OnAwake()
        {
            this.ATimelinesManager = GameObject.FindObjectOfType<ATimelinesManager>();
            this.GameInputManager = GameObject.FindObjectOfType<GameInputManager>();
            var Coroutiner = GameObject.FindObjectOfType<Coroutiner>();

            this.GameInputManager.Init();
            GameObject.FindObjectOfType<LevelAvailabilityManager>().Init();
            GameObject.FindObjectOfType<AGhostPOIManager>().Init();
            this.ATimelinesManager.Init();
            GameObject.FindObjectOfType<TimelinesEventManager>().Init();
            Coroutiner.StartCoroutine(this.InitializeTimelinesAtEndOfFrame());
        }

        private IEnumerator InitializeTimelinesAtEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            ATimelinesManager.InitTimelinesAtEndOfFrame();
        }
    }
}

