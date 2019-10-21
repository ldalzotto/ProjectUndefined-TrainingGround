using System.Collections;
using CoreGame;
using LevelManagement;
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
            StartLevelManager.Get().Init();
            if (levelType == LevelType.STARTMENU)
            {
                CoreGameSingletonInstances.GameInputManager.Init(CursorLockMode.Confined);
            }
            else
            {
                CoreGameSingletonInstances.GameInputManager.Init(CursorLockMode.Locked);
            }

            LevelAvailabilityManager.Get().Init();
            CoreGameSingletonInstances.ATimelinesManager.Init();
            LevelManager.Get().Init(levelType);

            if (this.levelType != LevelType.STARTMENU)
            {
                CoreGameSingletonInstances.PlayerAdventurePositionManager.Init();
                LevelChunkFXTransitionManager.Get().Init();
                CoreGameSingletonInstances.Coroutiner.StartCoroutine(InitializeTimelinesAtEndOfFrame());
            }
        }


        protected void OnStart()
        {
        }

        protected void BeforeTick(float d)
        {
            CoreGameSingletonInstances.PersistanceManager.Tick(d);
            if (levelType != LevelType.STARTMENU) LevelChunkFXTransitionManager.Get().Tick(d);
        }

        private IEnumerator InitializeTimelinesAtEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            CoreGameSingletonInstances.ATimelinesManager.InitTimelinesAtEndOfFrame();
        }
    }
}