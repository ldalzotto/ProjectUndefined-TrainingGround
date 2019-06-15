using System.Collections;
using UnityEngine;

namespace CoreGame
{
    public abstract class AsbtractCoreGameManager : MonoBehaviour
    {

        private ATimelinesManager ATimelinesManager;
        private GameInputManager GameInputManager;
        private PersistanceManager PersistanceManager;

        protected void OnAwake()
        {
            this.PersistanceManager = GameObject.FindObjectOfType<PersistanceManager>();
            this.ATimelinesManager = GameObject.FindObjectOfType<ATimelinesManager>();
            this.GameInputManager = GameObject.FindObjectOfType<GameInputManager>();
            var Coroutiner = GameObject.FindObjectOfType<Coroutiner>();

            this.PersistanceManager.Init();
            this.GameInputManager.Init();
            GameObject.FindObjectOfType<LevelAvailabilityManager>().Init();
            GameObject.FindObjectOfType<AGhostPOIManager>().Init();
            this.ATimelinesManager.Init();
            GameObject.FindObjectOfType<TimelinesEventManager>().Init();
            Coroutiner.StartCoroutine(this.InitializeTimelinesAtEndOfFrame());
        }

        protected void BeforeTick(float d)
        {
            this.PersistanceManager.Tick(d);
        }

        private IEnumerator InitializeTimelinesAtEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            ATimelinesManager.InitTimelinesAtEndOfFrame();
        }

    }
}

