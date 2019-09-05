using CoreGame;
using System.Linq;
using UnityEngine;

namespace AdventureGame
{
    public class GameManager : AsbtractCoreGameManager
    {

        private ContextActionManager ContextActionManager;
        private ContextActionWheelManager ContextActionWheelManager;
        private PlayerPointOfInterestSelectionManager PlayerPointOfInterestSelectionManager;
        private PlayerManager PlayerManager;
        private NPCManager NPCManager;
        private InventoryManager InventoryManager;
        private DiscussionManager DiscussionManager;
        private PointOfInterestManager PointOfInterestManager;
        private CutscenePlayerManagerV2 CutscenePlayerManagerV2;
        private CameraMovementManager CameraMovementManager;
        private AdventureTutorialEventSender AdventureTutorialEventSender;
        private TutorialManager TutorialManager;

#if UNITY_EDITOR
        private EditorOnlyModules EditorOnlyModules = new EditorOnlyModules();
#endif

        private void Awake()
        {
            GameObject.FindObjectOfType<GameManagerPersistanceInstance>().Init();
            this.AfterGameManagerPersistanceInstanceInitialization();
            base.OnAwake(LevelType.ADVENTURE);
        }

        protected virtual void AfterGameManagerPersistanceInstanceInitialization() { }

        void Start()
        {
            base.OnStart();

            //load dynamic POI
            var allLoadedLevelChunkID = GameObject.FindObjectOfType<LevelManager>().AllLoadedLevelZonesChunkID;
            var allActivePOIDefinitionIds = GameObject.FindObjectsOfType<PointOfInterestType>().ToList().ConvertAll(p => p.PointOfInterestDefinitionID);
            foreach (var elligiblePOIDefinitionIdTo in CoreGameSingletonInstances.AGhostPOIManager.GetAllPOIIdElligibleToBeDynamicallyInstanciated(allLoadedLevelChunkID))
            {
                if (!allActivePOIDefinitionIds.Contains(elligiblePOIDefinitionIdTo))
                {
                    PointOfInterestType.Instanciate(elligiblePOIDefinitionIdTo);
                }
            }

            var InventoryMenu = AInventoryMenu.FindCurrentInstance();
            InventoryMenu.gameObject.SetActive(true);

            ContextActionManager = FindObjectOfType<ContextActionManager>();
            ContextActionWheelManager = FindObjectOfType<ContextActionWheelManager>();
            PlayerPointOfInterestSelectionManager = FindObjectOfType<PlayerPointOfInterestSelectionManager>();
            PlayerManager = FindObjectOfType<PlayerManager>();
            NPCManager = FindObjectOfType<NPCManager>();
            InventoryManager = FindObjectOfType<InventoryManager>();
            DiscussionManager = FindObjectOfType<DiscussionManager>();
            PointOfInterestManager = GameObject.FindObjectOfType<PointOfInterestManager>();
            CutscenePlayerManagerV2 = GameObject.FindObjectOfType<CutscenePlayerManagerV2>();
            CameraMovementManager = GameObject.FindObjectOfType<CameraMovementManager>();
            AdventureTutorialEventSender = GameObject.FindObjectOfType<AdventureTutorialEventSender>();
            TutorialManager = GameObject.FindObjectOfType<TutorialManager>();

            //initialization
            CameraMovementManager.Init();
            GameObject.FindObjectOfType<AdventureEventsManager>().Init();
            GameObject.FindObjectOfType<CutscenePositionsManager>().Init();
            CutscenePlayerManagerV2.Init();
            PlayerPointOfInterestSelectionManager.Init();
            PlayerManager.Init();
            FindObjectOfType<InventoryEventManager>().Init();
            GameObject.FindObjectOfType<InventoryMenu>().Init();
            InventoryManager.Init();
            PointOfInterestManager.Init();
            GameObject.FindObjectOfType<CutsceneGlobalController>().Init();
            GameObject.FindObjectOfType<CutsceneEventManager>().Init();
            DiscussionManager.Init();
            AdventureTutorialEventSender.Init();
            GameObject.FindObjectOfType<ContextActionWheelEventManager>().Init();
            TutorialManager.Init();

#if UNITY_EDITOR
            this.EditorOnlyModules.Init();
#endif
        }

        void Update()
        {
            if (!this.IsInitializing)
            {
                var d = Time.deltaTime;

                this.BeforeTick(d);

                TutorialManager.Tick(d);
                CutscenePlayerManagerV2.Tick(d);
                if (!CutscenePlayerManagerV2.IsCutscenePlaying)
                {
                    this.AdventureTutorialEventSender.Tick(d);
                }
                ContextActionWheelManager.Tick(d);
                ContextActionManager.Tick(d);
                PointOfInterestManager.Tick(d);
                PlayerPointOfInterestSelectionManager.Tick(d);
                PlayerManager.Tick(d);
                NPCManager.Tick(d);
                CameraMovementManager.Tick(d);
                DiscussionManager.Tick(d);
                InventoryManager.Tick(d);

#if UNITY_EDITOR
                this.EditorOnlyModules.Tick(d);
#endif
            }

        }

        private void FixedUpdate()
        {
            var d = Time.fixedDeltaTime;
            PlayerManager.FixedTick(d);
            NPCManager.FixedTick(d);
        }

        private void LateUpdate()
        {
            var d = Time.deltaTime;
            PointOfInterestManager.LateTick(d);
            PlayerManager.LateTick(d);
        }

        private void OnGUI()
        {

        }

        private void OnDrawGizmos()
        {
            if (PlayerManager != null)
            {
                PlayerManager.OnGizmoTick();
            }
        }


    }

#if UNITY_EDITOR
    class EditorOnlyModules
    {
        private AdventureDebugModule AdventureDebugModule;

        public void Init()
        {
            this.AdventureDebugModule = GameObject.FindObjectOfType<AdventureDebugModule>();
            this.AdventureDebugModule.Init();
        }

        public void Tick(float d)
        {
            this.AdventureDebugModule.Tick(d);
        }
    }
#endif

}
