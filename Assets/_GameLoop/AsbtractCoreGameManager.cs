using System.Collections;
using CoreGame;
using UnityEngine;

namespace GameLoop
{
    public abstract class AsbtractCoreGameManager : MonoBehaviour
    {
        private LevelType levelType;

        protected void OnAwake(LevelType levelType)
        {
            new GameLogHandler();

            this.levelType = levelType;

            CoreGameSingletonInstances.PersistanceManager.Init();
            CoreGameSingletonInstances.StartLevelManager.Init();
            CoreGameSingletonInstances.GameInputManager.Init(levelType);
            CoreGameSingletonInstances.LevelAvailabilityManager.Init();
            CoreGameSingletonInstances.ATimelinesManager.Init();
            CoreGameSingletonInstances.LevelManager.Init(levelType);

            if (this.levelType != LevelType.STARTMENU)
            {
                CoreGameSingletonInstances.PlayerAdventurePositionManager.Init();
                CoreGameSingletonInstances.LevelChunkFXTransitionManager.Init();
                CoreGameSingletonInstances.Coroutiner.StartCoroutine(InitializeTimelinesAtEndOfFrame());
            }
        }


        protected void OnStart()
        {
        }

        protected void BeforeTick(float d)
        {
            CoreGameSingletonInstances.PersistanceManager.Tick(d);
            if (levelType != LevelType.STARTMENU) CoreGameSingletonInstances.LevelChunkFXTransitionManager.Tick(d);
        }

        private IEnumerator InitializeTimelinesAtEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            CoreGameSingletonInstances.ATimelinesManager.InitTimelinesAtEndOfFrame();
        }
    }
}