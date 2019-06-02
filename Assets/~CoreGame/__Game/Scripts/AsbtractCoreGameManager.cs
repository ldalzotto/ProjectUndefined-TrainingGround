using UnityEngine;
using System.Collections;

namespace CoreGame
{
    public abstract class AsbtractCoreGameManager : MonoBehaviour
    {

        private ATimelinesManager ATimelinesManager;

        protected void OnAwake()
        {
            this.ATimelinesManager = GameObject.FindObjectOfType<ATimelinesManager>();
            var Coroutiner = GameObject.FindObjectOfType<Coroutiner>();


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

