using System.Linq;
using CoreGame;
using InteractiveObjects;
using Obstacle;
using RangeObjects;
using UnityEngine;

namespace AdventureGame
{
    public class GameManager : AsbtractCoreGameManager
    {
        private AdventureTutorialEventSender AdventureTutorialEventSender;
        private ContextActionManager ContextActionManager;
        private ContextActionWheelManager ContextActionWheelManager;
        private CutscenePlayerManagerV2 CutscenePlayerManagerV2;
        private DiscussionManager DiscussionManager;

#if UNITY_EDITOR
        private EditorOnlyModules EditorOnlyModules = new EditorOnlyModules();
#endif
        private InventoryManager InventoryManager;
        private NPCManager NPCManager;
        private PlayerManager PlayerManager;
        private PointOfInterestManager PointOfInterestManager;
        private TutorialManager TutorialManager;

        private void Awake()
        {
            FindObjectOfType<GameManagerPersistanceInstance>().Init();
            AfterGameManagerPersistanceInstanceInitialization();
            OnAwake(LevelType.ADVENTURE);
        }

        protected virtual void AfterGameManagerPersistanceInstanceInitialization()
        {
        }

        private void Start()
        {
            OnStart();

            //load dynamic POI
            var allLoadedLevelChunkID = CoreGameSingletonInstances.LevelManager.AllLoadedLevelZonesChunkID;
            var allActivePOIDefinitionIds = FindObjectsOfType<PointOfInterestType>().ToList().ConvertAll(p => p.PointOfInterestDefinitionID);
            foreach (var elligiblePOIDefinitionIdTo in CoreGameSingletonInstances.AGhostPOIManager.GetAllPOIIdElligibleToBeDynamicallyInstanciated(allLoadedLevelChunkID))
                if (!allActivePOIDefinitionIds.Contains(elligiblePOIDefinitionIdTo))
                    PointOfInterestType.Instanciate(elligiblePOIDefinitionIdTo);

            var InventoryMenu = AInventoryMenu.FindCurrentInstance();
            InventoryMenu.gameObject.SetActive(true);

            ContextActionManager = FindObjectOfType<ContextActionManager>();
            ContextActionWheelManager = FindObjectOfType<ContextActionWheelManager>();
            PlayerManager = FindObjectOfType<PlayerManager>();
            NPCManager = FindObjectOfType<NPCManager>();
            InventoryManager = FindObjectOfType<InventoryManager>();
            DiscussionManager = FindObjectOfType<DiscussionManager>();
            PointOfInterestManager = AdventureGameSingletonInstances.PointOfInterestManager;
            CutscenePlayerManagerV2 = AdventureGameSingletonInstances.CutscenePlayerManagerV2;
            AdventureTutorialEventSender = AdventureGameSingletonInstances.AdventureTutorialEventSender;
            TutorialManager = CoreGameSingletonInstances.TutorialManager;

            //initialization
            CameraMovementManager.Get().Init();
            AdventureGameSingletonInstances.AdventureEventsManager.Init();
            AdventureGameSingletonInstances.CutscenePositionsManager.Init();


            RangeObjectV2Manager.Get().Init();
            InteractiveObjectV2Manager.Get().Init();

            CutscenePlayerManagerV2.Init();
            PlayerManager.Init();
            FindObjectOfType<InventoryEventManager>().Init();
            AdventureGameSingletonInstances.InventoryMenu.Init();
            InventoryManager.Init();
            PointOfInterestManager.Init();
            AdventureGameSingletonInstances.CutsceneGlobalController.Init();
            AdventureGameSingletonInstances.CutsceneEventManager.Init();
            DiscussionManager.Init();
            AdventureTutorialEventSender.Init();
            AdventureGameSingletonInstances.ContextActionWheelEventManager.Init();
            TutorialManager.Init();

#if UNITY_EDITOR
            EditorOnlyModules.Init();
#endif
        }

        private void Update()
        {
            if (!IsInitializing)
            {
                var d = Time.deltaTime;

                BeforeTick(d);

                ObstacleOcclusionCalculationManagerV2.Get().Tick(d);
                RangeIntersectionCalculationManagerV2.Get().Tick(d);

                TutorialManager.Tick(d);
                CutscenePlayerManagerV2.Tick(d);
                if (!CutscenePlayerManagerV2.IsCutscenePlaying) AdventureTutorialEventSender.Tick(d);

                RangeObjectV2Manager.Get().Tick(d);

                ContextActionWheelManager.Tick(d);
                ContextActionManager.Tick(d);
                PointOfInterestManager.Tick(d);
                PlayerPointOfInterestSelectionManager.Get().Tick(d);
                PlayerManager.Tick(d);

                InteractiveObjectV2Manager.Get().Tick(d);
                InteractiveObjectV2Manager.Get().AfterTicks();

                NPCManager.Tick(d);
                CameraMovementManager.Get().Tick(d);
                DiscussionManager.Tick(d);
                InventoryManager.Tick(d);

#if UNITY_EDITOR
                EditorOnlyModules.Tick(d);
#endif
            }
        }

        private void FixedUpdate()
        {
            var d = Time.fixedDeltaTime;
            PlayerManager.FixedTick(d);
            InteractiveObjectV2Manager.Get().FixedTick(d);
            NPCManager.FixedTick(d);
        }

        private void LateUpdate()
        {
            var d = Time.deltaTime;
            PointOfInterestManager.LateTick(d);
            InteractiveObjectV2Manager.Get().LateTick(d);
            PlayerManager.LateTick(d);
        }

        private void OnGUI()
        {
        }

        private void OnDrawGizmos()
        {
            if (PlayerManager != null) PlayerManager.OnGizmoTick();
        }
    }

#if UNITY_EDITOR
    internal class EditorOnlyModules
    {
        private AdventureDebugModule AdventureDebugModule;

        public void Init()
        {
            AdventureDebugModule = AdventureGameSingletonInstances.AdventureDebugModule;
            AdventureDebugModule.Init();
        }

        public void Tick(float d)
        {
            AdventureDebugModule.Tick(d);
        }
    }
#endif
}