using System;
using UnityEngine;

namespace CoreGame
{
    public static class CoreGameSingletonInstances
    {
        private static PersistanceManager persistanceManager;
        private static ATimelinesManager aTimelinesManager;
        private static GameInputManager gameInputManager;
        private static Coroutiner coroutiner;
        private static LevelChunkFXTransitionManager levelChunkFXTransitionManager;
        private static LevelAvailabilityManager levelAvailabilityManager;
        private static TimelinesEventManager timelinesEventManager;
        private static LevelTransitionManager levelTransitionManager;
        private static LevelManagerEventManager levelManagerEventManager;
        private static LevelMemoryManager levelMemoryManager;
        private static PlayerAdventurePositionManager playerAdventurePositionManager;
        private static LevelManager levelManager;
        private static DiscussionPositionManager discussionPositionManager;
        private static CoreConfigurationManager coreConfigurationManager;
        private static PlayerManagerType playerManagerType;
        private static CoreStaticConfigurationContainer coreStaticConfigurationContainer;
        private static AutoSaveIcon autoSaveIcon;
        private static DiscussionPositionsType discussionPositionsType;
        private static TutorialManager tutorialManager;
        private static StartLevelManager startLevelManager;

        public static PersistanceManager PersistanceManager => FindAndSetInstanceIfNull(persistanceManager, obj => persistanceManager = obj);

        public static ATimelinesManager ATimelinesManager => FindAndSetInstanceIfNull(aTimelinesManager, obj => aTimelinesManager = obj);

        public static GameInputManager GameInputManager => FindAndSetInstanceIfNull(gameInputManager, obj => gameInputManager = obj);

        public static Coroutiner Coroutiner => FindAndSetInstanceIfNull(coroutiner, obj => coroutiner = obj);

        public static LevelChunkFXTransitionManager LevelChunkFXTransitionManager => FindAndSetInstanceIfNull(levelChunkFXTransitionManager, obj => levelChunkFXTransitionManager = obj);

        public static LevelAvailabilityManager LevelAvailabilityManager => FindAndSetInstanceIfNull(levelAvailabilityManager, obj => levelAvailabilityManager = obj);

        public static TimelinesEventManager TimelinesEventManager => FindAndSetInstanceIfNull(timelinesEventManager, obj => timelinesEventManager = obj);

        public static LevelTransitionManager LevelTransitionManager => FindAndSetInstanceIfNull(levelTransitionManager, obj => levelTransitionManager = obj);

        public static LevelManagerEventManager LevelManagerEventManager => FindAndSetInstanceIfNull(levelManagerEventManager, obj => levelManagerEventManager = obj);

        public static LevelMemoryManager LevelMemoryManager => FindAndSetInstanceIfNull(levelMemoryManager, obj => levelMemoryManager = obj);

        public static PlayerAdventurePositionManager PlayerAdventurePositionManager => FindAndSetInstanceIfNull(playerAdventurePositionManager, obj => playerAdventurePositionManager = obj);

        public static LevelManager LevelManager => FindAndSetInstanceIfNull(levelManager, obj => levelManager = obj);

        public static DiscussionPositionManager DiscussionPositionManager => FindAndSetInstanceIfNull(discussionPositionManager, obj => discussionPositionManager = obj);

        public static CoreConfigurationManager CoreConfigurationManager => FindAndSetInstanceIfNull(coreConfigurationManager, obj => coreConfigurationManager = obj);

        public static PlayerManagerType PlayerManagerType => FindAndSetInstanceIfNull(playerManagerType, obj => playerManagerType = obj);

        public static CoreStaticConfigurationContainer CoreStaticConfigurationContainer => FindAndSetInstanceIfNull(coreStaticConfigurationContainer, obj => coreStaticConfigurationContainer = obj);

        public static AutoSaveIcon AutoSaveIcon => FindAndSetInstanceIfNull(autoSaveIcon, obj => autoSaveIcon = obj);

        public static Canvas GameCanvas
        {
            get
            {
                if (LevelManager.CurrentLevelType == LevelType.ADVENTURE)
                    return GameObject.FindGameObjectWithTag(TagConstants.ADVENTURE_CANVAS).GetComponent<Canvas>();
                else if (LevelManager.CurrentLevelType == LevelType.PUZZLE)
                    return GameObject.FindGameObjectWithTag(TagConstants.PUZZLE_CANVAS).GetComponent<Canvas>();
                else if (LevelManager.CurrentLevelType == LevelType.STARTMENU) return GameObject.FindGameObjectWithTag(TagConstants.START_MENU_CANVAS).GetComponent<Canvas>();

                return null;
            }
        }

        public static DiscussionPositionsType DiscussionPositionsType => FindAndSetInstanceIfNull(discussionPositionsType, obj => discussionPositionsType = obj);

        public static TutorialManager TutorialManager => FindAndSetInstanceIfNull(tutorialManager, obj => tutorialManager = obj);

        public static StartLevelManager StartLevelManager => NewInstanceIfNull(startLevelManager, obj => startLevelManager = obj);

        public static T FindAndSetInstanceIfNull<T>(T obj, Action<T> setter) where T : Behaviour
        {
            if (obj == null)
            {
                var foundObj = GameObject.FindObjectOfType<T>();
                setter.Invoke(foundObj);
                return foundObj;
            }
            else
            {
                return obj;
            }
        }

        public static T NewInstanceIfNull<T>(T obj, Action<T> setter)
        {
            if (obj == null)
            {
                var createdObject = Activator.CreateInstance<T>();
                setter.Invoke(createdObject);
                return createdObject;
            }
            else
            {
                return obj;
            }
        }
    }
}