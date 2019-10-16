using System.Collections;
using UnityEngine;

namespace CoreGame
{
    public abstract class AsbtractCoreGameManager : MonoBehaviour
    {
        private LevelType levelType;
        private bool isInitializing;

        protected bool IsInitializing { get => isInitializing; }

        protected void OnAwake(LevelType levelType)
        {
            new GameLogHandler();

            this.isInitializing = true;
            this.levelType = levelType;

            CoreGameSingletonInstances.PersistanceManager.Init();
            CoreGameSingletonInstances.StartLevelManager.Init();
            CoreGameSingletonInstances.GameInputManager.Init(levelType);
            CoreGameSingletonInstances.LevelAvailabilityManager.Init();
            CoreGameSingletonInstances.AGhostPOIManager.Init();
            CoreGameSingletonInstances.ATimelinesManager.Init();
            CoreGameSingletonInstances.LevelManager.Init(levelType);
            CanvasScalerManager.Get().Init();

            if (this.levelType != LevelType.STARTMENU)
            {
                CoreGameSingletonInstances.PlayerAdventurePositionManager.Init();
                CoreGameSingletonInstances.APointOfInterestEventManager.Init();
                CoreGameSingletonInstances.LevelMemoryManager.Init(levelType, CoreGameSingletonInstances.LevelManager);
                CoreGameSingletonInstances.LevelChunkFXTransitionManager.Init();
                CoreGameSingletonInstances.Coroutiner.StartCoroutine(this.InitializeTimelinesAtEndOfFrame());
            }
        }


        protected void OnStart()
        {
            if (this.levelType != LevelType.STARTMENU)
            {
                this.PointOfInterestInitialisation();
                CoreGameSingletonInstances.Coroutiner.StartCoroutine(this.PointOfInterestInitialisationAtEndOfFrame());
            }
        }

        protected void BeforeTick(float d)
        {
            CoreGameSingletonInstances.PersistanceManager.Tick(d);
            if (this.levelType != LevelType.STARTMENU)
            {
                CoreGameSingletonInstances.LevelChunkFXTransitionManager.Tick(d);
            }
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

