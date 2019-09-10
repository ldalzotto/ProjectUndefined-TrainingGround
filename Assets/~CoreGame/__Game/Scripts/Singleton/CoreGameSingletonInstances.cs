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
        private static AGhostPOIManager aGhostPOIManager;
        private static LevelChunkFXTransitionManager levelChunkFXTransitionManager;
        private static LevelAvailabilityManager levelAvailabilityManager;
        private static TimelinesEventManager timelinesEventManager;
        private static LevelTransitionManager levelTransitionManager;
        private static LevelManagerEventManager levelManagerEventManager;
        private static LevelMemoryManager levelMemoryManager;
        private static PlayerAdventurePositionManager playerAdventurePositionManager;
        private static APointOfInterestEventManager aPointOfInterestEventManager;
        private static LevelManager levelManager;
        private static DiscussionPositionManager discussionPositionManager;
        private static CoreConfigurationManager coreConfigurationManager;
        private static PlayerManagerType playerManagerType;
        private static CoreStaticConfigurationContainer coreStaticConfigurationContainer;
        private static CircleFillBarRendererManager circleFillBarRendererManager;
        private static AutoSaveIcon autoSaveIcon;
        private static DiscussionPositionsType discussionPositionsType;
        private static FXContainerManager fXContainerManager;

        public static PersistanceManager PersistanceManager { get => FindAndSetInstanceIfNull(persistanceManager, obj => persistanceManager = obj); }
        public static ATimelinesManager ATimelinesManager { get => FindAndSetInstanceIfNull(aTimelinesManager, obj => aTimelinesManager = obj); }
        public static GameInputManager GameInputManager { get => FindAndSetInstanceIfNull(gameInputManager, obj => gameInputManager = obj); }
        public static Coroutiner Coroutiner { get => FindAndSetInstanceIfNull(coroutiner, obj => coroutiner = obj); }
        public static AGhostPOIManager AGhostPOIManager { get => FindAndSetInstanceIfNull(aGhostPOIManager, obj => aGhostPOIManager = obj); }
        public static LevelChunkFXTransitionManager LevelChunkFXTransitionManager { get => FindAndSetInstanceIfNull(levelChunkFXTransitionManager, obj => levelChunkFXTransitionManager = obj); }
        public static LevelAvailabilityManager LevelAvailabilityManager { get => FindAndSetInstanceIfNull(levelAvailabilityManager, obj => levelAvailabilityManager = obj); }
        public static TimelinesEventManager TimelinesEventManager { get => FindAndSetInstanceIfNull(timelinesEventManager, obj => timelinesEventManager = obj); }
        public static LevelTransitionManager LevelTransitionManager { get => FindAndSetInstanceIfNull(levelTransitionManager, obj => levelTransitionManager = obj); }
        public static LevelManagerEventManager LevelManagerEventManager { get => FindAndSetInstanceIfNull(levelManagerEventManager, obj => levelManagerEventManager = obj); }
        public static LevelMemoryManager LevelMemoryManager { get => FindAndSetInstanceIfNull(levelMemoryManager, obj => levelMemoryManager = obj); }
        public static PlayerAdventurePositionManager PlayerAdventurePositionManager { get => FindAndSetInstanceIfNull(playerAdventurePositionManager, obj => playerAdventurePositionManager = obj); }
        public static APointOfInterestEventManager APointOfInterestEventManager { get => FindAndSetInstanceIfNull(aPointOfInterestEventManager, obj => aPointOfInterestEventManager = obj); }
        public static LevelManager LevelManager { get => FindAndSetInstanceIfNull(levelManager, obj => levelManager = obj); }
        public static DiscussionPositionManager DiscussionPositionManager { get => FindAndSetInstanceIfNull(discussionPositionManager, obj => discussionPositionManager = obj); }
        public static CoreConfigurationManager CoreConfigurationManager { get => FindAndSetInstanceIfNull(coreConfigurationManager, obj => coreConfigurationManager = obj); }
        public static PlayerManagerType PlayerManagerType { get => FindAndSetInstanceIfNull(playerManagerType, obj => playerManagerType = obj); }
        public static CoreStaticConfigurationContainer CoreStaticConfigurationContainer { get => FindAndSetInstanceIfNull(coreStaticConfigurationContainer, obj => coreStaticConfigurationContainer = obj); }
        public static CircleFillBarRendererManager CircleFillBarRendererManager { get => FindAndSetInstanceIfNull(circleFillBarRendererManager, obj => circleFillBarRendererManager = obj); }
        public static AutoSaveIcon AutoSaveIcon { get => FindAndSetInstanceIfNull(autoSaveIcon, obj => autoSaveIcon = obj); }
        public static Canvas GameCanvas
        {
            get
            {
                if (CoreGameSingletonInstances.LevelManager.CurrentLevelType == LevelType.ADVENTURE)
                {
                    return GameObject.FindGameObjectWithTag(TagConstants.ADVENTURE_CANVAS).GetComponent<Canvas>();
                }
                else
                {
                    return GameObject.FindGameObjectWithTag(TagConstants.PUZZLE_CANVAS).GetComponent<Canvas>();
                }
            }
        }
        public static DiscussionPositionsType DiscussionPositionsType { get => FindAndSetInstanceIfNull(discussionPositionsType, obj => discussionPositionsType = obj); }
        public static FXContainerManager FXContainerManager { get => FindAndSetInstanceIfNull(fXContainerManager, obj => fXContainerManager = obj); }

        public static T FindAndSetInstanceIfNull<T>(T obj, Action<T> setter) where T : Behaviour
        {
            if (obj == null)
            {
                T foundObj = GameObject.FindObjectOfType<T>();
                setter.Invoke(foundObj);
                return foundObj;
            }
            else { return obj; }
        }

    }

}
