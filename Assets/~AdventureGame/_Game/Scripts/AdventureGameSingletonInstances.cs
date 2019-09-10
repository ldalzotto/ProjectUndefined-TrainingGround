using CoreGame;

namespace AdventureGame
{
    public static class AdventureGameSingletonInstances
    {
        private static InventoryEventManager inventoryEventManager;
        private static AdventureDebugModule adventureDebugModule;
        private static PointOfInterestManager pointOfInterestManager;
        private static CutscenePlayerManagerV2 cutscenePlayerManagerV2;
        private static AdventureTutorialEventSender adventureTutorialEventSender;
        private static AdventureEventsManager adventureEventsManager;
        private static CutscenePositionsManager cutscenePositionsManager;
        private static InventoryMenu inventoryMenu;
        private static CutsceneGlobalController cutsceneGlobalController;
        private static CutsceneEventManager cutsceneEventManager;
        private static ContextActionWheelEventManager contextActionWheelEventManager;
        private static ContextActionEventManager contextActionEventManager;
        private static AdventureGameConfigurationManager adventureGameConfigurationManager;
        private static DiscussionEventHandler discussionEventHandler;
        private static ContextActionManager contextActionManager;
        private static PlayerManager playerManager;
        private static InventoryManager inventoryManager;
        private static ContextActionWheelManager contextActionWheelManager;
        private static SelectionWheel adventureSelectionWheel;
        private static GhostsPOIManager ghostsPOIManager;
        private static AdventureStaticConfigurationContainer adventureStaticConfigurationContainer;
        private static DiscussionManager discussionManager;
        private static DiscussionWindowsContainer discussionWindowsContainer;
        private static PlayerPointOfInterestSelectionManager playerPointOfInterestSelectionManager;
        private static PointOfInterestAdventureEventManager pointOfInterestAdventureEventManager;

        public static InventoryEventManager InventoryEventManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(inventoryEventManager, obj => inventoryEventManager = obj); }
        public static AdventureDebugModule AdventureDebugModule { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(adventureDebugModule, obj => adventureDebugModule = obj); }
        public static PointOfInterestManager PointOfInterestManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(pointOfInterestManager, obj => pointOfInterestManager = obj); }
        public static CutscenePlayerManagerV2 CutscenePlayerManagerV2 { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(cutscenePlayerManagerV2, obj => cutscenePlayerManagerV2 = obj); }
        public static AdventureTutorialEventSender AdventureTutorialEventSender { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(adventureTutorialEventSender, obj => adventureTutorialEventSender = obj); }
        public static AdventureEventsManager AdventureEventsManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(adventureEventsManager, obj => adventureEventsManager = obj); }
        public static CutscenePositionsManager CutscenePositionsManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(cutscenePositionsManager, obj => cutscenePositionsManager = obj); }
        public static InventoryMenu InventoryMenu { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(inventoryMenu, obj => inventoryMenu = obj); }
        public static CutsceneGlobalController CutsceneGlobalController { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(cutsceneGlobalController, obj => cutsceneGlobalController = obj); }
        public static CutsceneEventManager CutsceneEventManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(cutsceneEventManager, obj => cutsceneEventManager = obj); }
        public static ContextActionWheelEventManager ContextActionWheelEventManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(contextActionWheelEventManager, obj => contextActionWheelEventManager = obj); }
        public static ContextActionEventManager ContextActionEventManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(contextActionEventManager, obj => contextActionEventManager = obj); }
        public static AdventureGameConfigurationManager AdventureGameConfigurationManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(adventureGameConfigurationManager, obj => adventureGameConfigurationManager = obj); }
        public static DiscussionEventHandler DiscussionEventHandler { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(discussionEventHandler, obj => discussionEventHandler = obj); }
        public static ContextActionManager ContextActionManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(contextActionManager, obj => contextActionManager = obj); }
        public static PlayerManager PlayerManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(playerManager, obj => playerManager = obj); }
        public static InventoryManager InventoryManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(inventoryManager, obj => inventoryManager = obj); }
        public static ContextActionWheelManager ContextActionWheelManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(contextActionWheelManager, obj => contextActionWheelManager = obj); }
        public static SelectionWheel AdventureSelectionWheel { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(adventureSelectionWheel, obj => adventureSelectionWheel = obj); }
        public static GhostsPOIManager GhostsPOIManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(ghostsPOIManager, obj => ghostsPOIManager = obj); }
        public static AdventureStaticConfigurationContainer AdventureStaticConfigurationContainer { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(adventureStaticConfigurationContainer, obj => adventureStaticConfigurationContainer = obj); }
        public static DiscussionManager DiscussionManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(discussionManager, obj => discussionManager = obj); }
        public static DiscussionWindowsContainer DiscussionWindowsContainer { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(discussionWindowsContainer, obj => discussionWindowsContainer = obj); }
        public static PlayerPointOfInterestSelectionManager PlayerPointOfInterestSelectionManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(playerPointOfInterestSelectionManager, obj => playerPointOfInterestSelectionManager = obj); }
        public static PointOfInterestAdventureEventManager PointOfInterestAdventureEventManager { get => CoreGameSingletonInstances.FindAndSetInstanceIfNull(pointOfInterestAdventureEventManager, obj => pointOfInterestAdventureEventManager = obj); }
    }
}
