using UnityEngine;
using System.Collections;

namespace CoreGame
{
    public abstract class AsbtractCoreGameManager : MonoBehaviour
    {

        private ATimelinesManager ATimelinesManager;

        protected void OnStart()
        {
            this.ATimelinesManager = GameObject.FindObjectOfType<ATimelinesManager>();
            var Coroutiner = GameObject.FindObjectOfType<Coroutiner>();

            this.ATimelinesManager.Init();
            Coroutiner.StartCoroutine(this.InitializeAllTimelinesAtEndOfFrame());
        }

        private IEnumerator InitializeAllTimelinesAtEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            ATimelinesManager.InitAllTimelines();
        }

        private void OnApplicationQuit()
        {
            this.ATimelinesManager.ApplicationQuit();
        }
    }
}

