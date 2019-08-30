using System.Collections;
using UnityEngine;

namespace CoreGame
{
    public abstract class AsbtractCoreGameManager : MonoBehaviour
    {
        private bool isInitializing;

        protected bool IsInitializing { get => isInitializing; }

        protected void OnAwake(LevelType levelType)
        {
            new GameLogHandler();

            this.isInitializing = true;

            CoreGameSingletonInstances.PersistanceManager.Init();
            CoreGameSingletonInstances.GameInputManager.Init();
            CoreGameSingletonInstances.LevelAvailabilityManager.Init();
            CoreGameSingletonInstances.AGhostPOIManager.Init();
            CoreGameSingletonInstances.ATimelinesManager.Init();
            CoreGameSingletonInstances.PlayerAdventurePositionManager.Init();
            CoreGameSingletonInstances.APointOfInterestEventManager.Init();
            CoreGameSingletonInstances.LevelManager.Init(levelType);
            CoreGameSingletonInstances.LevelMemoryManager.Init(levelType, CoreGameSingletonInstances.LevelManager);
            CoreGameSingletonInstances.LevelChunkFXTransitionManager.Init();

            CoreGameSingletonInstances.Coroutiner.StartCoroutine(this.InitializeTimelinesAtEndOfFrame());
        }


        protected void OnStart()
        {
            this.PointOfInterestInitialisation();
            CoreGameSingletonInstances.Coroutiner.StartCoroutine(this.PointOfInterestInitialisationAtEndOfFrame());
        }

        protected void BeforeTick(float d)
        {
            CoreGameSingletonInstances.PersistanceManager.Tick(d);
            CoreGameSingletonInstances.LevelChunkFXTransitionManager.Tick(d);
        }

        private IEnumerator InitializeTimelinesAtEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            CoreGameSingletonInstances.ATimelinesManager.InitTimelinesAtEndOfFrame();
        }

        private void PointOfInterestInitialisation()
        {
            var allActivePOI = GameObject.FindObjectsOfType<APointOfInterestType>();
            if (allActivePOI != null)
            {
                for (var i = 0; i < allActivePOI.Length; i++)
                {
                    allActivePOI[i].Init();
                }
            }
        }

        private IEnumerator PointOfInterestInitialisationAtEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            var allActivePOI = GameObject.FindObjectsOfType<APointOfInterestType>();
            if (allActivePOI != null)
            {
                for (var i = 0; i < allActivePOI.Length; i++)
                {
                    Debug.Log(MyLog.Format(allActivePOI[i].PointOfInterestId.ToString()));
                    allActivePOI[i].Init_EndOfFrame();
                }
            }
            this.isInitializing = false;
        }
    }

}

